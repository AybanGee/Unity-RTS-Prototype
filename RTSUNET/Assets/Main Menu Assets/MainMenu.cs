using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

	public void EnterSinglePlayer () {
		if (LobbyManager.singleton != null) {
			Destroy (LobbyManager.singleton.gameObject);
			Debug.Log ("Lobby Exists");
		}
		SceneManager.LoadScene("CampaignSelect");
	}
	public void EnterLobby () {
		if (LobbyManager.singleton != null) {
			Destroy (LobbyManager.singleton.gameObject);
			Debug.Log ("Lobby Exists");
		}
		SceneManager.LoadScene ("Lobby");
	}
	public void QuitGame () {
		Debug.Log ("Quiting Game");
		Application.Quit ();
	}
	void Start () {
		if (LobbyManager.singleton != null) {
			Destroy (LobbyManager.singleton.gameObject);
			Debug.Log ("Lobby Exists (STart)");
		}
	}
}