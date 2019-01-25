using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class UnitSystem : NetworkBehaviour {
	public int UnitSpawnIndex = 4;
	public Vector3 spawnPoint;
	[HideInInspector]
	public UnitGroup unitGroup,thGroup,bGroup;
	PlayerObject PO;
	

	void Awake () {
		PO = GetComponent<PlayerObject> ();
	}
	//Move to new script
	public void spawnUnit (int spawnIndex, Vector3 pos, Quaternion rot) {
		CmdSpawnObject (spawnIndex, pos, rot);

	}

	[Command]
	public void CmdSpawnObject (int spawnableObjectIndex, Vector3 position, Quaternion rotation) {
		GameObject go = NetworkManager.singleton.spawnPrefabs[UnitSpawnIndex];
		PlayerUnit playerUnit =  unitGroup.units[spawnableObjectIndex];

		go = Instantiate (go, position, rotation,this.transform);
		MonoUnitFramework muf = go.GetComponent<MonoUnitFramework>();
		muf.playerUnit = playerUnit;
		muf.PO = PO;
		

		playerUnit.Initialize(go);	
		//ClientScene.RegisterPrefab(go);	
			
		
		//playerUnit.Initialize(go);
		//initialize abilities

		NetworkIdentity ni = go.GetComponent<NetworkIdentity> ();
		NetworkServer.SpawnWithClientAuthority (go, connectionToClient);

		RpcAssignObject (ni, spawnableObjectIndex);
		//MonoUnit unit = go.GetComponent<MonoUnit> ();
		// go.GetComponent<Unit> ().graphics.GetComponent<Renderer> ().material.color = LobbyManager.singleton.GetComponent<LobbyManager>().gameColors.gameColorList()[PO.colorIndex];
		// unit.team = PO.team;
		// unit.playerObject =  PO;
	}

	

	[ClientRpc]
	public void RpcAssignObject (NetworkIdentity id, int spawnableObjectIndex) {
		Debug.Log ("RpcAssign");
		GameObject go = id.gameObject;
		GameObject graphics = unitGroup.units[spawnableObjectIndex].graphics;
		Vector3 offset = new Vector3(0,(graphics.transform.localScale.y /2) + 0.35f,0);
		graphics = Instantiate (graphics, go.transform.position + offset , go.transform.rotation, go.transform);
		graphics.GetComponent<GraphicsHolder> ().colorize (LobbyManager.singleton.GetComponent<LobbyManager> ().gameColors.gameColorList () [PO.colorIndex]);
		//go.GetComponent<CharacterAnimator>().animator = graphics.GetComponent<Animator>();
		//Assigning data
		MonoUnit unit = go.GetComponent<MonoUnit> ();
		unit.team = PO.team;
		//Assign data here
	
		go.name = PO.team + " - unit" + go.GetComponent<NetworkIdentity> ().netId;
	
		UnitSelectable unitSelectable = go.AddComponent<UnitSelectable> ();
		unitSelectable.playerObject = PO;
		PO.myUnits.Add (go);
	}

	//for Spawning
	public int GetUnitIndex(PlayerUnit unit){
		int unitIndex = 0;
			unitIndex = unitGroup.units.IndexOf(unit);
		return unitIndex;
	}
}