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

	public UIGameManager uiGameManager;
	#endregion

	public List<GameObject> selectedUnits = new List<GameObject> ();
	public List<GameObject> myUnits = new List<GameObject> ();
	public List<GameObject> myBuildings = new List<GameObject> ();
	public Camera cam;
	[SerializeField]
	BuildingFactionGroups buildingFactionGroups;
	[SerializeField]
	UnitFactionGroup unitFactionGroup;
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
	[SyncVar]
	public int baseNo;
	public int manna;

	public static PlayerObject singleton;
	// Use this for initialization
	void Awake()
	{
		Debug.Log("PlayerObject awake");


	}

	void Start () {

	if(isLocalPlayer){
		Debug.Log("Assigning Player singleton");
			singleton = this;
	}

		GameObject parentObject = GameObject.FindWithTag("Players");	
		this.transform.SetParent(parentObject.transform);


	//Awake^

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


		if (UIGameManager.singleton != null) {
			uiGameManager = UIGameManager.singleton;
			uiGameManager.Initialize (this);
		}
		//Setup Systems

		gameObject.name = gameObject.name + "NID" + GetComponent<NetworkIdentity> ().netId;

		Debug.Log ("PlayerObject::Start -- Spawning my own personal Unit");

		cam = Camera.main;


	}
	string DebugText (GameObject inspect) {
		string s = "Debug:";
		//running on what;
		s += isServer? " Server": " Client";
		//selected
		s += "\nSelected Unit Count:" + selectedUnits.Count;
		//one unit selected
		if (inspect) {
			s += "\nSelected:" + inspect.name;
			try {
				s += "\n • ID:" + inspect.GetComponent<NetworkBehaviour> ().netId;
				s += "\n • Is Able:" + ((inspect.GetComponent<MonoUnitFramework> () != null) ? "Yes | Count:"+ inspect.GetComponent<MonoUnitFramework> ().abilities.Count :"No");
				s += "\n • Is Damageable:" + ((inspect.GetComponent<Damageable> () != null) ? "Yes | Health:" + inspect.GetComponent<Damageable> ().currentHealth: "No");
				s += "\n • Is Attacker:" + ((inspect.GetComponent<Attacker> () != null) ? "Yes | Skill(s):" + inspect.GetComponent<Attacker> ().skills.Count : "No");
			} catch (System.Exception) {
				s += "\nNot Unit";
			}
		}
		return s;

	}

	void Update () {
		//Remember: Update runs on EVERYONE's computer, wether or not they own this
		//particular player object.
		if (isLocalPlayer == false) {
			return;
		}
		
		//DEBUGGING
		Ray _ray = cam.ScreenPointToRay (Input.mousePosition);
		RaycastHit _hit;
		if (Physics.Raycast (_ray, out _hit, 100000)) {
		if (UIGameManager.singleton != null)
			if (UIGameManager.singleton.debugTxt != null)
				UIGameManager.singleton.debugTxt.text = DebugText (_hit.collider.gameObject);
		}
		//Spawns Unit DEBUG ONLY
		if (Input.GetKeyDown (KeyCode.Space)) {
			UnitSys.spawnUnit (0, new Vector3 (17f, -0.5f, 25f), Quaternion.identity);

		}
		if (Input.GetKeyDown (KeyCode.M)) {
			UnitSys.spawnUnit (1, new Vector3 (17f, -0.5f, 25f), Quaternion.identity);
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

					MonoUnitFramework interactable = hit.collider.GetComponent<MonoUnitFramework> ();
					//.Log("JORI JORI AJA AJA " + hit.collider.name);
					if (interactable != null) {

						if (myUnits.Count > 0)
							foreach (GameObject unit in selectedUnits) {
								//TO BE REMOVED IF FOUNF SOLUTION ON NOT DELETEiNG OBJECTS
								if (unit == null) {
									selectedUnits.Remove (unit);
									continue;
								}
								MonoUnit monoUnit = unit.GetComponent<MonoUnit> ();
								MonoSkill skill = defaultSkill (monoUnit);
								Debug.Log ("Applying default skill " + skill);
								if (skill == null) continue;
								monoUnit.SetFocus (interactable, skill);
							}
						return;

					}
				}

				if (Physics.Raycast (ray, out hit, 10000, movementMask)) {
					moveUnits (hit.point);
				}

			}

		//DEBUGGGG
		if (selectedUnits.Count == 1 && debugToggleForGameCommands) {
			uiGameManager.commandsHandler.ShowAbilities (selectedUnits[0].GetComponent<MonoUnitFramework> ().abilities);
			debugToggleForGameCommands = false;
		}
	}
	bool debugToggleForGameCommands = true;

	MonoSkill defaultSkill (MonoUnitFramework monoUnit) {
		List<MonoAbility> abilities = monoUnit.abilities;
		foreach (MonoAbility ability in abilities) {
			List<MonoSkill> skills = ability.skills;
			if (skills.Count <= 0) continue;
			else return skills[0]; //returns first skill on list

		}
		return null;
	}
	#region "Unit Selection"
	public void DeselectAll (BaseEventData eventData) { //if(!isLocalPlayer)return;
		//DEBUGGGG
		//uiGameManager.commandsHandler.ClearAbilities ();
		debugToggleForGameCommands = true;
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

	public void RemoveUnit(GameObject go){
		if(selectedUnits.Contains(go))
		selectedUnits.Remove(go);
		if(myUnits.Contains(go))
		myUnits.Remove(go);

		if(myUnits.Contains(null))
		Debug.Log("There is null");

		CleanSelection(selectedUnits);

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
			gos[i].GetComponent<MonoUnit> ().RemoveFocus ();
			gos[i].GetComponent<MonoUnit> ().MoveToPoint (hit + offset);

			rowCount++;
		}
		yield return null;
	}
	#endregion
	

}