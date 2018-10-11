using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPlayerUI : MonoBehaviour {

	public TMP_Dropdown dropdownFactionUI;
	public Dropdown dropdownColorsUI;

	GameColorsScriptable gameColors;
	private void Start () {

		gameColors = LobbyManager.singleton.GetComponent<LobbyManager>().gameColors;
		if (dropdownFactionUI == null) return;
		populateFactionDropdown (dropdownFactionUI);
		if (dropdownColorsUI == null){Debug.LogError("Cannot Find Colors UI");return;} 
		if(gameColors == null){Debug.LogError("Cannot Find Colors");return;} 
		populatedropdownColors (dropdownColorsUI);
	}

	void populateFactionDropdown (TMP_Dropdown d) {
		string[] factionEnumsStr = Enum.GetNames (typeof (Factions));
		List<string> factionNames = new List<string> (factionEnumsStr);

		d.AddOptions (factionNames);

	}

	void populatedropdownColors (Dropdown d) {
		d.ClearOptions();
		List<Sprite> coloredSprites = new List<Sprite>();
		List<Dropdown.OptionData> colorItems = new List<Dropdown.OptionData>();
	
		foreach (KeyValuePair<string, Color> entry in gameColors.gameColors)
	{
		Texture2D tex = new Texture2D(1,1);
		tex.SetPixel(0,0,entry.Value);
		tex.Apply();
		
		Sprite sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0, 0));
			var colorOption = new Dropdown.OptionData(entry.Key,sprite);
			colorItems.Add(colorOption);
	}
	d.AddOptions(colorItems);

		

	}

}