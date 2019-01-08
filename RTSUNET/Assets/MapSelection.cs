using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class MapSelection : NetworkBehaviour {

	public MapDictionary mapsList;
	Dropdown m_Dropdown;
	public GameObject mapDisplay;
	public GameObject description;
	public string sceneName;
	private bool useAble = true;


	void Start(){
		LoadMaps();
		ChangeDisplay();
	}

	public void ToggleMapSelect(bool isAllowed){
		
		GetComponent<CanvasGroup>().interactable = isAllowed;
/* 		GetComponent<CanvasGroup>().interactable = true;
		 if(!islocalPlayer) GetComponent<CanvasGroup>().interactable = false; */
	}

	void LoadMaps(){
		List<string> mapNames = new List<string>();
		foreach(Map map in mapsList.Maps){
			mapNames.Add(map.mapName);
		}
		m_Dropdown = GetComponent<Dropdown>();
		//Clear the old options of the Dropdown menu
		m_Dropdown.ClearOptions();
		//Add the options created in the List above
        m_Dropdown.AddOptions(mapNames);
	}

	public void ChangeDisplay(){
		m_Dropdown = GetComponent<Dropdown>();
		Map m =  mapsList.Maps[m_Dropdown.value];

		mapDisplay.GetComponent<Image>().sprite = m.displayImage;
		description.GetComponent<TextMeshProUGUI>().text = m.description;
	}

	public void SetMapToLoad(){
		Map m =  mapsList.Maps[m_Dropdown.value];
		NetworkLobbyManager.singleton.GetComponent<LobbyManager>().mapName = m.sceneName;
	}

	[Command]
    public void CmdSelectMap(int mapIndex){
        // Set team of player on the server.
        RpcSelectMap(mapIndex);
    }

	[ClientRpc]
    public void RpcSelectMap(int mapIndex){
        // Set team of player on the server.
       transform.GetComponent<Dropdown>().value = mapIndex;
    }

	
}
