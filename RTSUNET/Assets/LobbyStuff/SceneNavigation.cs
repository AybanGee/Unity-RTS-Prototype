using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneNavigation : MonoBehaviour {

	public void GoToMainMenu () {
		Load("Main Menu");
		
	}
	
	void Load (string sceneName) 
	{
		if(!SceneManager.GetSceneByName(sceneName).isLoaded)
			SceneManager.LoadScene(sceneName);
	}

	void Unload (string sceneName) 
	{
		if(SceneManager.GetSceneByName(sceneName).isLoaded)
			SceneManager.UnloadScene(sceneName);
	}


}
