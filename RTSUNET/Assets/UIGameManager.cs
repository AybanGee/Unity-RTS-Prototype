using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIGameManager : MonoBehaviour {
	public DragSelectionHandler dragSelectionHandler;
	public UIGameCommandsHandler commandsHandler;
	public TextMeshProUGUI manaHolder, victoryDisplay, endTextDisplay;
	public GameObject winScreen,spectateScreen,notice;

	public Text debugTxt;

	public static UIGameManager singleton;
	void Awake () {
		if (singleton == null)
			singleton = this;

		//commandsHandler.commandsPanel =
	}

	public void Initialize (PlayerObject po) {
		if (dragSelectionHandler != null) dragSelectionHandler.AssignPlayerObject (po);
		else Debug.LogError ("Cannot find drag selector handler");

		if (commandsHandler != null) commandsHandler.Initialize (po);
		else Debug.LogError ("Cannot find commandsHandler");
	}

	public void ShowWinScreen (bool isWinner) {

		if (isWinner == true) {
			victoryDisplay.text = "Victory";
		} else {
			victoryDisplay.text = "Defeat";
		}

		winScreen.SetActive (true);
	}

	public void ShowSpectatorScreen () {
		spectateScreen.SetActive (true);	
	}
}