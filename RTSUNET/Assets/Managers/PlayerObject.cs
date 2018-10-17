using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.Networking;

public class PlayerObject : NetworkBehaviour {
	#region "SYSTEMS"
	public BuildingSystem BuildSys;
	public UnitSystem UnitSys;
	#endregion

	public List<GameObject> selectedUnits = new List<GameObject> ();
	public List<GameObject> myUnits = new List<GameObject> ();
	public List<GameObject> myBuildings = new List<GameObject> ();
	public Camera cam;
	[SerializeField]
	BuildingFactionGroups buildingFactionGroups;
	[SerializeField]
	UnitFactionGroup unitFactionGroup;
	float ang;
	public LayerMask movementMask;
	//passed variables
	[SyncVar]
	public int playerId;
	[SyncVar]
	public int team;
	[SyncVar]
	public int factionIndex;
	[SyncVar]
	public string playerName;
	[SyncVar]
	public int colorIndex;
	public int manna;

	// Use this for initialization
	void Start () {

		BuildSys = GetComponent<BuildingSystem> ();
		if (BuildSys == null) { Debug.LogError ("Building System not found on player object"); } else {
			BuildSys.buildingGroups = buildingFactionGroups.buildingFactionDictionary[(Factions) factionIndex];

		}

		UnitSys = GetComponent<UnitSystem> ();
		if (UnitSys == null) { Debug.LogError ("Unit System not found on player object"); } else {
			UnitSys.unitGroup = unitFactionGroup.factionUnitDictionary[(Factions) factionIndex];

		}

		if (isLocalPlayer == false) {
			//This object belongs to another player.
			return;
		}
		if (DragSelectionHandler.singleton.playerObject == null) {
			DragSelectionHandler.singleton.AssignPlayerObject (this);
		}
		//Setup Systems



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
			UnitSys.spawnUnit (0, new Vector3 (17f, -1f, 25f), Quaternion.identity);

		}
		if (Input.GetKeyDown (KeyCode.M)) {
			UnitSys.spawnUnit (1, new Vector3 (17f, -1f, 25f), Quaternion.identity);
		}
		if (Input.GetKeyDown (KeyCode.B)) {
			BuildSys.ToggleBuildMode ();
		}
		if (BuildSys.buildMode) {
			BuildSys.BuildingControl ();
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

								unit.GetComponent<UnitNew> ().SetFocus (interactable);
							}
						return;

					}
				}

				if (Physics.Raycast (ray, out hit, 10000, movementMask)) {
					moveUnits (hit.point);
				}

			}
	}

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
			gos[i].GetComponent<UnitNew> ().RemoveFocus ();
			gos[i].GetComponent<UnitNew> ().MoveToPoint (hit + offset);

			rowCount++;
		}
		yield return null;
	}
	#endregion

}