using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;

public class LobbyManager : NetworkLobbyManager {

	public GameObject playerUiPrefab;
	public GameObject playerUIiPanel;

	public GameObject playerCanvas;
	public  override void OnStartHost(){
		print("Host started");
		base.OnStartHost();

	
	}
	public override void OnLobbyStartClient(NetworkClient lobbyClient){
		Debug.Log("Client Started");
		base.OnLobbyStartClient(lobbyClient);
		
	}
	public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId){
		base.OnServerAddPlayer(conn,playerControllerId);
		Debug.Log("OnServerAddPlayer");
		//	GameObject ui = Instantiate(playerUiPrefab);
		///	GameObject txt = ui.GetComponent<ComponentHandler>().components[0].componentObject;
		//	txt.GetComponent<TextMeshProUGUI>().text = "Player" + playerControllerId;
		///	ui.transform.SetParent(playerUIiPanel.transform);	
	}///!

	public override void OnLobbyClientSceneChanged(NetworkConnection conn){
		Debug.Log("Scene Changed");
		
		playerUIiPanel.SetActive(false);
		playerCanvas.SetActive(false);
		
		base.OnLobbyClientSceneChanged(conn);

	//	DragSelectionHandler.singleton.StartforClient();
		
			}
	public override bool OnLobbyServerSceneLoadedForPlayer(GameObject lobbyPlayer,GameObject gamePlayer){
	
		LobbyPlayer lb = lobbyPlayer.GetComponent<LobbyPlayer>();
		PlayerObject po = gamePlayer.GetComponent<PlayerObject>();
		po.playerName = lb.P_name;
		po.team = lb.team;
		po.faction = lb.faction;
		Debug.Log("Transition method of team" + lb.team);
		bool value = base.OnLobbyServerSceneLoadedForPlayer(lobbyPlayer,gamePlayer);
		return value;
	}



	public override void OnClientDisconnect(NetworkConnection conn){
		base.OnClientDisconnect(conn);
		Debug.Log("Disconected client");
	}

	public override void OnStopClient(){
		base.OnStopClient();
			playerUIiPanel.SetActive(true);
			playerCanvas.SetActive(true);
		Debug.Log("Stopped Client");
	}

}
