using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EnemySpawn : NetworkBehaviour {

	public PlayerObject enemyPO;
	public SP_spawnholder sp_spawn;
	public Coroutine holderSearch;
	PlayerObject _po;
	bool isSpawned = false;

	public List<QuestTrigger> qt = new List<QuestTrigger>();
	void Start () {
		//holderSearch = StartCoroutine (FindingBaseLocation ());
	}
/* 	public override void OnStartAuthority () {
			if (GetComponent<NetworkIdentity> ().connectionToClient == null)
			Debug.LogError ("Oopsie doopsie");
		CmdOnStart();
	}

	[Command] void CmdOnStart () {
		if (GetComponent<NetworkIdentity> ().connectionToClient == null){
			
			Debug.LogError (gameObject.name + " Enemy Spawn has no connectionToClient");
		}
		//	Debug.Log ("Finding Spawn Holder");
		enemyPO = GetComponent<PlayerObject> ();
		holderSearch = StartCoroutine (FindingBaseLocation ());
	} */
	public IEnumerator FindingBaseLocation () {
		/* 	while (!enemyPO.connectionToClient.isReady) {
				Debug.LogWarning ("Wait for connection to client");
				yield return new WaitForSeconds (0.25f);
			} */
		while (sp_spawn == null) {
			if (SP_spawnholder.singleton != null)
				sp_spawn = SP_spawnholder.singleton;
			Debug.Log ("while is running");
			yield return null;
		}

		if (sp_spawn != null) {
			Debug.Log ("holders are found");

			if (sp_spawn != null && !isSpawned) {
				//spawnObjects ();
				_po = transform.parent.GetChild(0).GetComponent<PlayerObject>();
				_po.SpawnEnemyBuildings();
			}
			//isFinishedLoading = true;
			//moveCamToBase();
		} else
			Debug.Log ("holders are not found");
		//Invoke spawning here	

		yield return null;

		_po.SwapPOTeams(false);
	}

	private void Update () {
		if (sp_spawn == null)
			holderSearch = StartCoroutine (FindingBaseLocation ());

	}

	public void spawnObjects (PlayerObject PO) {
		//BuildingSystem BuildSys = GetComponent<PlayerObject>().BuildSys;
		foreach (GameObject spawnHolder in sp_spawn.baseLocations) {
			int buildingIndex = 3;
			Debug.Log ("Spawning Townhall");
			PO.BuildSys.SpawnBuilding (buildingIndex, spawnHolder.transform.position, spawnHolder.transform.rotation);
		}
		foreach (GameObject spawnHolder in sp_spawn.barracksLocations) {
			int buildingIndex = 0;
			Debug.Log ("Spawning Barracks");
			PO.BuildSys.SpawnBuilding (buildingIndex, spawnHolder.transform.position, spawnHolder.transform.rotation);
		}
		foreach (GameObject spawnHolder in sp_spawn.supplyLocations) {
			int buildingIndex = 1;
			Debug.Log ("Spawning Supply Stash");
			PO.BuildSys.SpawnBuilding (buildingIndex, spawnHolder.transform.position, spawnHolder.transform.rotation);
		}
		foreach (GameObject spawnHolder in sp_spawn.towerLocations) {
			int buildingIndex = 2;
			Debug.Log ("Spawning Towers");
			PO.BuildSys.SpawnBuilding (buildingIndex, spawnHolder.transform.position, spawnHolder.transform.rotation);
			//add triggers here
			
		}

		isSpawned = true;
		//PO.SwapPOTeams(false);
	}
}