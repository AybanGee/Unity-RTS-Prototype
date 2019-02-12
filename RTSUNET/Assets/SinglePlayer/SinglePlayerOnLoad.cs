using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinglePlayerOnLoad : MonoBehaviour {

	public LobbyManager LM;
	public LobbyPlayer LP;
	public GameObject loadingScreen;
	// Use this for initialization
	void Start () {

		LM = (LobbyManager) LobbyManager.singleton;
		LM.isSinglePlayer = true;
		LM.StopHost ();
		LM.transform.GetChild (0).gameObject.SetActive (true);
		LM.RoomScreen.SetActive (true);
		StartCoroutine (StartHostDelay ());
		//LM.CtrStartHost();
		//StartCoroutine (StartAsHost());

	}
	IEnumerator StartHostDelay () {
		yield return new WaitForSeconds (3);
		LM.CtrStartHost ();
		//LM.CtrStartClient ("localhost");

		yield return new WaitForSeconds (1);
		LM.transform.GetChild (0).gameObject.SetActive (true);
		LM.playerUIiPanel.SetActive (true);

		loadingScreen.SetActive (false);
		for (int i = 0; i < LM.lobbySlots.Length; i++) {
			if (LM.lobbySlots[i] != null) {
				LP = (LobbyPlayer) LM.lobbySlots[i];
			}
		}

		yield return null;
	}

	public void SetFaction (int faction) {
		Debug.Log ("Setting Faction to : " + faction);
		if (faction == 0) {
			LP.CmdSelectFaction (faction);
			LM.mapName = "sp_loop";
		} else if (faction == 1) {
			LP.CmdSelectFaction (faction);
			LM.mapName = "sp_loop";
		}
		LP.OnClickReady ();
	}

}