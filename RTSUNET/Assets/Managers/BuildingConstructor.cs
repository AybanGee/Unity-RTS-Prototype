using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

[RequireComponent(typeof(BuildingSystem))]
public class BuildingConstructor : NetworkBehaviour {
	int team;
	BuildingSystem buildingSystem;
	[SerializeField]
	int obstacleSizeCut = 2;
	[SerializeField]
	int obstacleHeightAdd = 2;
	public int rubbleIndex = 2;
	void Start () {
		buildingSystem = GetComponent<BuildingSystem> ();
	}
	
	public void SpawnRubble (int buildingSpawnIndex, Vector3 position, Quaternion rotation, int t) {
		team = t;
		CmdSpawnObject (buildingSpawnIndex,position,rotation);
	}

	[Command]
	public void CmdSpawnObject (int buildingSpawnIndex, Vector3 position, Quaternion rotation) {
		Debug.Log("Spawning rubble");
		//Copying the components of the building to the rubble
		GameObject rubble = NetworkManager.singleton.spawnPrefabs[rubbleIndex];
		Building bldg = buildingSystem.buildingGroups.buildings[buildingSystem.selectedBuildingIndex];
		GameObject buildingGraphics = bldg.graphics;

		//initialize mono constructable unit
		MonoConstructableUnit mcu = rubble.GetComponent<MonoConstructableUnit>();
		mcu.InitializeConstructable(bldg,buildingSpawnIndex,team,buildingSystem.PO);

		//TODO: Find out for what this section is?
		GameObject building = NetworkManager.singleton.spawnPrefabs[buildingSpawnIndex];
		BoxCollider buildingCollider = building.GetComponent<BoxCollider> ();
		buildingCollider.size = buildingGraphics.transform.localScale; // + buildingGroups.buildings[selectedBuildingIndex].addedColliderScale;
		Vector3 rubbleSize = rubble.transform.localScale;
		rubbleSize.x = buildingCollider.size.x;
		rubbleSize.z = buildingCollider.size.z;
		rubble.transform.localScale = rubbleSize;

		NavMeshObstacle navMeshObstacle = rubble.GetComponent<NavMeshObstacle> ();
		Vector3 obstacleSize = new Vector3 (1, 1, 1);
		obstacleSize.x -= Mathf.Clamp (obstacleSizeCut, 1, int.MaxValue) / 10f;
		obstacleSize.y += Mathf.Clamp (obstacleHeightAdd / 100f, 2, int.MaxValue);
		obstacleSize.z -= Mathf.Clamp (obstacleSizeCut, 1, int.MaxValue) / 10f;
		navMeshObstacle.size = obstacleSize;

		
/*
		//Assign data for the rubble
		ConstructionInteractable constructionInteractable = rubble.GetComponent<ConstructionInteractable> ();
		constructionInteractable.constructionTime = buildingSystem.buildingGroups.buildings[buildingSystem.selectedBuildingIndex].creationTime;
		constructionInteractable.buildingIndex = buildingSystem.selectedBuildingIndex;
		constructionInteractable.team = team;
		if (rubbleSize.x > rubbleSize.z)
			constructionInteractable.influenceRadius = rubbleSize.x + 1;
		else
			constructionInteractable.influenceRadius = rubbleSize.z + 1;
		constructionInteractable.playerObject = buildingSystem.PO;
 */
		//Instantiating the rubble
		rubble = Instantiate (rubble, position, rotation);

		NetworkIdentity ni = rubble.GetComponent<NetworkIdentity> ();
		Debug.Log ("Player Object :: --Spawning Unit");
		NetworkServer.Spawn (rubble);
		bool ToF = rubble.GetComponent<NetworkIdentity> ().AssignClientAuthority (GetComponent<NetworkIdentity> ().connectionToClient);

	}


}