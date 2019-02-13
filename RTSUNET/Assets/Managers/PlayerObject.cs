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
	BuildingFactionGroups buildingFactionGroups, buildableGroup;
	[SerializeField]
	UnitFactionGroup unitFactionGroup, townhallUnitGroup, barracksUnitGroup;
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
	[SyncVar]
	public bool mapIsLoaded = false;
	[SyncVar]
	public bool startNow = false;
	LoadMap mapLoader;
	public List<PlayerObject> players = new List<PlayerObject> ();
	public GameObject camGroup;
	BaseHolder baseholder;
	public List<PlayerObject> playingPlayers = new List<PlayerObject> ();
	public List<PlayerObject> defeatedPlayers = new List<PlayerObject> ();
	[SyncVar]
	public bool gameIsDone = false;
	[SyncVar]
	public bool isDefeated = false;
	[SyncVar]
	public bool isWinner = false;
	public Coroutine gameChecker;
	public GameObject townhall;
	public bool checkerWait = false;

	bool buildingsAreShown = false;

	public static PlayerObject singleton;

	public bool isSinglelPlayer = false;

	public bool isDummyPlayer = false;

	public QuestFactionGroup questFactionGroup;

	[HideInInspector]
	public QuestManager QM;
	[HideInInspector]
	public QuestEventReciever QER;
	// Use this for initialization
	void Awake () {
		Debug.Log ("PlayerObject awake");
	}

	void Start () {
		/* 		if (!isDummyPlayer) {
					if (LobbyManager.singleton.GetComponent<LobbyManager> ().isSinglePlayer) {

						// TODO setup own variables here
					
						CmdSpawnOpponent ();
					}
				}
		 */

		if (isSinglelPlayer) {
			QM = gameObject.AddComponent<QuestManager> ();
			QM.quests = questFactionGroup.questUnitDictionary[(Factions) factionIndex].quests;
			QM.PO = this;
			QER = gameObject.AddComponent<QuestEventReciever> ();

			//if(uiGameManager.questUI != null)
				//uiGameManager.questUI.gameObject.SetActive (true);
		}
		if (isLocalPlayer) {
			Debug.Log ("Assigning Player singleton");
			singleton = this;
		}

		GameObject parentObject = GameObject.FindWithTag ("Players");
		this.transform.SetParent (parentObject.transform);

		//Awake^

		BuildSys = GetComponent<BuildingSystem> ();
		if (BuildSys == null) { Debug.LogError ("Building System not found on player object"); } else {
			BuildSys.buildingGroups = buildingFactionGroups.buildingFactionDictionary[(Factions) factionIndex];
			BuildSys.buildableGroup = buildableGroup.buildingFactionDictionary[(Factions) factionIndex];

		}

		UnitSys = GetComponent<UnitSystem> ();
		if (UnitSys == null) { Debug.LogError ("Unit System not found on player object"); } else {
			UnitSys.unitGroup = unitFactionGroup.factionUnitDictionary[(Factions) factionIndex];
			UnitSys.thGroup = townhallUnitGroup.factionUnitDictionary[(Factions) factionIndex];
			UnitSys.bGroup = barracksUnitGroup.factionUnitDictionary[(Factions) factionIndex];

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

		mapLoader = LoadMap.singleton;

		StartCoroutine (WaitForMap ());
		if (isServer)
			StartCoroutine (GetAllPlayerObjects ());

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
				s += "\n • Is Able:" + ((inspect.GetComponent<MonoUnitFramework> () != null) ? "Yes | Count:" + inspect.GetComponent<MonoUnitFramework> ().abilities.Count : "No");
				s += "\n • Is Damageable:" + ((inspect.GetComponent<Damageable> () != null) ? "Yes | Health:" + inspect.GetComponent<Damageable> ().currentHealth : "No");
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
		//If player is already Defeated return
		if (isDefeated == true) {
			DeselectAll (new BaseEventData (EventSystem.current));
			return;
		}
		if (Input.GetKeyDown (KeyCode.O)) {
			SwapPOTeams (true);
		}
		if (Input.GetKeyDown (KeyCode.I)) {
			SwapPOTeams (false);
		}

		//Surrender Debug
		if (Input.GetKeyDown (KeyCode.P)) {
			SetDefeatStatus (!isDefeated);
		}

		if (Input.GetKeyDown (KeyCode.T)) {
			ShowNotice ("Debug Notice");
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
								MonoUnitFramework monoUnit = unit.GetComponent<MonoUnitFramework> ();
								MonoSkill skill = monoUnit.defaultUnitSkill ();
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

		uiGameManager.manaHolder.text = manna.ToString ("N0");
		if (selectedUnits.Count <= 0 && buildingsAreShown == false) {
			UpdateUI ();
		}
	}

	void LateUpdate () {
		/* 		string output = "townhall : " + townhall;
				output += "\n isDefeated: " + isDefeated;
				output += "\n checkerWait: " + checkerWait;
				Debug.Log (output);

				if (townhall == null && isDefeated == false && checkerWait == true) {
					isDefeated = true;
					DeselectAll (new BaseEventData (EventSystem.current));
					Debug.Log ("You are Defeated");
				} */
	}
	bool debugToggleForGameCommands = true;

	#region "Unit Selection"

	public void UpdateUI () {
		buildingsAreShown = false;

		//For Single Selection
		if (selectedUnits.Count == 1 && debugToggleForGameCommands) {
			debugToggleForGameCommands = false;
			//Check if selected Unit is a Building
			if (selectedUnits[0].GetComponent<MonoBuilding> () != null) {
				Debug.Log ("MonoBuilding Exists");
				//Check if Building can spawn Units
				if (selectedUnits[0].GetComponent<QueueingSystem> () != null) {
					Debug.Log ("Q system : " + selectedUnits[0].GetComponent<QueueingSystem> ());
					uiGameManager.commandsHandler.ShowAbilities (selectedUnits[0].GetComponent<QueueingSystem> ().spawnableUnits, selectedUnits[0].GetComponent<QueueingSystem> ());
					uiGameManager.commandsHandler.ShowProcessQueue (selectedUnits[0].GetComponent<QueueingSystem> ().spawnQueue, selectedUnits[0].GetComponent<QueueingSystem> ());

				} else {
					//Display Other Building Abilities Here
					uiGameManager.commandsHandler.ShowAbilitiesTowers (selectedUnits[0].GetComponent<MonoUnitFramework> ().abilities);
				}
			}

			//Check if selected Unit is an actual Unit
			if (selectedUnits[0].GetComponent<MonoUnit> () != null) {
				uiGameManager.commandsHandler.ShowAbilities (selectedUnits[0].GetComponent<MonoUnitFramework> ().abilities);
			}
		}

		//Check if multiple Units are Selected
		if (selectedUnits.Count >= 2) {
			uiGameManager.commandsHandler.ShowMultiUnit (selectedUnits);
		}

		//Display Buildable Buildings
		if (selectedUnits.Count == 0) {
			uiGameManager.commandsHandler.ShowBuildings (BuildSys);
			buildingsAreShown = true;
		}
	}
	public void DeselectAll (BaseEventData eventData) { //if(!isLocalPlayer)return;
		//DEBUGGGG
		//uiGameManager.commandsHandler.ClearAbilities ();
		uiGameManager.commandsHandler.ClearAbilities ();
		uiGameManager.commandsHandler.ResetQueueDisplay ();

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

	public void RemoveUnit (GameObject go) {
		if (selectedUnits.Contains (go))
			selectedUnits.Remove (go);
		if (myUnits.Contains (go))
			myUnits.Remove (go);

		if (myUnits.Contains (null))
			Debug.Log ("There is null");

		CleanSelection (selectedUnits);

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

	#region MapIsLoaded
	//For Server
	//Get all player objects
	public IEnumerator GetAllPlayerObjects () {
		LobbyManager LM = LobbyManager.singleton.GetComponent<LobbyManager> ();
		Transform playerTr = transform.parent;

		//Debug.Log("Get all players start");
		while (players.Count != LM.lobbyPlayers.Count) {
			//Debug.Log("Get all players loop");

			players = new List<PlayerObject> ();
			foreach (Transform p in playerTr) {
				//Debug.Log("Get all players for each loop");

				PlayerObject po = p.GetComponent<PlayerObject> ();
				if (po != null) {
					if (!players.Contains (po))
						players.Add (po);
				}
			}

			yield return null;
		}
		//All players have been found
		Debug.Log ("all have been found : " + players.Count);
		//Add to list of playing players

		if (isServer)
			playingPlayers = players;

		StartCoroutine (CheckAllPlayersAreLoaded ());
		yield return null;
	}

	IEnumerator CheckAllPlayersAreLoaded () {
		bool allReady = false;

		while (!allReady) {
			allReady = true;
			foreach (PlayerObject p in players) {
				if (p.mapIsLoaded != true) {
					allReady = false;
				}
			}
			yield return null;
		}
		foreach (PlayerObject p in players) {
			p.CmdIAmReady (p.GetComponent<NetworkIdentity> ());

			//p.CmdMoveCamToBase();
		}
	}

	[Command]
	void CmdIAmReady (NetworkIdentity id) {
		id.gameObject.GetComponent<PlayerObject> ().startNow = true;
		RpcIAmReady (id);
	}

	[ClientRpc]
	void RpcIAmReady (NetworkIdentity id) {
		id.gameObject.GetComponent<PlayerObject> ().startNow = true;
	}
	//For Client
	IEnumerator WaitForMap () {
		Debug.Log ("Waiting for map start");

		while (mapLoader.isFinishedLoading != true) {
			Debug.Log ("Waiting for map loop");

			yield return null;
		}
		if (mapLoader.isFinishedLoading == true) {
			Debug.Log ("map is Loaded : " + mapLoader.isFinishedLoading);

			SetLoadStatus (true);
		}
		StartCoroutine (WaitForStart ());
		yield return null;

	}

	IEnumerator WaitForStart () {
		Debug.Log ("Waiting for start");

		while (startNow != true) {
			Debug.Log ("Waiting for start loop");

			yield return null;
		}

		if (startNow == true) {
			//move cam to base
			Debug.Log ("Is Local Player : " + isLocalPlayer);
			if (isLocalPlayer) {
				mapLoader = LoadMap.singleton;
				Debug.Log ("mapLoader : " + mapLoader);
				mapLoader.moveCamToBase ();
			}
			Debug.Log ("start Now");

		}
		yield return null;

		//Start Game Checker
		if (isServer) {
			Debug.Log ("GameChecker :: Entering : Waiting 10 Sec.");
			playingPlayers = players;
			Debug.Log ("GameChecker :: Playing : " + playingPlayers.Count);

			yield return new WaitForSeconds (10);
			if (gameChecker == null && isServer && !isSinglelPlayer)
				gameChecker = StartCoroutine (GameChecker ());
		}
	}

	public void SetLoadStatus (bool isReady) {
		CmdSetLoaded (isReady);
		//lM = LoadMap.singleton;	
	}

	[Command]
	void CmdSetLoaded (bool isReady) {
		mapIsLoaded = isReady;
		RpcSetLoaded (mapIsLoaded);
	}

	[ClientRpc]
	void RpcSetLoaded (bool isReady) {
		mapIsLoaded = isReady;
	}
	//=====
	public void SetStartStatus (bool isReady) {
		CmdSetStartStatus (isReady);
	}

	[Command]
	void CmdSetStartStatus (bool isReady) {
		startNow = isReady;
		RpcSetStartStatus (startNow);
	}

	[ClientRpc]
	void RpcSetStartStatus (bool isReady) {
		startNow = isReady;
	}
	//=====

	/* 		[Command]
			public void CmdMoveCamToBase(){
				Debug.Log("CMD move cam ");
				LoadMap lM = LoadMap.singleton;	
				lM.moveCamToBase();
				RpcMoveCamToBase();
			}
			[ClientRpc]
			public void RpcMoveCamToBase(){
				LoadMap lM = LoadMap.singleton;	
				lM.moveCamToBase();
			} */
	#endregion
	#region Game Checker
	//Server
	IEnumerator GameChecker () {
		Debug.Log ("GameChecker :: Entering : Waiting 10 Sec.");
		yield return new WaitForSeconds (10);

		while (gameIsDone == false) {
			UpdatePlayingList ();
			CheckForWinner ();

			yield return null;
		}

		Debug.Log ("PO :: GameChecker : Game is Done!");
		SetWinner ();
	}

	public void CheckForWinner () {
		bool flag = true;
		for (int i = playingPlayers.Count - 1; i >= 0; i--) {
			if (playingPlayers[0].team != playingPlayers[i].team) {
				flag = false;
			}
		}
		gameIsDone = flag;
	}

	public void SetWinner () {
		for (int i = players.Count - 1; i >= 0; i--) {
			if (players[i].team == playingPlayers[0].team) {
				players[i].SetWinnerStatus (true);
				players[i].SetDefeatStatus (true);
			}

			players[i].SetGameStatus (true);

		}
	}

	void UpdatePlayingList () {
		for (int i = playingPlayers.Count - 1; i >= 0; i--) {
			if (playingPlayers[i].isDefeated == true) {
				playingPlayers.RemoveAt (i);
			}
		}
	}

	//Commands and RPC's
	public void SetDefeatStatus (bool input) {
		Debug.Log ("SetDefeatStatus :: input : " + input);
		CmdSetDefeatStatus (input);

		uiGameManager.ShowSpectatorScreen ();
	}

	[Command]
	void CmdSetDefeatStatus (bool input) {
		Debug.Log ("CMD :: SetDefeatStatus :: input : " + input);

		isDefeated = input;
		RpcSetDefeatStatus (isDefeated);
	}

	[ClientRpc]
	void RpcSetDefeatStatus (bool input) {
		Debug.Log ("RPC :: SetDefeatStatus :: input : " + input);

		isDefeated = input;
	}

	public void SetWinnerStatus (bool input) {
		CmdSetWinnerStatus (input);

		uiGameManager.ShowWinScreen (input);
	}

	[Command]
	void CmdSetWinnerStatus (bool input) {
		isWinner = input;
		RpcSetWinnerStatus (isWinner);
	}

	[ClientRpc]
	void RpcSetWinnerStatus (bool input) {
		isWinner = input;
	}

	public void SetGameStatus (bool input) {
		CmdSetGameStatus (input);

		uiGameManager.ShowWinScreen (isWinner);
	}

	[Command]
	void CmdSetGameStatus (bool input) {
		gameIsDone = input;
		RpcSetGameStatus (gameIsDone);
	}

	[ClientRpc]
	void RpcSetGameStatus (bool input) {
		gameIsDone = input;
	}
	//Client

	void OnDefeat () {
		SetDefeatStatus (true);
		//Show Spectator UI
	}
	void OnGameIsDone () {
		//Show End UI
	}
	#endregion

	public void ShowNotice (string inputText) {
		GameObject notice = Instantiate (uiGameManager.notice, uiGameManager.winScreen.transform.parent);
		notice.GetComponent<NoticeAnimator> ().textHolder.text = inputText;
	}

	#region SinglePlayers

	public override void OnStartAuthority () {
		Debug.LogWarning ("<color:red>AuthStart</color>");
		if (!isDummyPlayer) {
			if (LobbyManager.singleton.GetComponent<LobbyManager> ().isSinglePlayer) {

				// TODO setup own variables here
				if (hasAuthority)
					Debug.LogWarning ("I have authority and by the power vested in me of the department of education division of cavite. I now spawn thee Sir SpawnOpponent as spawn in the network this day of february 12 2019");
				StartCoroutine (WaitForReady ());
			}
		}

	}
	bool isSwapped = false;
	//Call after move camera to base if singleplayer
	public void SpawnEnemyBuildings () {
		SwapPOTeams (true);
		//wait until teams are swapped

		//spawn Buildings Here

		Debug.Log ("PlayerObject :: SpawnEnemyBuildings : isSwapped : " + isSwapped);
		if (isSwapped)
			transform.parent.GetChild (1).GetComponent<EnemySpawn> ().spawnObjects (this);

		//SwapPOTeams(false);
	}

	int temp_team;
	int temp_factionIndex;
	int temp_colorIndex;
	public void SwapPOTeams (bool toEnemy) {
		if (toEnemy) {
			temp_team = team;
			temp_factionIndex = factionIndex;
			temp_colorIndex = colorIndex;

			team = team + 1;
			factionIndex = (factionIndex == 0) ? 1 : 0;
			colorIndex = colorIndex + 3;

			BuildSys.buildingGroups = buildingFactionGroups.buildingFactionDictionary[(Factions) factionIndex];
			BuildSys.buildableGroup = buildableGroup.buildingFactionDictionary[(Factions) factionIndex];

		} else {
			team = temp_team;
			factionIndex = temp_factionIndex;
			colorIndex = temp_colorIndex;

			BuildSys.buildingGroups = buildingFactionGroups.buildingFactionDictionary[(Factions) factionIndex];
			BuildSys.buildableGroup = buildableGroup.buildingFactionDictionary[(Factions) factionIndex];
		}
		Debug.Log ("PlayerObject :: TeamSwap : Buildables : " + BuildSys.buildableGroup);

		isSwapped = !isSwapped;

	}

	[Command] public void CmdSpawnOpponent () {
		GameObject go = NetworkManager.singleton.spawnPrefabs[5]; //sets spawnable index of the single players
		go = Instantiate (go, Vector3.zero, Quaternion.identity, this.transform);
		PlayerObject opponentPO = go.GetComponent<PlayerObject> ();
		//Setting up variables of opponentPO
		opponentPO.isDummyPlayer = true;
		opponentPO.team = team + 1;
		opponentPO.factionIndex = (factionIndex == 0) ? 1 : 0;
		opponentPO.colorIndex = colorIndex + 3;
		opponentPO.playerId = (int) playerId;

		/* [SyncVar]
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
			public int manna; */
		Debug.LogError ("Now we will add authority to the enemy object");
		if (GetComponent<NetworkIdentity> ().connectionToClient != null)
			Debug.LogError ("Network identity does not equal null");
		NetworkServer.SpawnWithClientAuthority (go, GetComponent<NetworkIdentity> ().connectionToClient);
		go.GetComponent<NetworkIdentity> ().AssignClientAuthority (GetComponent<NetworkIdentity> ().connectionToClient);

		// go.GetComponent<NetworkIdentity> ().AssignClientAuthority (GetComponent<NetworkIdentity> ().connectionToClient);

		/* 		EnemySpawn es = go.GetComponent<EnemySpawn>();
				es.holderSearch = es.StartCoroutine(es.FindingBaseLocation()); */

	}

	IEnumerator WaitForReady ( /* GameObject _go */ ) {
		while (!connectionToClient.isReady) {
			yield return new WaitForSeconds (0.25f);
		}
		Debug.LogWarning ("Connection to client is ready!");
		CmdSpawnOpponent ();
		/* 	NetworkServer.SpawnWithClientAuthority (_go, connectionToClient);
			bool ToF = _go.GetComponent<NetworkIdentity> ().AssignClientAuthority (GetComponent<NetworkIdentity> ().connectionToClient);

			_go.SetActive (true); */
		yield return null;
	}
	#endregion
}