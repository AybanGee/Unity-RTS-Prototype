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



	void Awake () {
		Instance = this;

		//commented for testing
		//LobbyManager LM = LobbyManager.singleton.GetComponent<LobbyManager>();
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

		StartCoroutine(FindingBaseLocation());
	}
	IEnumerator FindingBaseLocation () {


		if (BaseHolder.singleton != null) {
			Debug.Log ("baseholder exists");
		} else
			Debug.Log ("baseholder does not exists");

//		SceneManager.SetActiveScene(SceneManager.GetSceneByName(mapSceneName));
		
		while (baseholder == null) {
			if(BaseHolder.singleton != null)
			baseholder = BaseHolder.singleton;
			Debug.Log ("while is running");

			yield return null;
		}

		if (baseholder != null) {
			Debug.Log ("base found");
			//move Camera to location
			//LateStart(1);
			moveCamToBase();

		} else
			Debug.Log ("base is not found");
		//Invoke spawning here
		
		yield return null;
	}

	void moveCamToBase(){
		//di ko alam kung pano ipapasa yung base number
		PlayerObject PO = PlayerObject.singleton;
		//error dito
		//Vector3 baseLoc = baseholder.baseLocations[PO.baseNo].transform.position;
		GameObject baseLoc = baseholder.baseLocations[PO.baseNo];
		camGroup.transform.position =  new Vector3(baseLoc.transform.position.x, 0 , baseLoc.transform.position.x);

		//maybe spawn Town Center here?
		int buildingIndex = 2;
       	PO.BuildSys.SpawnBuilding(buildingIndex,baseLoc.transform.position,baseLoc.transform.rotation);

	}


}