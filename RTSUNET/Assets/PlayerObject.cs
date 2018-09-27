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
			spawnUnit ();
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
				CmdSpawnBuilding (selectedCreateBuildingIndex, mouseWorldPointPosition);
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

					Character interactable = hit.collider.GetComponent<Character> ();
					//.Log("JORI JORI AJA AJA " + hit.collider.name);
					if (interactable != null) {
						if (interactable.GetComponent<Unit> ().team == team) {
							//hard coded stuff;
							Debug.Log ("Oops Same team");
							return;
						}
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
		if (false) {
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
	[Command]
	void CmdSpawnBuilding (int spawnableIndex, Vector3 position) {

		GameObject go = NetworkManager.singleton.spawnPrefabs[spawnableIndex];
		go = Instantiate (go, position, Quaternion.identity);
		Renderer[] renderers = go.GetComponent<BuildingUnit> ().teamColoredGfx;
		if (renderers.Length > 0)
			for (int i = 0; i < renderers.Length; i++) {
				renderers[i].material.color = selectedColor[team - 1];
			}

		
		Vector3 navMeshObstacleSize = go.GetComponent<BoxCollider> ().size;
	

		Destroy (go.GetComponent<Rigidbody> ());
		//Destroy(go.GetComponent<BoxCollider>());
		Destroy (go.GetComponent<BuildingCreationTrigger> ());

		go.GetComponent<BuildingUnit> ().team = team;
		NetworkIdentity ni = go.GetComponent<NetworkIdentity> ();
		Debug.Log ("Player Object :: --Spawning Unit");
		NetworkServer.Spawn (go);
		bool ToF = go.GetComponent<NetworkIdentity> ().AssignClientAuthority (GetComponent<NetworkIdentity> ().connectionToClient);

		Debug.Log ("Player Object :: --Unit Spawned : " + ToF);
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
		Renderer[] renderers = spawnHolder.GetComponent<BuildingUnit> ().teamColoredGfx;
		if (renderers.Length > 0)
			for (int i = 0; i < renderers.Length; i++) {
				renderers[i].material.color = selectedColor[t - 1];
			}
		myBuildings.Add (spawnHolder);

	}
	#endregion
	#region "Interactions"
	[Command]
	void CmdFocus (NetworkIdentity unitID, NetworkIdentity interactionId) {
		unitID.GetComponent<Unit> ().SetFocus (interactionId.GetComponent<Character> ());
		RpcFocus (unitID, interactionId);
	}

	[ClientRpc]
	void RpcFocus (NetworkIdentity unitID, NetworkIdentity interactionId) {
		unitID.GetComponent<Unit> ().SetFocus (interactionId.GetComponent<Character> ());
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
	void spawnUnit () {
		CmdSpawnObject (0);
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

		spawnHolder.name = team + " - unit - " + netId;
		spawnHolder.GetComponent<Unit> ().graphics.GetComponent<MeshRenderer> ().materials[0].color = selectedColor[t - 1];

		spawnHolder.GetComponent<CharStats> ().netPlayer = this;
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