using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;
[RequireComponent (typeof (PlayerObject))]
public class BuildingSystem : NetworkBehaviour {
	public BuildingConstructor constructor;
	public BuildingGroups buildingGroups, buildableGroup;
	public bool buildMode = false;
	public bool isGrid = true;
	public int prefabBuildingIndex = 1;
	public int selectedBuildingIndex; // AYBAN ETO YUNG BUILDING NA GUSTO MONG PALITAN
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

	Vector3 buildingOffset = new Vector3 (0, 0, 0);
	[SerializeField]
	int obstacleSizeCut = 0;
	[SerializeField]
	int obstacleHeightAdd = 2;

	void Awake () {
		//Move to spawn manager
		PO = GetComponent<PlayerObject> ();
		Debug.Log (PO);
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

				if (buildingGroups.buildings[selectedBuildingIndex].type == BuildingType.SupplyChain) {
					if (!buildingCreationTrigger.inRange) {
						isValidLocation = false;
						TogglePlaceholderColor ();

					} else if (buildingCreationTrigger.inRange && buildingCreationTrigger.colliderCount > 0) {
						isValidLocation = false;
						TogglePlaceholderColor ();
					} else {
						isValidLocation = true;
						TogglePlaceholderColor ();

					}
				}

			}

		}
		if (Input.GetMouseButtonDown (0) && isValidLocation) {
			PO.manna -= buildingGroups.buildings[selectedBuildingIndex].manaCost;
			Debug.Log ("Building System :: team : " + PO.team);
			constructor.SpawnRubble (selectedBuildingIndex, mouseWorldPointPosition - buildingOffset, buildingPlaceholder.transform.rotation, PO.team);
			ToggleBuildMode ();
		} else if (Input.GetMouseButtonDown (0) && !isValidLocation) {
			if ((buildingGroups.buildings[selectedBuildingIndex].type == BuildingType.SupplyChain)) {
				if (!buildingCreationTrigger.inRange) {
					PO.ShowNotice ("Supply Stash must be built near Ore.");
				} else if (buildingCreationTrigger.inRange && buildingCreationTrigger.colliderCount > 0) {
					PO.ShowNotice ("Building Obstructed.");
				}
			} else {
				PO.ShowNotice ("Building Obstructed.");
			}

		}

	}
	public void ToggleBuildMode () {
		// if we are in build mode turn it off 
		//we can check here if manna is sufficient for the selected building
		//Condition NetworkManager.singleton.spawnPrefabs[selectedCreateBuildingIndex].GetComponent<BuildingUnit> ().buildingPrice > manna
		if (buildingGroups.buildings[selectedBuildingIndex].manaCost > PO.manna) {
			Debug.Log ("Insufficient funds");
			PO.ShowNotice ("Insufficient Manna");
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
			//buildingCollider.size = buildingGroups.buildings[selectedBuildingIndex].addedColliderScale + new Vector3(.1f,0,.1f);

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
		MonoBuilding buildingUnit = go.GetComponent<MonoBuilding> ();
		//Vector3 navMeshObstacleSize = go.GetComponent<BoxCollider> ().size - new Vector3 (1, 0, 1);

		//AssignData (go, spawnableIndex, buildingUnit, navMeshObstacleSize);

		//if (graphics == null) { Debug.LogError ("No graphics"); }
		go = Instantiate (go, position, rotation, this.transform);

		/* 		MonoBuilding  buildingUnit = go.GetComponent<MonoBuilding> ();
				//BuildingUnit buildingUnit = go.GetComponent<BuildingUnit> ();
				Vector3 navMeshObstacleSize = go.GetComponent<BoxCollider> ().size;
		 */

		/* 		MonoUnitFramework muf = go.GetComponent<MonoUnitFramework> ();
				Building building = buildingGroups.buildings[spawnableIndex];
				//muf.playerUnit = buildingUnit;
				muf.PO = PO;
				building.Initialize (go);

				AssignData (go, spawnableIndex, buildingUnit, navMeshObstacleSize);
		 */

		//DESTROY COMPONENTS NOT NEEDED
		/* 	Destroy (go.GetComponent<Rigidbody> ());
		Destroy (go.GetComponent<BuildingCreationTrigger> ());
 */
		//SPAWNING AND AUTHORIZE BLDG
		NetworkServer.SpawnWithClientAuthority (go, connectionToClient);
		bool ToF = go.GetComponent<NetworkIdentity> ().AssignClientAuthority (GetComponent<NetworkIdentity> ().connectionToClient);

		//SYNCING TO CLIENTS
		NetworkIdentity ni = go.GetComponent<NetworkIdentity> ();

		RpcAssignObject (ni, spawnableIndex);
	}

	[ClientRpc]
	public void RpcAssignObject (NetworkIdentity id, int spawnableIndex) {
		Debug.Log ("Building RPC Assign");

		//RpcAddPlayerUnit
		GameObject spawnHolder = id.gameObject;
		Building building = buildingGroups.buildings[spawnableIndex];
		MonoUnitFramework muf = spawnHolder.GetComponent<MonoUnitFramework> ();

		//AssignData
		muf.building = building;
		muf.PO = PO;
		building.Initialize (spawnHolder);

		//Add Graphics
		Debug.Log ("RPC add Graphics");

		GameObject graphics = buildingGroups.buildings[spawnableIndex].graphics;
		Vector3 grapihicsOffset = new Vector3 (0, graphics.transform.localScale.y / 2, 0);

		graphics = Instantiate (graphics, spawnHolder.transform.position /* + grapihicsOffset */ , spawnHolder.transform.rotation, spawnHolder.transform);
		graphics.GetComponent<GraphicsHolder> ().colorize (LobbyManager.singleton.GetComponent<LobbyManager> ().gameColors.gameColorList () [PO.colorIndex]);

		MonoBuilding buildingUnit = spawnHolder.GetComponent<MonoBuilding> ();
		Vector3 navMeshObstacleSize = buildingGroups.buildings[spawnableIndex].addedColliderScale;

		if (spawnHolder.transform.childCount <= 0) { Debug.LogError ("No graphics"); }
		NavMeshObstacle navMeshObstacle = spawnHolder.AddComponent (typeof (NavMeshObstacle)) as NavMeshObstacle;
		//navMeshObstacleSize.x -= obstacleSizeCut;
		//navMeshObstacleSize.z += obstacleSizeCut;
		navMeshObstacleSize.y = obstacleHeightAdd;

		navMeshObstacle.size = navMeshObstacleSize;
		navMeshObstacle.carving = true;

		BoxCollider boxCollider = spawnHolder.GetComponent<BoxCollider> ();
		navMeshObstacleSize.y *= 4;
		boxCollider.size = navMeshObstacleSize;
		spawnHolder.name = PO.team + " - bldg - " + id.netId;

		//		spawnHolder.GetComponent<BuildingStats> ().netPlayer = PO;
		GraphicsHolder graphicsHolder = spawnHolder.GetComponentInChildren<GraphicsHolder> ();

		buildingUnit.team = PO.team;
		//Unit seelctable for bldgs
		UnitSelectable unitSelectable = spawnHolder.AddComponent<UnitSelectable> ();
		unitSelectable.playerObject = PO;
		unitSelectable.isOneSelection = true;
		if (graphicsHolder != null)
			graphicsHolder.colorize (LobbyManager.singleton.GetComponent<LobbyManager> ().gameColors.gameColorList () [PO.colorIndex]);
		PO.myBuildings.Add (spawnHolder);

		AssignData (spawnHolder, spawnableIndex, buildingUnit, navMeshObstacleSize);

	}
	#endregion

	public void AssignData (GameObject spawnHolder, int spawnableIndex, MonoBuilding buildingUnit, Vector3 navMeshObstacleSize) {
		//Assign data

		//nauunang subukun lagyan ng abilities bago ma assign
		Debug.Log ("Assigning Data");

		/* 	
			buildingUnit.team = PO.team;
			buildingUnit.buildingType = buildingGroups.buildings[spawnableIndex].type;	
			buildingUnit.primitiveAbilities = buildingGroups.buildings[spawnableIndex].abilities;
			Debug.Log("abilities Count" + buildingGroups.buildings[spawnableIndex].abilities.Count);
			Debug.Log("prime abilities Count" +buildingUnit.primitiveAbilities.Count);
		 */ //Dito may abilities na

		//Assigning of special scripts for building
		/* 		BuildingInteractable buildingInteractable = null;
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

			*/

		if (!isLocalPlayer) return;
		//if (spawnHolder.GetComponent<QueueingSystem> () != null) return;

		QueueingSystem qs;
		TownhallTrigger tt;
		DefaultSkillManager ds;

		switch (buildingGroups.buildings[spawnableIndex].type) {
			case BuildingType.Barracks:
				Debug.Log ("Barracks Time");

				if (spawnHolder.GetComponent<QueueingSystem> () == null) {
					qs = spawnHolder.AddComponent<QueueingSystem> ();
					qs.PO = PO;
					qs.spawnableUnits = PO.UnitSys.bGroup.units;
				}

				break;
			case BuildingType.TownCenter:
				Debug.Log ("TownHall Time");
				if (spawnHolder.GetComponent<QueueingSystem> () == null) {
					qs = spawnHolder.AddComponent<QueueingSystem> ();
					qs.PO = PO;
					qs.spawnableUnits = PO.UnitSys.thGroup.units;
				}
				if (spawnHolder.GetComponent<TownhallTrigger> () == null) {
					tt = spawnHolder.AddComponent<TownhallTrigger> ();
					tt.PO = PO;
				}
				break;
			case BuildingType.Tower:
				if(spawnHolder.GetComponent<DefaultSkillManager>() == null){
					ds = spawnHolder.AddComponent<DefaultSkillManager>();
				}
				break;
			case BuildingType.SupplyChain:
				Debug.Log ("Supply Time");
				if (spawnHolder.GetComponent<TownhallTrigger> () == null) {
					qs = spawnHolder.AddComponent<QueueingSystem> ();
					qs.PO = PO;
					qs.spawnableUnits = PO.UnitSys.thGroup.units;
				}

				spawnHolder.AddComponent<Supplier> ();
				break;
		}
		//end of assignments

	}

}