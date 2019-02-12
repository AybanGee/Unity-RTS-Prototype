using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MapSelection : NetworkBehaviour {

	public MapDictionary mapsList;
	Dropdown m_Dropdown;
	public GameObject mapDisplay;
	public GameObject description;
	public string sceneName;
	private bool useAble = true;

	void Awake () {
		m_Dropdown = GetComponent<Dropdown> ();
		LoadMaps ();
		ChangeDisplay ();
	}

	public void ToggleMapSelect (bool isAllowed) {
		GetComponent<CanvasGroup> ().interactable = isAllowed;
	}

	void LoadMaps () {
		m_Dropdown.ClearOptions ();
		List<string> mapNames = new List<string> ();
		foreach (Map map in mapsList.Maps) {
			mapNames.Add (map.mapName);
		}
		//Clear the old options of the Dropdown menu
		m_Dropdown.ClearOptions ();
		//Add the options created in the List above
		m_Dropdown.AddOptions (mapNames);
	}

	public void ChangeDisplay () {
		Map m = mapsList.Maps[m_Dropdown.value];
		mapDisplay.GetComponent<Image> ().sprite = m.displayImage;
		description.GetComponent<TextMeshProUGUI> ().text = m.description;

	}

	public void SetMapToLoad (int dpval) {
		Map m = mapsList.Maps[dpval];
		NetworkLobbyManager.singleton.GetComponent<LobbyManager> ().mapName = m.sceneName;

		Debug.Log("Map Selection :: Map Name : " + m.mapName);
		Debug.Log("Map Selection :: Scene Name : " + m.sceneName);
	}


}