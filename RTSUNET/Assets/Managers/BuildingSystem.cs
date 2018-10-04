using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AI;
[RequireComponent(typeof(PlayerObject))]
public class BuildingSystem : NetworkBehaviour {

	public bool buildMode = false;
	public int selectedCreateBuildingIndex;
	GameObject buildingPlaceholder;
	BuildingCreationTrigger buildingCreationTrigger;
	[SerializeField]
	Material placeholderMat;
	[SerializeField]
	Material invalidPlaceholderMat;
	Renderer[] placeHolderRenderers;
	bool isValidLocation = true;
	PlayerObject PO;
	void Start()
	{
		PO = GetComponent<PlayerObject>();
	}
	public void BuildingControl(){
		if (Input.GetKeyDown (KeyCode.Escape)) {
				ToggleBuildMode ();
			}
			Ray ray = PO.cam.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			Vector3 mouseWorldPointPosition;
			if (Physics.Raycast (ray, out hit, 10000, PO.movementMask))
				mouseWorldPointPosition = hit.point;
			else
				return;

			if (buildingPlaceholder != null) {
				if (Input.GetKeyDown (KeyCode.Less) || Input.GetKeyDown (KeyCode.Comma))
					buildingPlaceholder.transform.Rotate (0, -45, 0);
				else if (Input.GetKeyDown (KeyCode.Greater) || Input.GetKeyDown (KeyCode.Period))
					buildingPlaceholder.transform.Rotate (0, 45, 0);

				buildingPlaceholder.transform.position = mouseWorldPointPosition;
				//check validity of location
				if (buildingCreationTrigger != null) {
					if (buildingCreationTrigger.colliderCount > 0) {
						isValidLocation = false;
						TogglePlaceholderColor ();

					} else {
						isValidLocation = true;
						TogglePlaceholderColor ();
					}
				}

			}
			if (Input.GetMouseButtonDown (0) && isValidLocation) {
				PO.manna -= NetworkManager.singleton.spawnPrefabs[selectedCreateBuildingIndex].GetComponent<BuildingUnit> ().buildingPrice;
				CmdSpawnRubble (selectedCreateBuildingIndex, mouseWorldPointPosition, buildingPlaceholder.transform.rotation, PO.team);
				ToggleBuildMode ();
			}
	}
	public void ToggleBuildMode () {
		// if we are in build mode turn it off 
		//we can check here if manna is sufficient for the selected building
		//Condition NetworkManager.singleton.spawnPrefabs[selectedCreateBuildingIndex].GetComponent<BuildingUnit> ().buildingPrice > manna
		if (NetworkManager.singleton.spawnPrefabs[selectedCreateBuildingIndex].GetComponent<BuildingUnit> ().buildingPrice > PO.manna) {
			Debug.Log ("Insufficient funds");
			if (buildingPlaceholder)
				Destroy (buildingPlaceholder);
			buildMode = false;
			return;
		}
		if (buildMode) {
			Debug.Log ("Exit Build Mode");

			Destroy (buildingPlaceholder);

		} else {
			Debug.Log ("Entering Build Mode");
			buildingPlaceholder = Instantiate (NetworkManager.singleton.spawnPrefabs[selectedCreateBuildingIndex]);
			buildingPlaceholder.GetComponent<BuildingUnit> ().AscendOnStart = false;
			buildingCreationTrigger = buildingPlaceholder.GetComponent<BuildingCreationTrigger> ();
			placeHolderRenderers = buildingPlaceholder.GetComponentsInChildren<Renderer> ();
			TogglePlaceholderColor ();

		}

		buildMode = !buildMode;
	}

	public void TogglePlaceholderColor () {
		if (buildingPlaceholder != null)
			if (placeHolderRenderers.Length > 0)
				for (int i = 0; i < placeHolderRenderers.Length; i++) {
					if (isValidLocation)
						placeHolderRenderers[i].material = placeholderMat;
					else
						placeHolderRenderers[i].material = invalidPlaceholderMat;
				}
	}

	[SerializeField]
	int obstacleSizeCut = 2;
	[SerializeField]
	int obstacleHeightAdd = 2;
	#region "Network Spawn Building"
	[Command]
	public void CmdSpawnBuilding (int spawnableIndex, Vector3 position, Quaternion rotation) {

		GameObject go = NetworkManager.singleton.spawnPrefabs[spawnableIndex];
		go = Instantiate (go, position, rotation);
		Renderer[] renderers = go.GetComponent<BuildingUnit> ().teamColoredGfx;
		if (renderers.Length > 0)
			for (int i = 0; i < renderers.Length; i++) {
				renderers[i].material.color = PO.selectedColor[PO.team - 1];
			}
		//Assign data
		Vector3 navMeshObstacleSize = go.GetComponent<BoxCollider> ().size;
		go.GetComponent<BuildingUnit> ().team = PO.team;
		BuildingInteractable buildingInteractable = go.GetComponent<BuildingInteractable> ();
		if (navMeshObstacleSize.x > navMeshObstacleSize.z)
			buildingInteractable.influenceRadius = navMeshObstacleSize.x + 1;
		else
			buildingInteractable.influenceRadius = navMeshObstacleSize.z + 1;

		//DESTROY COMPONENTS NOT NEEDED
		Destroy (go.GetComponent<Rigidbody> ());
		Destroy (go.GetComponent<BuildingCreationTrigger> ());

		//SPAWNING AND AUTHORIZE BLDG
		NetworkServer.Spawn (go);
		bool ToF = go.GetComponent<NetworkIdentity> ().AssignClientAuthority (GetComponent<NetworkIdentity> ().connectionToClient);

		//SYNCING TO CLIENTS
		NetworkIdentity ni = go.GetComponent<NetworkIdentity> ();
		RpcAssignBuilding (ni, PO.team, navMeshObstacleSize);
	}

	[ClientRpc]
	void RpcAssignBuilding (NetworkIdentity id, int t, Vector3 navMeshObstacleSize) {
		//	if(!isLocalPlayer)return;

		//Debug.Log(id.netId.Value);
		GameObject spawnHolder = id.gameObject;
		NavMeshObstacle navMeshObstacle = spawnHolder.AddComponent (typeof (NavMeshObstacle)) as NavMeshObstacle;
		navMeshObstacleSize.x -= obstacleSizeCut;
		navMeshObstacleSize.y -= obstacleSizeCut;
		navMeshObstacleSize.z += obstacleHeightAdd;
		navMeshObstacle.size = navMeshObstacleSize;
		navMeshObstacle.carving = true;
		spawnHolder.name = PO.team + " - bldg - " + netId;

		spawnHolder.GetComponent<BuildingStats> ().netPlayer = PO;
		BuildingUnit buildingUnit = spawnHolder.GetComponent<BuildingUnit> ();
		buildingUnit.team = PO.team;
		Renderer[] renderers = buildingUnit.teamColoredGfx;
		if (renderers.Length > 0)
			for (int i = 0; i < renderers.Length; i++) {
				renderers[i].material.color = PO.selectedColor[t - 1];
			}
		PO.myBuildings.Add (spawnHolder);

	}
	#endregion
	#region "Network Spawn Rubble"
	[Command]
	void CmdSpawnRubble (int buildingSpawnIndex, Vector3 position, Quaternion rotation, int t) {
		int rubbleIndex = 2;
		//Copying the components of the building to the rubble
		GameObject rubble = NetworkManager.singleton.spawnPrefabs[rubbleIndex];
		GameObject building = NetworkManager.singleton.spawnPrefabs[buildingSpawnIndex];
		BoxCollider buildingCollider = building.GetComponent<BoxCollider> ();
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

		//Assign data for the rubble
		ConstructionInteractable constructionInteractable = rubble.GetComponent<ConstructionInteractable> ();
		constructionInteractable.constructionTime = building.GetComponent<BuildingUnit> ().constructionTime;
		constructionInteractable.buildingIndex = buildingSpawnIndex;
		constructionInteractable.team = t;
		if (rubbleSize.x > rubbleSize.z)
			constructionInteractable.influenceRadius = rubbleSize.x + 1;
		else
			constructionInteractable.influenceRadius = rubbleSize.z + 1;
		constructionInteractable.playerObject = PO;
		//Instantiating the rubble
		rubble = Instantiate (rubble, position, rotation);

		NetworkIdentity ni = rubble.GetComponent<NetworkIdentity> ();
		Debug.Log ("Player Object :: --Spawning Unit");
		NetworkServer.Spawn (rubble);
		bool ToF = rubble.GetComponent<NetworkIdentity> ().AssignClientAuthority (GetComponent<NetworkIdentity> ().connectionToClient);

		Debug.Log ("Player Object :: --Unit Spawned : " + ToF);
		RpcSpawnRubble (ni, t);
	}

	[ClientRpc]
	void RpcSpawnRubble (NetworkIdentity ni, int t) {
		ConstructionInteractable constructionInteractable = ni.gameObject.GetComponent<ConstructionInteractable> ();
		constructionInteractable.playerObject = PO;
		constructionInteractable.team = t;
	}
	#endregion

}
