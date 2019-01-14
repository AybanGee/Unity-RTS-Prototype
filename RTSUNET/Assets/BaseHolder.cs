using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class BaseHolder : MonoBehaviour {

	public static BaseHolder singleton;

	public GameObject[] baseLocations = new GameObject[4];
	// Use this for initialization
	void Awake()
	{
		 if (singleton != null && singleton != this)	
		 	{	
			//singleton = null;
                this.enabled = false;
			}
			else
                singleton = this;	
	}
	
	// Update is called once per frame
	void Update () {

	}
}
