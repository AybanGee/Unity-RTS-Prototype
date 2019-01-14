using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MoveToOtherScene : MonoBehaviour {
	
	float speed;
	// Use this for initialization
	void Awake () {
		SceneManager.MoveGameObjectToScene(  gameObject,SceneManager.GetActiveScene());
/* 		speed = 5f;
		LateStart(0); */
	}
	
	IEnumerator LateStart(float waitTime){
		yield return new WaitForSeconds(waitTime);

		Scene s = SceneManager.GetActiveScene();

		Debug.Log("Scene Name : "+s.name);
		//MoveScene
		SceneManager.MoveGameObjectToScene(  gameObject,SceneManager.GetActiveScene());

	}
}
