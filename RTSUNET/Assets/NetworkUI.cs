using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class NetworkUI : MonoBehaviour {

	public void StopGame(){
		NetworkManager.singleton.StopHost();
		SceneManager.LoadScene("Main Menu");
	}
}
