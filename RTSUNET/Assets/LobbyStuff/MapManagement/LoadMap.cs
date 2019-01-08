using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadMap : MonoBehaviour {
	public string mapSceneName;
	
	public static LoadMap Instance{set;get;}

	void Awake () {
		Instance = this;
		
	LobbyManager LM = LobbyManager.singleton.GetComponent<LobbyManager>();
		mapSceneName = LM.mapName;
		Load(mapSceneName);
	}
	
	void Load (string sceneName) 
	{
		if(!SceneManager.GetSceneByName(sceneName).isLoaded)
			SceneManager.LoadScene(sceneName,LoadSceneMode.Additive);
	}

	void Unload (string sceneName) 
	{
		if(SceneManager.GetSceneByName(sceneName).isLoaded)
			SceneManager.UnloadScene(sceneName);
	}
}
