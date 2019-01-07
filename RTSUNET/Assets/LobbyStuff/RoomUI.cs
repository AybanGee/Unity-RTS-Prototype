using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RoomUI : MonoBehaviour {

	public string ipAddress,gameName;
	public int numOfPlayers;	

	public TextMeshProUGUI gameNameUI,ipAddressUI,numOfPlayersUI;
	void Start () {
		//patulong na lang kay kuya sa pag spawn ng prefab
		//naka array na yung data na ilalagay sa netDiscovery
		gameNameUI.text = gameName;
		ipAddressUI.text = ipAddress;
		//numOfPlayersUI.text = numOfPlayers.ToString();
	    //this.transform.SetParent(LobbyManager.singleton.GetComponent<LobbyManager>().roomUIPanel.transform,false);
	}

}
