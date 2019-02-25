using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

[RequireComponent (typeof (BuildingSystem))]
public class BuildingConstructor : NetworkBehaviour {
	int team;
	BuildingSystem buildingSystem;
	[SerializeField]
	int obstacleSizeCut = 2;
	[SerializeField]
	int obstacleHeightAdd = 2;
	public int rubbleIndex = 2;
	PlayerObject PO;

	void Awake () {
		buildingSystem = GetComponent<BuildingSystem> ();
	}
	void Start () {
		PO = buildingSystem.PO;
	}

	public void SpawnRubble (int buildingSpawnIndex, Vector3 position, Quaternion rotation, int t) {
		team = t;

		Debug.Log ("Building Constructor :: team : " + team);
		CmdSpawnObject (buildingSpawnIndex, position, rotation, team);

	}

	[Command]
	public void CmdSpawnObject (int buildingSpawnIndex, Vector3 position, Quaternion rotation, int _team) {
		Debug.Log ("Spawning rubble");
		Debug.Log ("BuidingConstructor :: CMDteam : " + team);
		Debug.Log ("BuidingConstructor :: buildingSpawnIndex : " + buildingSpawnIndex);

		//Copying the components of the building to the rubble
		GameObject rubble = NetworkManager.singleton.spawnPrefabs[rubbleIndex];
		Building bldg = buildingSystem.buildingGroups.buildings[buildingSystem.selectedBuildingIndex];

		rubble = Instantiate (rubble, position, rotation);

		//initialize mono constructable unit
		MonoConstructableUnit mcu = rubble.GetComponent<MonoConstructableUnit> ();
		mcu.InitializeConstructable (bldg, buildingSystem.selectedBuildingIndex, _team, buildingSystem.PO);

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

		NetworkIdentity ni = rubble.GetComponent<NetworkIdentity> ();
		Debug.Log ("Player Object :: --Spawning Unit");
		NetworkServer.SpawnWithClientAuthority (rubble, connectionToClient);
		bool ToF = rubble.GetComponent<NetworkIdentity> ().AssignClientAuthority (GetComponent<NetworkIdentity> ().connectionToClient);

		RpcAssignObject (ni, buildingSpawnIndex, _team);

	}

	[ClientRpc]
	public void RpcAssignObject (NetworkIdentity id, int spawnableObjectIndex, int _team) {

		Debug.Log ("RpcAssign");
		GameObject go = id.gameObject;
		Building bldg = buildingSystem.buildingGroups.buildings[buildingSystem.selectedBuildingIndex];
		GameObject rubble = NetworkManager.singleton.spawnPrefabs[rubbleIndex];

		//initialize mono constructable unit
		MonoConstructableUnit mcu = go.GetComponent<MonoConstructableUnit> ();
		mcu.InitializeConstructable (bldg, buildingSystem.selectedBuildingIndex, _team, buildingSystem.PO);

		Debug.Log ("Building System :: RpcAssign : GO : " + go);
		//Move to RPC
		//TODO: Find out for what this section is?
		GameObject buildingGraphics = bldg.graphics;
		GameObject buildingPrefab = NetworkManager.singleton.spawnPrefabs[1];
		BoxCollider buildingCollider = buildingPrefab.GetComponent<BoxCollider> ();
		buildingCollider.size = bldg.addedColliderScale;

		Vector3 sizeHolder = new Vector3 (bldg.rubbleSize.x, bldg.rubbleSize.y, bldg.rubbleSize.z);
		Vector3 rubbleSize = sizeHolder + new Vector3 (.1f, .1f, .1f);
		rubble.transform.localScale = rubbleSize;
		go.transform.localScale = rubbleSize;

		// rubbleSize.x = buildingCollider.size.x;
		//	rubbleSize.z = buildingCollider.size.z;

		BoxCollider rubbleCollider = rubble.GetComponent<BoxCollider> ();
		if (rubble.GetComponent<BoxCollider> () != null)
			Debug.Log ("Constructor :: Rubble Collider : " + rubbleCollider);

		NavMeshObstacle navMeshObstacle = rubble.GetComponent<NavMeshObstacle> ();
		Vector3 obstacleSize = bldg.rubbleObstacleSize;
		/* 		obstacleSize.x -= Mathf.Clamp (obstacleSizeCut, 1, int.MaxValue) / 10f;
				obstacleSize.y += Mathf.Clamp (obstacleHeightAdd / 100f, 2, int.MaxValue);
				obstacleSize.z -= Mathf.Clamp (obstacleSizeCut, 1, int.MaxValue) / 10f; */
		rubbleCollider.size = bldg.rubbleObstacleSize;
		navMeshObstacle.size = bldg.rubbleObstacleSize;

		string output = "";
		output += "Constructor :: (Before) \n";
		output += "  Building Index :: " + buildingSystem.selectedBuildingIndex + "\n";
		output += "  Building :: Rubble Size : " +  bldg.rubbleObstacleSize + "\n";
		output += "  Rubble   :: Rubble Size : " +  rubbleCollider.size + "\n";

		Debug.Log(output);


		//Assigning data
		MonoConstructableUnit building = go.GetComponent<MonoConstructableUnit> ();
		building.team = PO.team;
		//Debug.Log ("UnitSystem :: RpcAssign : MonoUnit : " + building);
		//Debug.Log ("UnitSystem :: RpcAssign : Primitive Abilities : " + building.primitiveAbilities.Count);

		//Debug.Log ("UnitSystem :: unit : " + unit);
		//Debug.Log ("UnitSystem :: playerunit : " + playerUnit);
		//Debug.Log ("UnitSystem :: abilities.Count : " + playerUnit.abilities.Count);

		//go.name = PO.team + " - consructble" + go.GetComponent<NetworkIdentity> ().netId;

		/* 		UnitSelectable unitSelectable = go.AddComponent<UnitSelectable> ();
				unitSelectable.playerObject = PO;
				PO.myBuildings.Add (go); */
	}

}