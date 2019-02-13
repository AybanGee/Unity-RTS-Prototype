using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class UnitSystem : NetworkBehaviour {
	public int UnitSpawnIndex = 4;
	public Vector3 spawnPoint;
	[HideInInspector]
	public UnitGroup unitGroup, thGroup, bGroup;
	PlayerObject PO;

	void Awake () {
		PO = GetComponent<PlayerObject> ();
	}
	//Move to new script
	public void spawnUnit (int spawnIndex, Vector3 pos, Quaternion rot) {
		CmdSpawnObject (spawnIndex, pos, rot);

	}
	[ClientRpc]
	public void RpcAddPlayerUnit(NetworkIdentity ni, int spawnableObjectIndex){
		GameObject go = ni.gameObject;
		PlayerUnit playerUnit = unitGroup.units[spawnableObjectIndex];
		Debug.Log(go);
		MonoUnitFramework muf = go.GetComponent<MonoUnitFramework> ();
		muf.playerUnit = playerUnit;
		muf.PO = PO;	
		playerUnit.Initialize (go);
	}

	[Command]
	public void CmdSpawnObject (int spawnableObjectIndex, Vector3 position, Quaternion rotation) {
		GameObject go = NetworkManager.singleton.spawnPrefabs[UnitSpawnIndex];
		PlayerUnit playerUnit = unitGroup.units[spawnableObjectIndex];

		go = Instantiate (go, position, rotation);

		//set Graphics

		/* 		Debug.Log ("CMD add Graphics");
				GameObject graphics = unitGroup.units[spawnableObjectIndex].graphics;
				Vector3 offset = new Vector3 (0, (graphics.transform.localScale.y / 2) + 0.35f, 0);
				graphics = Instantiate (graphics, go.transform.position + offset, go.transform.rotation, go.transform);
				graphics.GetComponent<GraphicsHolder> ().colorize (LobbyManager.singleton.GetComponent<LobbyManager> ().gameColors.gameColorList () [PO.colorIndex]);

				Animator anim = graphics.transform.GetChild (0).GetComponent<Animator> ();
				go.GetComponent<CharacterAnimator> ().animator = anim; */

		//go.GetComponent<NetworkAnimator>().animator = graphics.GetComponent<Animator>();
	//	Debug.Log ("UnitSystem :: MonoUnitFramework : " + muf);
	//	Debug.Log ("UnitSystem :: Abilities : " + muf.primitiveAbilities.Count);
		//ClientScene.RegisterPrefab(go);

		//playerUnit.Initialize(go);
		//initialize abilities
	//	muf.debugMonoUnit ();
		NetworkIdentity ni = go.GetComponent<NetworkIdentity> ();
		NetworkServer.SpawnWithClientAuthority (go, connectionToClient);
		
		RpcAddPlayerUnit(go.GetComponent<NetworkIdentity>(),spawnableObjectIndex);

		/* 		bool wait = true;
				while (wait) {
					Debug.Log("Unit System :: Wait :");
					if (muf.primitiveAbilities.Count > 0) {
						Debug.Log ("Unit System :: Player Abilities : " + muf.primitiveAbilities.Count);
						wait = false;
					}
				} */
		//Debug.Log("MonoUnitFramework : ");
		Debug.Log ("UnitSystem :: NetID : Abilities : " + ni.gameObject.GetComponent<MonoUnitFramework> ().primitiveAbilities.Count);

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
		go.GetComponent<MonoUnitFramework>().debugMonoUnit ();

		//Debug.Log("RPC Assign :: NetID : " + id);

		Debug.Log ("UnitSystem :: RpcAssign : GO : " + go);
		//PlayerUnit playerUnit = unitGroup.units[spawnableObjectIndex];

		/* 	if(!isServer)
			playerUnit.Initialize (go); */

		//set Graphics
		/* if (go.transform.gameObject.GetComponentInChildren<GraphicsHolder> () == null) { */
		Debug.Log ("RPC add Graphics");
		GameObject graphics = unitGroup.units[spawnableObjectIndex].graphics;
		Vector3 offset = new Vector3 (0, (graphics.transform.localScale.y / 2) + 0.35f, 0);
		graphics = Instantiate (graphics, go.transform.position + offset, go.transform.rotation, go.transform);
		graphics.GetComponent<GraphicsHolder> ().colorize (LobbyManager.singleton.GetComponent<LobbyManager> ().gameColors.gameColorList () [PO.colorIndex]);

		Animator anim = graphics.transform.GetChild (0).GetComponent<Animator> ();
		go.GetComponent<CharacterAnimator> ().animator = anim;
		/* 
				} */

		//Assigning data
		MonoUnit unit = go.GetComponent<MonoUnit> ();
		unit.team = PO.team;
		Debug.Log ("UnitSystem :: RpcAssign : MonoUnit : " + unit);
		Debug.Log ("UnitSystem :: RpcAssign : Primitive Abilities : " + unit.primitiveAbilities.Count);

		//Debug.Log ("UnitSystem :: unit : " + unit);
		//Debug.Log ("UnitSystem :: playerunit : " + playerUnit);
		//Debug.Log ("UnitSystem :: abilities.Count : " + playerUnit.abilities.Count);

		go.name = PO.team + " - unit" + go.GetComponent<NetworkIdentity> ().netId;

		UnitSelectable unitSelectable = go.AddComponent<UnitSelectable> ();
		unitSelectable.playerObject = PO;
		PO.myUnits.Add (go);

		if(PO.isSinglelPlayer){
			QuestItem qi = go.AddComponent<QuestItem>();
		}
	}

	//for Spawning
	public int GetUnitIndex (PlayerUnit unit) {
		int unitIndex = 0;
		unitIndex = unitGroup.units.IndexOf (unit);
		return unitIndex;
	}
}