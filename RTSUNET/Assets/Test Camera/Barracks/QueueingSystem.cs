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
	Coroutine processQueue;

	void Start () {
		queueIsRunning = false;

	}

	IEnumerator ProcessQueue () {
		bool isDone = false;
		while (spawnQueue.Count > 0) {

			queueIsRunning = true;
			PlayerUnit u = spawnQueue[0];
			//if(isDone)			
			creationTimeHolder = u.creationTime;

			//Debug.Log("CreationHolder : " + creationTimeHolder);
			while (creationTimeHolder > 0) {
				//Debug.Log("Processing Unit : " + creationTimeHolder);
				if (spawnQueue.Count == 0) {
					queueIsRunning = false;
					StopCoroutine (processQueue);
				}

				creationTimeHolder -= Time.deltaTime;
				yield return null;
			}

			//Debug.Log("Finished Unit");
			//UpdateUI
			//Di ko pa rin alam kung pano yung sa UI
			Debug.Log ("is Selected : " + GetComponent<UnitSelectable> ().isSelected);
			if (GetComponent<UnitSelectable> ().isSelected)
				UpdateUIList ();
			//Get Index of Unit

			//Debug.Log("Player Object : " + PO);
			//Debug.Log("Unit System : " + PO.UnitSys);
			int unitIndex = PO.UnitSys.GetUnitIndex (u);

			//Spawn MonoUnitFramework
			PO.UnitSys.spawnUnit (unitIndex, spawnPoint, Quaternion.identity);
			//Move MonoUnitFramework to Rallypoint
			isDone = true;

			//Remove Unit
			QueueRemove (0);

		}
		queueIsRunning = false;
		yield return null;
	}
	//Location Setting
	public void SetSpawnPoint (Vector3 newSpawnPoint) {
		spawnPoint = newSpawnPoint;
	}
	public void SetRallyPoint (Vector3 newRallyPoint) {
		rallyPoint = newRallyPoint;
	}

	//Queue Manipulation
	public void AddToQueue (int unitIndex) {
		//check if Player has enough mana
		if (PO.manna < spawnableUnits[unitIndex].manaCost) {
			//Tell the player that he doesn't have enough Mana
			Debug.Log ("Not Enough Mana");
			return;
		}
		//check if the queue is full
		if (spawnQueue.Count <= 8) {
			spawnQueue.Add (spawnableUnits[unitIndex]);
			UpdateUIList ();
			//deduct mana from player
			PO.manna -= spawnableUnits[unitIndex].manaCost;
		} else {
			//the queue is Full
			Debug.Log ("The Queue is full");
		}
		//check the queue is already being processed
		if (!queueIsRunning) {
			processQueue = StartCoroutine (ProcessQueue ());
		}
	}

	public void RemoveFromQueue (int unitIndex) {
		
		//Debug.Log ("unit Index : " + unitIndex);
		if (unitIndex > spawnQueue.Count - 1) { Debug.LogError ("Index " + unitIndex + " was out of range FUUUU-"); return; }
		
		//Refund Manna
		PO.manna += spawnQueue[unitIndex].manaCost;
		
		//Remove from Queue
		spawnQueue.RemoveAt (unitIndex);

		//Reset Timer if First Unit
		if (unitIndex == 0 && spawnQueue.Count > 0) {
			PlayerUnit u = spawnQueue[0];
			creationTimeHolder = u.creationTime;
		}

		//Update Display
		if (GetComponent<UnitSelectable> ().isSelected)
			UpdateUIList ();
	}

	public void QueueRemove (int unitIndex) {

		if (unitIndex > spawnQueue.Count - 1) { Debug.LogError ("Index " + unitIndex + " was out of range FUUUU-"); return; }

		//Remove From List
		spawnQueue.RemoveAt (unitIndex);

		//Reset Timer
		if (unitIndex == 0 && spawnQueue.Count > 0) {
			PlayerUnit u = spawnQueue[0];
			creationTimeHolder = u.creationTime;
		}

		//Update Display
		if (GetComponent<UnitSelectable> ().isSelected)
			UpdateUIList ();
	}

	//Display
	public void UpdateUIList () {
		//Clear Buttons
		PO.uiGameManager.commandsHandler.ShowProcessQueue (spawnQueue, this);
	}

}