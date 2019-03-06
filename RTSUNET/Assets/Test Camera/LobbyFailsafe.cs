using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyFailsafe : MonoBehaviour {

	public GameObject LM;
	public GameObject clientButton;
	bool buttonActive = false;

	// Update is called once per frame
	void Start(){
		LM = LobbyManager.singleton.gameObject;
	}

	void Update () {
		if(Input.GetKeyDown(KeyCode.I)){
			LM.gameObject.SetActive(true);
		}

		if (Input.GetKeyDown (KeyCode.P) && LM != null) {
			LM.SetActive(true);
			LM.GetComponent<LobbyManager> ().StopHost ();
			LM.GetComponent<RTSNetworkDiscovery> ().StopBroadcast ();
			LM.GetComponent<SceneNavigation> ().GoToMainMenu ();
		}
		if (Input.GetKeyDown (KeyCode.O)) {
			buttonActive = !buttonActive; 
			clientButton.SetActive(buttonActive);
		}
	}
}