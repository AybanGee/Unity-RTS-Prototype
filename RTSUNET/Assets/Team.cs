using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class Team :NetworkBehaviour {

	public List<Color> teamColors;


List<PlayerObject> playerObjects;
 	[SyncVar]
	public int playerCount = 0;
	// Use this for initialization
	void Start () {
		
	}
	void Awake(){
		playerCount++;
		Debug.Log("Player Count " +  playerCount);
	}
	// Update is called once per frame
	void Update () {
		
	} 



}
