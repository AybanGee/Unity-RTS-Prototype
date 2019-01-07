using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System;
 
 [RequireComponent(typeof(RTSNetworkDiscovery))]
public class LobbyManager : NetworkLobbyManager {

	public GameObject playerUiPrefab;
	public GameObject playerUIiPanel;
	public GameObject roomUIPanel;
	public GameObject roomUIPrefab;
	public GameObject playerCanvas;
	public GameObject LobbyScreen;
	public GameObject RoomScreen;



	public GameColorsScriptable gameColors;
	public string mapName;
	public string gameName;


	#region "Host & Client Controls"
	public void CtrStartHost () {
		StartHost ();
	}
	public void CtrStartClient () {
		StartClient ();
		toggleMenu();
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
	public override bool OnLobbyServerSceneLoadedForPlayer (GameObject lobbyPlayer, GameObject gamePlayer) {

		LobbyPlayer lb = lobbyPlayer.GetComponent<LobbyPlayer> ();
		PlayerObject po = gamePlayer.GetComponent<PlayerObject> ();
		po.playerName = lb.P_name;
		po.team = lb.team;
		po.factionIndex = lb.faction;
		po.colorIndex = lb.colorIndex;
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

		 public static void StopClientAndBroadcast()
        {
            RTSNetworkDiscovery.singleton.StopBroadcast();
            onBroadcastStopped += singleton.StopClient;
        }
 
        public static void StopServerAndBroadcast()
        {
            RTSNetworkDiscovery.singleton.StopBroadcast();
            onBroadcastStopped += singleton.StopServer;
        }
 
        public static void StopHostAndBroadcast()
        {
            RTSNetworkDiscovery.singleton.StopBroadcast();
            onBroadcastStopped += singleton.StopHost;
        }
 
        private static event Action onBroadcastStopped;
 
        void Update()
        {
            if (onBroadcastStopped != null) {
                if (!RTSNetworkDiscovery.singleton.running && RTSNetworkDiscovery.stopConfirmed) {
                    onBroadcastStopped.Invoke();
                    onBroadcastStopped = null;
                } else {
                    if (LogFilter.logDebug)
                        Debug.Log("Waiting for broadcasting to stop completely", gameObject);
                    RTSNetworkDiscovery.singleton.StopBroadcast();
                }
            }
        }
#endregion
	public void SetAddress( TextMeshProUGUI TextPro){
		networkAddress = TextPro.text;
	}

	public void toggleMenu(){
		if(RoomScreen.activeSelf){
			RoomScreen.SetActive(false);
			LobbyScreen.SetActive(true);
		}
		else{
			RoomScreen.SetActive(true);
			LobbyScreen.SetActive(false);
		}
		
/* 			RoomScreen.SetActive(!RoomScreen.activeSelf);
			LobbyScreen.SetActive(!LobbyScreen.activeSelf);

 */
	}
	
}