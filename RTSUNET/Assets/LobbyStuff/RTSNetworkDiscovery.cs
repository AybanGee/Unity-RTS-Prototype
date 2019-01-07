using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class RTSNetworkDiscovery : NetworkDiscovery {
	public static string gameName = "local";
	public static int target = 0;
	public static RTSNetworkDiscovery singleton;
	private float timeout = 5f;
	public static bool stopConfirmed = false;
	private List<LanConnectionInfo> lanAddresses = new List<LanConnectionInfo> ();

	private void Start () {
		StartListen ();
		StartCoroutine (CleanupExpiredEntries ());
	}

	public void StartBroadcast () {
		while (running) {
			base.StopBroadcast ();
			StopBroadcast ();
			Debug.Log ("Its already running(B)");
		}
		if (!running) {
			Debug.Log ("gameName : " + gameName);
			RTSNetworkDiscovery.singleton.broadcastData = gameName;
			Initialize ();
			StartAsServer ();
		}
	}

	public void StartListen () {
		if (isServer) {
			while (running) {
				base.StopBroadcast ();
				StopBroadcast ();
				Debug.Log ("Its already running(L)");
			}
			if (!running) {
				Initialize ();
				StartAsClient ();
			}
		} else {
			Initialize ();
			StartAsClient ();
		}
	}
	private IEnumerator CleanupExpiredEntries () {
		while (true) {
			bool changed = false;
			//var keys = lanAddresses;
			List <LanConnectionInfo> tempLan = new List<LanConnectionInfo>(lanAddresses);

			foreach (var key in tempLan) {
				if (key.timeout <= Time.time) {
					lanAddresses.Remove (key);
					changed = true;
					Debug.Log ("Removed : "+ changed);

				}
			}
			if (changed){
				Debug.Log ("changed : "+ changed);
				UpdateMatchInfos ();
			}

			yield return new WaitForSeconds (timeout);
		}
	}

	public override void OnReceivedBroadcast (string fromAddress, string data) {

		base.OnReceivedBroadcast (fromAddress, data);

		LanConnectionInfo info = new LanConnectionInfo (fromAddress, data);

		Debug.Log ("Current " + info.ipAddress);

		bool alreadyExists = false;
		foreach (LanConnectionInfo i in lanAddresses) {
			if (i.ipAddress == info.ipAddress) {
				alreadyExists = true;

				Debug.Log ("Already Exists : " + info.ipAddress);
				if (i == info) i.timeout = Time.time + timeout;

			}
		}
		if (!alreadyExists) {
			Debug.Log ("Adding to Dictionary : " + info.ipAddress);
			info.timeout = Time.time + timeout;
			lanAddresses.Add (info);
			UpdateMatchInfos ();
		} 
	}

	private void UpdateMatchInfos () {
		//update UI to add/remove available games
		Debug.Log (lanAddresses.Count);
		Debug.Log ("List updated!");
		UpdateUI ();
	}

	#region fix1	
	void Awake () {
		if (singleton != null && singleton != this)
			this.enabled = false;
		else
			singleton = this;
	}

	public new void StopBroadcast () {
		if (running)
			base.StopBroadcast ();
		ConfirmStopped ();
	}

	void LateUpdate () {
		if (!running && !stopConfirmed)
			ConfirmStopped ();
	}

	void ConfirmStopped () {
		try {
			stopConfirmed = !NetworkTransport.IsBroadcastDiscoveryRunning ();
		} catch (Exception) {
			stopConfirmed = true;
		}
	}
	#endregion

	void UpdateUI () {
		Debug.Log("updates");
		LobbyManager LM = GetComponent<LobbyManager> ();
		Transform RoomPanel = LM.roomUIPanel.transform;

		ClearChildren(RoomPanel);

		foreach (LanConnectionInfo l in lanAddresses) {
			GameObject Room = LM.roomUIPrefab;
			Room.GetComponent<RoomUI> ().ipAddress = l.ipAddress;
			Room.GetComponent<RoomUI> ().gameName = l.rawData[0];

			Room = Instantiate (Room, RoomPanel, false);
			Room.GetComponent<Button>().onClick.AddListener(delegate {
				LM.CtrStartClient();
				//LM.toggleMenu();
				});
			//Debug.Log(lanAddresses.Key.rawData[2]);
			//Debug.Log(lanAddresses.Key.ipAddress);
		}
	}

	public void ClearChildren (Transform panel) {
		foreach(Transform t in panel){
			Destroy(t.gameObject);
		}
		/* 		var gameObjects = GameObject.FindGameObjectsWithTag ("RoomUI");
				
				for(var i = 0 ; i < gameObjects.Length ; i ++)
				{
					Destroy(gameObjects[i]);
				} */
	}

	public void SetGameNameToLocal (TMP_InputField gameNameUI) {
		string gameNameNew = gameNameUI.text; // here "TextMeshProText" is 'TMP_InputField'
		Debug.Log ("New Game Name : " + gameNameNew);
		if (!string.IsNullOrEmpty (gameNameNew)) {
			gameName = gameNameNew;
		} else {
			gameName = "local";
		}
	}

}