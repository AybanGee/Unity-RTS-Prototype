using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueueingSystem : MonoBehaviour {

	//queue for spawning units
	public List<PlayerUnit> spawnQueue = new List<PlayerUnit> ();
	//available units in this building
	public List<PlayerUnit> spawnableUnits = new List<PlayerUnit> ();
	//
	public Vector3 spawnPoint;
	public Vector3 rallyPoint;
	public GameObject spawnPointHolder;
	public PlayerObject PO;
	public float creationTimeHolder;
	bool queueIsRunning;

	void Start () {
		//Set Spawn Point and Rally Point
		gameObject.transform.GetChild(1);
		//SetSpawnPoint(spawnPointHolder.transform.position);
		SetSpawnPoint(transform.position);
		queueIsRunning = false;

	}

	 IEnumerator ProcessQueue () {
		bool isDone = false;
	 	while (spawnQueue.Count > 0) {
			queueIsRunning = true;
			PlayerUnit u = spawnQueue[0];
			//if(isDone)			
			creationTimeHolder = u.creationTime;

			Debug.Log("CreationHolder : " + creationTimeHolder);
	 		while (creationTimeHolder > 0) {  
				//Debug.Log("Processing Unit : " + creationTimeHolder);
				creationTimeHolder -= Time.deltaTime;
	 			yield return null;
	 		}

				Debug.Log("Finished Unit");
			 	//UpdateUI
				 //Di ko pa rin alam kung pano yung sa UI
				UpdateUIList();
				//Get Index of Unit

				Debug.Log("Player Object : " + PO);
				Debug.Log("Unit System : " + PO.UnitSys);
				int unitIndex = PO.UnitSys.GetUnitIndex(u);


	 			//Spawn MonoUnitFramework
				PO.UnitSys.spawnUnit (unitIndex, spawnPoint, Quaternion.identity);
	 			//Move MonoUnitFramework to Rallypoint
				isDone = true;

				//Remove Unit
				RemoveFromQueue(0);
	 		
		}
		queueIsRunning = false;
		yield return null;
	}
	//Location Setting
	public void SetSpawnPoint(Vector3 newSpawnPoint){
		spawnPoint = newSpawnPoint;
	}
	public void SetRallyPoint(Vector3 newRallyPoint){
		rallyPoint = newRallyPoint;
	}


	//Queue Manipulation
	public void AddToQueue(int unitIndex){
		//check if Player has enough mana
		if(PO.manna < spawnableUnits[unitIndex].manaCost){
			//Tell the player that he doesn't have enough Mana
			Debug.Log("Not Enough Mana");
			return;
		}
		//check if the queue is full
		if(spawnQueue.Count <= 8){
			spawnQueue.Add(spawnableUnits[unitIndex]);
			UpdateUIList();
			//deduct mana from player
			PO.manna -= spawnableUnits[unitIndex].manaCost;
		}
		else{
			//the queue is Full
			Debug.Log("The Queue is full");
		}
		//check the queue is already being processed
		if(!queueIsRunning){
			StartCoroutine(ProcessQueue());
		}
	}

	public void RemoveFromQueue(int unitIndex){
		//refund mana
		PO.manna += spawnQueue[unitIndex].manaCost;
		//remove from queue
		spawnQueue.RemoveAt(unitIndex);
		//updateDisplay
		UpdateUIList();
	}

	//Display
	public void UpdateUIList(){
		//Clear Buttons
		//PO.uiGameManager.commandsHandler;
	}


}