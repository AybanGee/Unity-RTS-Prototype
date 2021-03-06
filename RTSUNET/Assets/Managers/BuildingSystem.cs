﻿using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;
[RequireComponent (typeof (PlayerObject))]
public class BuildingSystem : NetworkBehaviour {
	public BuildingConstructor constructor;
	public BuildingGroups buildingGroups;
	public bool buildMode = false;
	public bool isGrid = true;
	public int prefabBuildingIndex = 1;
	public int selectedBuildingIndex;
	GameObject buildingPlaceholder;
	BuildingCreationTrigger buildingCreationTrigger;
	[SerializeField]
	Material placeholderMat;
	[SerializeField]
	Material invalidPlaceholderMat;
	Renderer[] placeHolderRenderers;
	bool isValidLocation = true;
	[HideInInspector]
	public PlayerObject PO;
	[SerializeField]
	Vector3 buildingOffset = new Vector3 (0, 0, 0);
	[SerializeField]
	int obstacleSizeCut = 2;
	[SerializeField]
	int obstacleHeightAdd = 2;
	void Awake () {
		//Move to spawn manager
		PO = GetComponent<PlayerObject> ();
	}
	void Start () {
		constructor = GetComponent<BuildingConstructor> ();
		if (constructor == null) { Debug.LogError ("Building Constructor Not Found"); }
	}
	public void BuildingControl () {
		if (Input.GetKeyDown (KeyCode.Escape)) {
			ToggleBuildMode ();
		}
		Ray ray = PO.cam.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;
		Vector3 mouseWorldPointPosition;
		if (Physics.Raycast (ray, out hit, 10000, PO.movementMask)) {
			mouseWorldPointPosition = hit.point + buildingOffset;
			if (isGrid) {
				Vector3 clamped = mouseWorldPointPosition;
				clamped.x = Mathf.Round (clamped.x);
				clamped.z = Mathf.Round (clamped.z);
				mouseWorldPointPosition = clamped;
			}
		} else
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
			PO.manna -= buildingGroups.buildings[selectedBuildingIndex].manaCost;
			constructor.SpawnRubble (prefabBuildingIndex, mouseWorldPointPosition - buildingOffset, buildingPlaceholder.transform.rotation, PO.team);
			ToggleBuildMode ();
		}
	}
	public void ToggleBuildMode () {
		// if we are in build mode turn it off 
		//we can check here if manna is sufficient for the selected building
		//Condition NetworkManager.singleton.spawnPrefabs[selectedCreateBuildingIndex].GetComponent<BuildingUnit> ().buildingPrice > manna
		if (buildingGroups.buildings[selectedBuildingIndex].manaCost > PO.manna) {
			Debug.Log ("Insufficient funds");
			if (buildingPlaceholder)
				Destroy (buildingPlaceholder);
			buildMode = false;
			return;
		}

		TogglePlaceholder ();
		buildMode = !buildMode;
	}
	void TogglePlaceholder () {
		if (buildMode) {
			Debug.Log ("Exit Build Mode");

			Destroy (buildingPlaceholder);

		} else {
			Debug.Log ("Entering Build Mode");
			buildingPlaceholder = Instantiate (buildingGroups.buildings[selectedBuildingIndex].graphics);

			buildingOffset.y = buildingPlaceholder.transform.localScale.y / 2;

			BoxCollider buildingCollider = buildingPlaceholder.AddComponent<BoxCollider> ();
			buildingCollider.isTrigger = true;
			buildingCollider.size = buildingCollider.size + buildingGroups.buildings[selectedBuildingIndex].addedColliderScale;

			buildingCreationTrigger = buildingPlaceholder.AddComponent<BuildingCreationTrigger> ();
			placeHolderRenderers = buildingPlaceholder.GetComponentsInChildren<Renderer> ();
			TogglePlaceholderColor ();

		}
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

	#region "Network Spawn Building"
	public void SpawnBuilding (int spawnableIndex, Vector3 position, Quaternion rotation) {
		CmdSpawnObject (spawnableIndex, position, rotation);
	}

	[Command]
	public void CmdSpawnObject (int spawnableIndex, Vector3 position, Quaternion rotation) {

		GameObject go = NetworkManager.singleton.spawnPrefabs[prefabBuildingIndex];

		//if (graphics == null) { Debug.LogError ("No graphics"); }
		go = Instantiate (go, position, rotation);

		BuildingUnit buildingUnit = go.GetComponent<BuildingUnit> ();
		Vector3 navMeshObstacleSize = go.GetComponent<BoxCollider> ().size;

		AssignData (go, spawnableIndex, buildingUnit, navMeshObstacleSize);

		//DESTROY COMPONENTS NOT NEEDED
		Destroy (go.GetComponent<Rigidbody> ());
		Destroy (go.GetComponent<BuildingCreationTrigger> ());

		//SPAWNING AND AUTHORIZE BLDG
		NetworkServer.Spawn (go);
		bool ToF = go.GetComponent<NetworkIdentity> ().AssignClientAuthority (GetComponent<NetworkIdentity> ().connectionToClient);

		//SYNCING TO CLIENTS
		NetworkIdentity ni = go.GetComponent<NetworkIdentity> ();

		RpcAssignObject (ni, spawnableIndex);
	}

	[ClientRpc]
	public void RpcAssignObject (NetworkIdentity id, int spawnableIndex) {
		//	if(!isLocalPlayer)return;

		//Debug.Log(id.netId.Value);
		GameObject spawnHolder = id.gameObject;
		GameObject graphics = buildingGroups.buildings[spawnableIndex].graphics;
		Vector3 grapihicsOffset = new Vector3 (0, graphics.transform.localScale.y / 2, 0);
		graphics = Instantiate (graphics, spawnHolder.transform.position + grapihicsOffset, spawnHolder.transform.rotation, spawnHolder.transform);
		graphics.GetComponent<GraphicsHolder> ().colorize (LobbyManager.singleton.GetComponent<LobbyManager> ().gameColors.gameColorList () [PO.colorIndex]);
		BuildingUnit buildingUnit = spawnHolder.GetComponent<BuildingUnit> ();
		Vector3 navMeshObstacleSize = spawnHolder.GetComponent<BoxCollider> ().size;

		AssignData (spawnHolder, spawnableIndex, buildingUnit, navMeshObstacleSize);

		if (spawnHolder.transform.childCount <= 0) { Debug.LogError ("No graphics"); }
		NavMeshObstacle navMeshObstacle = spawnHolder.AddComponent (typeof (NavMeshObstacle)) as NavMeshObstacle;
		navMeshObstacleSize.x -= obstacleSizeCut;
		navMeshObstacleSize.y -= obstacleSizeCut;
		navMeshObstacleSize.z += obstacleHeightAdd;
		navMeshObstacle.size = navMeshObstacleSize;
		navMeshObstacle.carving = true;
		spawnHolder.name = PO.team + " - bldg - " + id.netId;

		spawnHolder.GetComponent<BuildingStats> ().netPlayer = PO;
		GraphicsHolder graphicsHolder = spawnHolder.GetComponentInChildren<GraphicsHolder> ();

		buildingUnit.team = PO.team;
		if (graphicsHolder != null)
			graphicsHolder.colorize (LobbyManager.singleton.GetComponent<LobbyManager> ().gameColors.gameColorList () [PO.colorIndex]);
		PO.myBuildings.Add (spawnHolder);

	}
	#endregion

	public void AssignData (GameObject spawnHolder, int spawnableIndex, BuildingUnit buildingUnit, Vector3 navMeshObstacleSize) {
		//Assign data

		buildingUnit.team = PO.team;
		buildingUnit.buildingType = buildingGroups.buildings[spawnableIndex].type;

		BuildingInteractable buildingInteractable = null;
		switch (buildingGroups.buildings[spawnableIndex].type) {
			case BuildingType.Barracks:
				buildingInteractable = spawnHolder.AddComponent<BuildingInteractable> ();
				break;
			case BuildingType.TownCenter:
				buildingInteractable = spawnHolder.AddComponent<BuildingInteractable> ();
				break;
			case BuildingType.Tower:
				buildingInteractable = spawnHolder.AddComponent<BuildingInteractable> ();
				break;
			case BuildingType.SupplyChain:
				buildingInteractable = spawnHolder.AddComponent<SupplyChainInteractable> ();
				break;
		}
		if (navMeshObstacleSize.x > navMeshObstacleSize.z)
			buildingInteractable.influenceRadius = navMeshObstacleSize.x + 1;
		else
			buildingInteractable.influenceRadius = navMeshObstacleSize.z + 1;
		//end of assignments

	}

}