using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkUI : MonoBehaviour {

	public void StopGame(){
		NetworkManager.singleton.StopHost();
	}
}
