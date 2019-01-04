using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

	public void EnterLobby(){
		SceneManager.LoadScene("Lobby");
	}


	public void QuitGame(){
		Debug.Log("Quiting Game");
		Application.Quit();
	}

}
