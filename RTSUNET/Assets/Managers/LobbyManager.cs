using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[RequireComponent (typeof (RTSNetworkDiscovery))]
public class LobbyManager : NetworkLobbyManager {

	public GameObject playerUiPrefab;
	public GameObject playerUIiPanel;
	//room discovery
	public GameObject roomUIPanel;
	public GameObject roomUIPrefab;
	//
	public GameObject playerCanvas;
	//screen swapping
	public GameObject LobbyScreen;
	public GameObject RoomScreen;

	//map selection
	public MapSelection mapDropdown;

	public GameColorsScriptable gameColors;
	//[HideInInspector()]
	//[SyncVar]
	public string mapName, gameName;
	public TextMeshProUGUI roomInfo;
	public TextMeshProUGUI statusTxt;

	//check if all players are ready
	//[SyncVar]
	public int readyCount;
	public int numOfPlayers;
	public bool allIsReady = false;
	public List<LobbyPlayer> lobbyPlayers = new List<LobbyPlayer> ();

	public bool isSinglePlayer = false;


	#region "Host & Client Controls"
	public void CtrStartHost () {
		StartHost ();
		InitializeRoom ("host");
		mapDropdown.ToggleMapSelect (true);
	}
	public void CtrStartClient (string ipAddress) {
		Debug.Log ("ipAddress " + ipAddress);
		networkAddress = ipAddress;
		Debug.Log ("networkAddress " + networkAddress);
		StartClient ();
		InitializeRoom ("client");
		mapDropdown.ToggleMapSelect (false);

	}

	public void InitializeRoom (string playerAuth) {

		roomInfo.text = "Room:'" + gameName + "' (" + playerAuth + ")";
		//update here on the current situation 
	}
	#endregion
	public override void OnStartHost () {
		print ("Host started");
		base.OnStartHost ();
	}
	public override void OnLobbyStartClient (NetworkClient lobbyClient) {
		Debug.Log ("Client Started");
		base.OnLobbyStartClient (lobbyClient);

	}
	public override void OnServerAddPlayer (NetworkConnection conn, short playerControllerId) {
		base.OnServerAddPlayer (conn, playerControllerId);
		Debug.Log ("OnServerAddPlayer");

		//	GameObject ui = Instantiate(playerUiPrefab);
		///	GameObject txt = ui.GetComponent<ComponentHandler>().components[0].componentObject;
		//	txt.GetComponent<TextMeshProUGUI>().text = "Player" + playerControllerId;
		///	ui.transform.SetParent(playerUIiPanel.transform);	
	} ///!

	public override void OnLobbyClientSceneChanged (NetworkConnection conn) {
		Debug.Log ("Scene Changed");

		playerUIiPanel.SetActive (false);
		playerCanvas.SetActive (false);

		base.OnLobbyClientSceneChanged (conn);

		//	DragSelectionHandler.singleton.StartforClient();

	}

	public override void OnLobbyStopClient () {
		Debug.Log ("OnLobbyStopClient");
		playerUIiPanel.SetActive (true);
		playerCanvas.SetActive (true);
		toggleMenu ();
		base.OnLobbyStopClient ();
	}

	public override void OnLobbyStopHost () {
		if (isSinglePlayer)
			Debug.Log ("OnLobbyStopHost");
		playerUIiPanel.SetActive (true);
		playerCanvas.SetActive (true);
		base.OnLobbyStopHost ();
	}

	//All players ready
	public override void OnLobbyServerPlayersReady () {
		Debug.Log ("All ready");
		//disables the 
		playerUIiPanel.GetComponent<CanvasGroup> ().interactable = false;

		if (!IsAllPlayersValid ()) {
			playerUIiPanel.GetComponent<CanvasGroup> ().interactable = true;
			return;
		}
		base.OnLobbyServerPlayersReady ();
	}

	public override bool OnLobbyServerSceneLoadedForPlayer (GameObject lobbyPlayer, GameObject gamePlayer) {

		LobbyPlayer lb = lobbyPlayer.GetComponent<LobbyPlayer> ();
		PlayerObject po = gamePlayer.GetComponent<PlayerObject> ();
		po.playerName = lb.P_name;
		po.team = lb.team;
		po.factionIndex = lb.faction;
		po.colorIndex = lb.colorIndex;

		if(isSinglePlayer)
		po.isSinglelPlayer = true;

		Debug.LogWarning (lb.baseNo);
		po.baseNo = lb.baseNo;
		po.playerId = (int) lobbyPlayer.GetComponent<NetworkIdentity> ().netId.Value;
		Debug.Log ("Transition method of team" + lb.team);
		bool value = base.OnLobbyServerSceneLoadedForPlayer (lobbyPlayer, gamePlayer);
		return value;
	}

	public override void OnClientDisconnect (NetworkConnection conn) {
		base.OnClientDisconnect (conn);
		Debug.Log ("Disconected client");
	}

	#region netDiscoveryfix1
	public static void StopClientAndBroadcast () {
		RTSNetworkDiscovery.singleton.StopBroadcast ();
		onBroadcastStopped += singleton.StopClient;
	}

	public static void StopServerAndBroadcast () {
		RTSNetworkDiscovery.singleton.StopBroadcast ();
		onBroadcastStopped += singleton.StopServer;
	}

	public static void StopHostAndBroadcast () {
		RTSNetworkDiscovery.singleton.StopBroadcast ();
		onBroadcastStopped += singleton.StopHost;
	}

	private static event Action onBroadcastStopped;

	void Update () {
		if (onBroadcastStopped != null) {
			if (!RTSNetworkDiscovery.singleton.running && RTSNetworkDiscovery.stopConfirmed) {
				onBroadcastStopped.Invoke ();
				onBroadcastStopped = null;
			} else {
				if (LogFilter.logDebug)
					Debug.Log ("Waiting for broadcasting to stop completely", gameObject);
				RTSNetworkDiscovery.singleton.StopBroadcast ();
			}
		}
	}
	#endregion
	public void SetAddress (TextMeshProUGUI TextPro) {
		networkAddress = TextPro.text;
	}

	public void toggleMenu () {
		if (RoomScreen.activeSelf) {
			RoomScreen.SetActive (false);
			LobbyScreen.SetActive (true);
		} else {
			RoomScreen.SetActive (true);
			LobbyScreen.SetActive (false);
		}

		/* 			RoomScreen.SetActive(!RoomScreen.activeSelf);
					LobbyScreen.SetActive(!LobbyScreen.activeSelf);

		 */
	}

	#region Player Preparation
	bool IsAllPlayersValid () {
		//prepare list of players
		lobbyPlayers.Clear ();

		foreach (NetworkLobbyPlayer nlp in lobbySlots) {
			if (nlp != null)
				lobbyPlayers.Add ((LobbyPlayer) nlp);
		}
		// if (lobbyPlayers.Count <= 0) {
		// 	ResetAllAsNotReady (lobbyPlayers);
		// 	ShowStatus ("Cannot Start! There are no players.", 5, Color.red);
		// 	return false;
		// }
		// if (lobbyPlayers.Count == 1) {
		// 	ResetAllAsNotReady (lobbyPlayers);
		// 	ShowStatus ("Cannot Start! At least 2 Players are required", 5, Color.red);
		// 	return false;
		// }
		// //check if all are same team
		// int sameCount = 0;
		// for (int i = 0; i < lobbyPlayers.Count - 1; i++) {
		// 	if (lobbyPlayers[i].team == lobbyPlayers[i + 1].team) sameCount++;
		// }
		// if (sameCount == lobbyPlayers.Count - 1) {
		// 	ResetAllAsNotReady (lobbyPlayers);
		// 	ShowStatus ("Cannot Start! Cannot have every one on one team", 5, Color.red);
		// 	return false;
		// }

		foreach (LobbyPlayer lp in lobbyPlayers) {
			if (lp.broadcaster != null)
				StopCoroutine (lp.broadcaster);
		}
		AssignPlayerBases (lobbyPlayers);
		return true;
	}
	void ResetAllAsNotReady (List<LobbyPlayer> lobbyPlayers) {
		foreach (LobbyPlayer lp in lobbyPlayers) {
			lp.OnClickReady ();
		}
	}

	void AssignPlayerBases (List<LobbyPlayer> lp) {
		//readyCount = 0;
		numOfPlayers = lp.Count;
		List<LobbyPlayer> newLp = lp.OrderBy (o => o.team).ToList ();
		if (newLp.Count > 2) {
			for (int i = 0; i < newLp.Count; i++) {
				newLp[i].OnChangeBase (i);
			}
		} else {
			for (int i = 0; i < newLp.Count; i++) {
				newLp[i].OnChangeBase (i + 1);
			}
		}

		foreach (LobbyPlayer _lp in lp) {
			_lp.LM.SetMapName (mapName);
		}
	}
	#endregion

	#region Status
	Coroutine statCoroutine;
	void ShowStatus (string message, float duration, Color color) {
		statusTxt.text = message;
		if (statCoroutine != null) StopCoroutine (statCoroutine);
		statCoroutine = StartCoroutine (StatDuration (duration, color));
	}
	IEnumerator StatDuration (float duration, Color color) {
		//show stat
		statusTxt.color = color;
		yield return new WaitForSeconds (duration);
		//fadeout
		Color tempColor = statusTxt.color;
		while (tempColor.a > 0) {
			tempColor.a -= 0.005f;
			statusTxt.color = tempColor;
			yield return null;

		}
		statusTxt.text = String.Empty;
		yield return null;
	}
	#endregion

	public void SetMapName (string input) {
		mapName = input;
	}

}