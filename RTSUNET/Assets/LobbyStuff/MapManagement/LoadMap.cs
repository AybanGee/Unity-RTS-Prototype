using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadMap : MonoBehaviour {
	public string mapSceneName;

	public int baseNum;

	public static LoadMap Instance { set; get; }

	public GameObject camGroup;

	BaseHolder baseholder;
	LobbyManager LM;

	public static LoadMap singleton;
	public bool isFinishedLoading = false;

	void Awake () {
		if (singleton != null && singleton != this)
			this.enabled = false;
		else
			singleton = this;
		//Instance  = this;

		//commented for testing
		LM = LobbyManager.singleton.GetComponent<LobbyManager> ();
		//mapSceneName = LM.mapName;

		Load (mapSceneName);

	}

	void Load (string sceneName) {
		if (!SceneManager.GetSceneByName (sceneName).isLoaded) {
			SceneManager.LoadScene (sceneName, LoadSceneMode.Additive);
			SceneManager.sceneLoaded += OnMapFinishedLoading;

		}
	}

	void Unload (string sceneName) {
		if (SceneManager.GetSceneByName (sceneName).isLoaded)
			SceneManager.UnloadScene (sceneName);
	}

	void OnMapFinishedLoading (Scene scene, LoadSceneMode mode) {
		Debug.Log ("Level loaded : " + scene.name + " Mode: " + mode);
		SceneManager.sceneLoaded -= OnMapFinishedLoading;
		//check if naka load na lahat
		StartCoroutine (FindingBaseLocation ());
	}
	IEnumerator FindingBaseLocation () {
		while (baseholder == null) {
			if (BaseHolder.singleton != null)
				baseholder = BaseHolder.singleton;
			Debug.Log ("while is running");
			yield return null;
		}

		if (baseholder != null) {
			Debug.Log ("base found");
			isFinishedLoading = true;
			//moveCamToBase();
		} else
			Debug.Log ("base is not found");
		//Invoke spawning here	

		yield return null;
	}

	IEnumerator WaitForOtherPlayers () {
		yield return null;
	}

	public void moveCamToBase () {
		//di ko alam kung pano ipapasa yung base number
		//PlayerObject PO = PlayerObject.singleton;
		//error dito
		//Vector3 baseLoc = baseholder.baseLocations[PO.baseNo].transform.position;
		Debug.Log ("Move cam to base");
		PlayerObject PO = PlayerObject.singleton;
		Debug.Log ("PO : " + PO);

		GameObject baseLoc = baseholder.baseLocations[PO.baseNo];

		Debug.Log ("baseLocation : " + baseLoc);

		camGroup.transform.position = new Vector3 (baseLoc.transform.position.x, 0, baseLoc.transform.position.x);

		//maybe spawn Town Center here?
		int buildingIndex = 3;
		Debug.Log ("Spawning Townhall");
		StartCoroutine (WaitForBuilding (PO));

		PO.BuildSys.SpawnBuilding (buildingIndex, baseLoc.transform.position, baseLoc.transform.rotation);

	}

	IEnumerator WaitForBuilding (PlayerObject PO) {
		Debug.Log ("Waiting for Building");
		while (PO.checkerWait == false) {

			while (PO.myBuildings.Count == 0) {
				Debug.Log ("Waiting for Building to Spawn");
				yield return null;
			}
			Debug.Log ("Adding building to holder : " + PO.myBuildings[0]);
			PO.townhall = PO.myBuildings[0];
			//PO.CmdSetChecker (true);
			bool flag = false;

			while (flag == false) {
				foreach (PlayerObject po in PO.players) {
					flag = true;
					if (po.checkerWait != true) {
						flag = false;
					}
					Debug.Log ("PO : " + po + "checker : " + po.checkerWait);
				}
				yield return null;
			}

			if (PO.isServer && flag) {
				//PO.StartGameLoop ();
				Debug.Log ("Starting Game Loop");
			}
			yield return null;
		}
	}

}