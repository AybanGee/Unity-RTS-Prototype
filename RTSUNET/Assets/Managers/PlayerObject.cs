using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.Networking;

public class PlayerObject : NetworkBehaviour {
	#region "SYSTEMS"
	public BuildingSystem BuildSys;
	#endregion

	public List<GameObject> selectedUnits = new List<GameObject> ();
	public List<GameObject> myUnits = new List<GameObject> ();
	public List<GameObject> myBuildings = new List<GameObject> ();
	public Camera cam;
	public List<Color> selectedColor = new List<Color> ();
	float ang;
	public LayerMask movementMask;
	//passed variables
	[SyncVar]
	public int playerId;
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
		BuildSys = GetComponent<BuildingSystem>();
		if(BuildSys == null){Debug.LogError("Building System not found on player object"); }
		gameObject.name = gameObject.name + "NID" + GetComponent<NetworkIdentity> ().netId;

		Debug.Log ("PlayerObject::Start -- Spawning my own personal Unit");

		cam = Camera.main;

	}
	//builder vars
	
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
			BuildSys.ToggleBuildMode ();
		}
		if (BuildSys.buildMode) {
			BuildSys.BuildingControl();
		} else
			//Move selected units to point
			if (Input.GetMouseButtonDown (1)) {
				//Slow script
				if (selectedUnits.Count <= 0) return;
				Ray ray = cam.ScreenPointToRay (Input.mousePosition);
				RaycastHit hit;

				//There should be at least 1 selected unit.

				if (Physics.Raycast (ray, out hit, 10000)) {

					Interactable interactable = hit.collider.GetComponent<Interactable> ();
					//.Log("JORI JORI AJA AJA " + hit.collider.name);
					if (interactable != null) {
						

							if (myUnits.Count > 0)
								foreach (GameObject unit in selectedUnits) {
									//TO BE REMOVED IF FOUNF SOLUTION ON NOT DELETEiNG OBJECTS
									if (unit == null) {
										selectedUnits.Remove (unit);
										continue;
									}
									

									unit.GetComponent<Unit> ().SetFocus (interactable);
								}
							return;

					}
				}

				
				if (Physics.Raycast (ray, out hit, 10000, movementMask)) {
					moveUnits (hit.point);
				}

			}
	}
	
	// #region "Interactions"
	// [Command]
	// void CmdFocus (NetworkIdentity unitID, NetworkIdentity interactionId) {
	// 	unitID.GetComponent<Unit> ().SetFocus (interactionId.GetComponent<Interactable> ());
	// 	RpcFocus (unitID, interactionId);
	// }

	// [ClientRpc]
	// void RpcFocus (NetworkIdentity unitID, NetworkIdentity interactionId) {
	// 	unitID.GetComponent<Unit> ().SetFocus (interactionId.GetComponent<Interactable> ());
	// }

	// #endregion
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
		Unit unit = go.GetComponent<Unit> ();
		unit.team = team;
		unit.playerObject = this;
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
		Unit unit = spawnHolder.GetComponent<Unit> ();
		unit.team = team;
		unit.playerObject = this;
		spawnHolder.name = team + " - unit" + spawnHolder.GetComponent<NetworkIdentity>().netId;
		spawnHolder.GetComponent<Unit> ().graphics.GetComponent<MeshRenderer> ().materials[0].color = selectedColor[t - 1];

		if (spawnHolder.GetComponent<CharStats> () != null)
			spawnHolder.GetComponent<CharStats> ().netPlayer = this;

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