using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using System;
using TMPro;


public class RTSNetworkDiscovery1 : NetworkDiscovery {
	public static string gameName = "local";
	public static int target = 0;
	public static RTSNetworkDiscovery1 singleton;
	private float timeout = 5f;
	public static bool stopConfirmed = false;
	private Dictionary<LanConnectionInfo,float> lanAddresses = new Dictionary<LanConnectionInfo,float>();

	private void Start() {
		StartListen();
		StartCoroutine(CleanupExpiredEntries());
	}

	public void StartBroadcast(){
		while(running){
			base.StopBroadcast();
			StopBroadcast();
			Debug.Log("Its already running(B)");
		}
		if(!running){
			Debug.Log("gameName : " + gameName);
			RTSNetworkDiscovery1.singleton.broadcastData += ":" + gameName;
			Initialize();
			StartAsServer();
		}
	}

	public void StartListen(){
		if(isServer){
			while(running){
				base.StopBroadcast();
				StopBroadcast();
				Debug.Log("Its already running(L)");
			}
			if(!running){
				Initialize();
				StartAsClient();
			}
		}
		else{
				Initialize();
				StartAsClient();
			}
	}
	private IEnumerator CleanupExpiredEntries()
	{
		while(true){
			bool changed = false;
			var keys = lanAddresses.Keys.ToList();

			foreach (var key in keys)
			{
				if(lanAddresses[key] <= Time.time){
					lanAddresses.Remove(key);
					changed = true;
				}
			}
			if(changed)
				UpdateMatchInfos();

			yield return new WaitForSeconds(timeout);
		}
	}

	public override void OnReceivedBroadcast(string fromAddress, string data){

		base.OnReceivedBroadcast(fromAddress,data);

		LanConnectionInfo info = new LanConnectionInfo(fromAddress,data);

		Debug.Log("Current " + info.ipAddress);

		#region checker
		bool alreadyExists = false;
		
		if(lanAddresses.Count == 0 || lanAddresses == null){
			Debug.Log("Adding to Dictionary : "+info.ipAddress);

			lanAddresses.Add(info, Time.time + timeout);
			UpdateMatchInfos();
			target ++;
		}

		Debug.Log("List count:"+lanAddresses.Count);
		Debug.Log("target count:"+lanAddresses.Count);

		foreach(KeyValuePair<LanConnectionInfo,float> lanAddresseschecker in lanAddresses)
			{
				if(lanAddresseschecker.Key.ipAddress == info.ipAddress){
					alreadyExists = true;	
				}
			}
			
			if(alreadyExists){
				Debug.Log("Already in Dictionary : "+info.ipAddress);
				lanAddresses[info] = Time.time + timeout;
			}
			else{
				Debug.Log("Adding to Dictionary : "+info.ipAddress);

				lanAddresses.Add(info, Time.time + timeout);
				UpdateMatchInfos();
				target++;
			}
		#endregion
/* 
		if(lanAddresses.ContainsKey(info) == false){
			Debug.Log("Adding to Dictionary : "+info.ipAddress);

			lanAddresses.Add(info, Time.time + timeout);
			UpdateMatchInfos();
		}
		else{
			Debug.Log("Already in Dictionary : "+info.ipAddress);
			lanAddresses[info] = Time.time + timeout;
		}		 */
	}
	private void UpdateMatchInfos(){
		//update UI to add/remove available games
		Debug.Log(lanAddresses.Count);
		Debug.Log("List updated!");
		UpdateUI();
	}

	#region fix1	
		void Awake()
        {
            if (singleton != null && singleton != this)
                this.enabled = false;
            else
                singleton = this;
        }
 
        public new void StopBroadcast()
        {
            if (running)
                base.StopBroadcast();
            ConfirmStopped();
        }
 
        void LateUpdate()
        {	
            if (!running && !stopConfirmed)
                ConfirmStopped();
        }
 
        void ConfirmStopped()
        {
            try {
                stopConfirmed = !NetworkTransport.IsBroadcastDiscoveryRunning();
            } catch (Exception) {
                stopConfirmed = true;
            }
        }
	#endregion

	void UpdateUI(){
		LobbyManager LM = GetComponent<LobbyManager>();
		GameObject RoomPanel = LM.roomUIPanel;

		//ClearChildren(RoomPanel);

		foreach(KeyValuePair<LanConnectionInfo,float> lanAddresses in lanAddresses)
		{
			GameObject Room = LM.roomUIPrefab;
			Room.GetComponent<RoomUI>().ipAddress = lanAddresses.Key.ipAddress;
			Room.GetComponent<RoomUI>().gameName = lanAddresses.Key.rawData[3];

			 Instantiate(Room,RoomPanel.transform, false);
				//Debug.Log(lanAddresses.Key.rawData[2]);
				//Debug.Log(lanAddresses.Key.ipAddress);
		}
	}


	public void ClearChildren(GameObject panel)
	{
/* 		var gameObjects = GameObject.FindGameObjectsWithTag ("RoomUI");
		
		for(var i = 0 ; i < gameObjects.Length ; i ++)
		{
			Destroy(gameObjects[i]);
		} */
	}

	public void SetGameNameToLocal(TMP_InputField gameNameUI){
		string gameNameNew = gameNameUI.text;  // here "TextMeshProText" is 'TMP_InputField'
		Debug.Log("New Game Name : " + gameNameNew);
		if (!string.IsNullOrEmpty(gameNameNew))
		{
			gameName = gameNameNew;
		}
		else
		{
			gameName = "local";
		}  
	}


}
