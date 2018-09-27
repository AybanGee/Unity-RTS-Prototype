using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.Networking;

public class PlayerObject : NetworkBehaviour {

	public List<GameObject> selectedUnits = new List<GameObject> ();
	public List<GameObject> myUnits = new List<GameObject> ();
	public List<GameObject> myBuildings = new List<GameObject> ();
	public Camera cam;
	public List<Color> selectedColor = new List<Color> ();
	float ang;
	public LayerMask movementMask;
	//passed variables
	[SyncVar]
	public int team;
	[SyncVar]
	public int faction;
	[SyncVar]
	public string playerName;
	public int manna;

	// Use this for initialization
	void Start () {
		//Is this actually my own local PlayerObject?
		if (isLocalPlayer == false) {
			//This object belongs to another player.
			return;
		}
		if (DragSelectionHandler.singleton.playerObject == null) {
			DragSelectionHandler.singleton.AssignPlayerObject (this);
		}

		gameObject.name = gameObject.name + "NID" + GetComponent<NetworkIdentity> ().netId;

		Debug.Log ("PlayerObject::Start -- Spawning my own personal Unit");

		cam = Camera.main;

	}
	//builder vars
	bool buildMode = false;
	public int selectedCreateBuildingIndex;
	GameObject buildingPlaceholder;
	BuildingCreationTrigger buildingCreationTrigger;
	[SerializeField]
	Material placeholderMat;
	[SerializeField]
	Material invalidPlaceholderMat;
	Renderer[] placeHolderRenderers;
	bool isValidLocation = true;
	void Update () {
		//Remember: Update runs on EVERYONE's computer, wether or not they own this
		//particular player object.
		if (isLocalPlayer == false) {
			return;
		}

		//Spawns Unit DEBUG ONLY
		if (Input.GetKeyDown (KeyCode.Space)) {
			spawnUnit (0);
		}
		if (Input.GetKeyDown (KeyCode.M)) {
			spawnUnit (3);
		}
		if (Input.GetKeyDown (KeyCode.B)) {
			ToggleBuildMode ();
		}
		if (buildMode) {
			if (Input.GetKeyDown (KeyCode.Escape)) {
				ToggleBuildMode ();
			}
			Ray ray = cam.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			Vector3 mouseWorldPointPosition;
			if (Physics.Raycast (ray, out hit, 10000, movementMask))
				mouseWorldPointPosition = hit.point;
			else
				return;

			if (buildingPlaceholder != null) {

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
				manna -= NetworkManager.singleton.spawnPrefabs[selectedCreateBuildingIndex].GetComponent<BuildingUnit> ().buildingPrice;
				CmdSpawnRubble (selectedCreateBuildingIndex, mouseWorldPointPosition, team);
				ToggleBuildMode ();
			}
		} else
			//Move selected units to point
			if (Input.GetMouseButtonDown (1)) {
				//Slow script

				Ray ray = cam.ScreenPointToRay (Input.mousePosition);
				RaycastHit hit;

				//There should be at least 1 selected unit.

				if (Physics.Raycast (ray, out hit, 10000)) {

					Interactable interactable = hit.collider.GetComponent<Interactable> ();
					//.Log("JORI JORI AJA AJA " + hit.collider.name);
					if (interactable != null) {
						//Checks if enemy unit
						if (interactable.GetComponent<Unit> () != null ) {
							if (interactable.GetComponent<Unit> ().team == team) {
								//hard coded stuff;
								Debug.Log ("Oops Same team");
								return;
							} else {
								Debug.Log ("SHIT KALABAN!");
								if (myUnits.Count > 0)
									foreach (GameObject unit in selectedUnits) {
										//TO BE REMOVED IF FOUNF SOLUTION ON NOT DELETEiNG OBJECTS
										if (unit == null) {
											selectedUnits.Remove (unit);
											continue;
										}

										CmdFocus (unit.GetComponent<NetworkIdentity> (), interactable.GetComponent<NetworkIdentity> ());
									}
								return;
							}
						}
						else if (interactable.GetComponent<BuildingUnit>() != null) {
							if (interactable.GetComponent<BuildingUnit>().team == team) {
								//hard coded stuff;
								Debug.Log ("Oops Same team");
								return;
							} else {
								Debug.Log ("SHIT BLDG NG KALABAN!");
								if (myUnits.Count > 0)
									foreach (GameObject unit in selectedUnits) {
										//TO BE REMOVED IF FOUNF SOLUTION ON NOT DELETEiNG OBJECTS
										if (unit == null) {
											selectedUnits.Remove (unit);
											continue;
										}

										CmdFocus (unit.GetComponent<NetworkIdentity> (), interactable.GetComponent<NetworkIdentity> ());
									}
								return;
							}
						}
						//Checks if construction
						else if (interactable.GetComponent<ConstructionInteractable> () != null) {
							if (interactable.GetComponent<ConstructionInteractable> ().team == team) {
								Debug.Log ("Lets Construct");
								if (myUnits.Count > 0)
									foreach (GameObject unit in selectedUnits) {
										//TO BE REMOVED IF FOUNF SOLUTION ON NOT DELETEiNG OBJECTS
										if (unit == null) {
											selectedUnits.Remove (unit);
											continue;
										}
										if (unit.GetComponent<Builder> () != null) {
											CmdFocus (unit.GetComponent<NetworkIdentity> (), interactable.GetComponent<NetworkIdentity> ());
											break;
										}
									}
							} else {

								Debug.Log ("Not ours! Your team:" + team + "  rubbles team:" + interactable.GetComponent<ConstructionInteractable> ().team);
								return;
							}

						}

					}
				}

				if (myUnits.Count <= 0) return;
				if (Physics.Raycast (ray, out hit, 10000, movementMask)) {
					//move movement mask to controller
					//Debug.Log("We Hit" +  hit.collider.name + " " + hit.point );
					//Point of Click and location of selected Unit = angle, for formation

					// Move our player to what we hit
					//moveUnits(hit.point);
					moveUnits (hit.point);
					//myUnit.GetComponent<Unit>().MoveToPoint(hit.point);

					//Stop focusing other objects
				}

			}
	}
	#region "Building System"
	public void ToggleBuildMode () {
		// if we are in build mode turn it off 
		//we can check here if manna is sufficient for the selected building
		//Condition NetworkManager.singleton.spawnPrefabs[selectedCreateBuildingIndex].GetComponent<BuildingUnit> ().buildingPrice > manna
		if (NetworkManager.singleton.spawnPrefabs[selectedCreateBuildingIndex].GetComponent<BuildingUnit> ().buildingPrice > manna) {
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
	public void CmdSpawnBuilding (int spawnableIndex, Vector3 position) {

		GameObject go = NetworkManager.singleton.spawnPrefabs[spawnableIndex];
		go = Instantiate (go, position, Quaternion.identity);
		Renderer[] renderers = go.GetComponent<BuildingUnit> ().teamColoredGfx;
		if (renderers.Length > 0)
			for (int i = 0; i < renderers.Length; i++) {
				renderers[i].material.color = selectedColor[team - 1];
			}
		//Assign data
		Vector3 navMeshObstacleSize = go.GetComponent<BoxCollider> ().size;
		go.GetComponent<BuildingUnit> ().team = team;
		BuildingInteractable buildingInteractable = go.GetComponent<BuildingInteractable>();
		if(navMeshObstacleSize.x > navMeshObstacleSize.z)
		buildingInteractable.radius = navMeshObstacleSize.x + 1;
		else
		buildingInteractable.radius = navMeshObstacleSize.z + 1;
		
		//DESTROY COMPONENTS NOT NEEDED
		Destroy (go.GetComponent<Rigidbody> ());
		Destroy (go.GetComponent<BuildingCreationTrigger> ());

		
		//SPAWNING AND AUTHORIZE BLDG
		NetworkServer.Spawn (go);
		bool ToF = go.GetComponent<NetworkIdentity> ().AssignClientAuthority (GetComponent<NetworkIdentity> ().connectionToClient);

		//SYNCING TO CLIENTS
		NetworkIdentity ni = go.GetComponent<NetworkIdentity> ();
		RpcAssignBuilding (ni, team, navMeshObstacleSize);
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
		spawnHolder.name = team + " - bldg - " + netId;

		spawnHolder.GetComponent<BuildingStats> ().netPlayer = this;
		Renderer[] renderers = spawnHolder.GetComponent<BuildingUnit> ().teamColoredGfx;
		if (renderers.Length > 0)
			for (int i = 0; i < renderers.Length; i++) {
				renderers[i].material.color = selectedColor[t - 1];
			}
		myBuildings.Add (spawnHolder);

	}
	#endregion
	#region "Network Spawn Rubble"
	[Command]
	void CmdSpawnRubble (int buildingSpawnIndex, Vector3 position, int t) {
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
			constructionInteractable.radius = rubbleSize.x + 1;
		else
			constructionInteractable.radius = rubbleSize.z + 1;
		constructionInteractable.playerObject = this;
		//Instantiating the rubble
		rubble = Instantiate (rubble, position, Quaternion.identity);

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
		constructionInteractable.playerObject = this;
		constructionInteractable.team = t;
	}
	#endregion
	#endregion
	#region "Interactions"
	[Command]
	void CmdFocus (NetworkIdentity unitID, NetworkIdentity interactionId) {
		unitID.GetComponent<Unit> ().SetFocus (interactionId.GetComponent<Interactable> ());
		RpcFocus (unitID, interactionId);
	}

	[ClientRpc]
	void RpcFocus (NetworkIdentity unitID, NetworkIdentity interactionId) {
		unitID.GetComponent<Unit> ().SetFocus (interactionId.GetComponent<Interactable> ());
	}

	[Command]
	public void CmdAttack (NetworkIdentity targetIdentity, NetworkIdentity myIdentity) {
		targetIdentity.GetComponent<CharStats> ().TakeDamage (myIdentity.GetComponent<CharStats> ().damage.GetValue ());
	}
	#endregion
	/////////////////////////////////// FUNCTIONS
	//SELECTION FUNCTIONS
	#region "Unit Selection"
	public void DeselectAll (BaseEventData eventData) { //if(!isLocalPlayer)return;
		CleanSelection (selectedUnits);
		foreach (GameObject unit in selectedUnits) {

			unit.GetComponent<UnitSelectable> ().OnDeselect (eventData);
		}
		selectedUnits.Clear ();
	}
	public void CleanSelection (List<GameObject> sUnits) {
		for (int i = sUnits.Count - 1; i >= 0; i--) {
			if (sUnits[i] == null) {
				sUnits.RemoveAt (i);
			}
		}
		selectedUnits = sUnits;
	}
	#endregion
	#region "movement"
	float offsetSize = 2;
	int perRow = 6;

	void moveUnits (Vector3 hit) {

		StartCoroutine (coMove (selectedUnits, hit));
	}
	IEnumerator coMove (List<GameObject> gos, Vector3 hit) {
		int rowCount = (-1) * (perRow / 2), colCount = 0;
		for (int i = 0; i < gos.Count; i++) {
			float x, z;
			if (rowCount >= perRow / 2) {
				rowCount = (-1) * (perRow / 2);
				colCount++;
			}

			x = offsetSize * rowCount;
			z = offsetSize * colCount;

			Vector3 offset = new Vector3 (x, 0, z);
			if (gos[i] == null) {
				continue;
			}
			gos[i].GetComponent<Unit> ().RemoveFocus ();
			gos[i].GetComponent<Unit> ().MoveToPoint (hit + offset);

			rowCount++;
		}
		yield return null;
	}
	#endregion
	#region "Spawn Handler"
	//Move to new script
	void spawnUnit (int spawnIndex) {
		CmdSpawnObject (spawnIndex);
	}
	//////////////////////////////////// COMMANDS
	// Commands are special functions that ONLY get executed on the server.
	public GameObject myUnit;
	[Command]
	void CmdSpawnObject (int spawnableObjectIndex) {
		//We are guaranteed to be on the severe right now.

		//selectedUnits.Add(go);

		//Now that the object exists on the server, propagate it to all
		//the clients(and also wire up the Network Identity)

		//Spawn Unit and Assign to a Player
		GameObject go = NetworkManager.singleton.spawnPrefabs[spawnableObjectIndex];
		go = Instantiate (go);
		go.GetComponent<Unit> ().team = team;
		NetworkIdentity ni = go.GetComponent<NetworkIdentity> ();
		Debug.Log ("Player Object :: --Spawning Unit");
		NetworkServer.Spawn (go);
		bool ToF = go.GetComponent<NetworkIdentity> ().AssignClientAuthority (GetComponent<NetworkIdentity> ().connectionToClient);

		Debug.Log ("Player Object :: --Unit Spawned : " + ToF);
		RpcAssignObject (ni, team);
	}

	//DYNAMIC IS NOT FEASIBLE... MUST ACCESS GLOBAL VAR TO WORK

	[ClientRpc]
	void RpcAssignObject (NetworkIdentity id, int t) {
		//	if(!isLocalPlayer)return;

		//Debug.Log(id.netId.Value);
		GameObject spawnHolder = id.gameObject;
		spawnHolder.GetComponent<Unit> ().team = team;
		spawnHolder.name = team + " - unit - " + netId;
		spawnHolder.GetComponent<Unit> ().graphics.GetComponent<MeshRenderer> ().materials[0].color = selectedColor[t - 1];

		if (spawnHolder.GetComponent<CharStats> () != null)
			spawnHolder.GetComponent<CharStats> ().netPlayer = this;
		if (spawnHolder.GetComponent<UnitCombat> () != null)
			spawnHolder.GetComponent<UnitCombat> ().netPlayer = this;

		spawnHolder.AddComponent<UnitSelectable> ();
		spawnHolder.GetComponent<UnitSelectable> ().playerObject = this;
		myUnits.Add (spawnHolder);

	}

	#endregion
	#region "DeSpawning"
	public void despawnUnit (GameObject go) {
		Debug.Log ("ABOUT TO UNSPAWN : " + go.name);

		Debug.Log ("Unspawning NID" + GetComponent<NetworkIdentity> ().netId);

		CmdDeSpawnObject (go.GetComponent<NetworkIdentity> ());
	}

	[Command]
	void CmdDeSpawnObject (NetworkIdentity id) {
		Debug.Log ("CmdDespawnObject");

		NetworkServer.Destroy (id.gameObject);

	}

	#endregion

}