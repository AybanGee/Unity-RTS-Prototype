using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SP_spawnholder : MonoBehaviour {

	public static SP_spawnholder singleton;

	public GameObject[] baseLocations = new GameObject[4];
	public GameObject[] supplyLocations = new GameObject[2];
	public GameObject[] barracksLocations = new GameObject[1];
	public GameObject[] towerLocations = new GameObject[4];

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
