using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class UnitSystem : NetworkBehaviour {
	public int UnitSpawnIndex = 4;
	[HideInInspector]
	public UnitGroup unitGroup;
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
		playerUnit.Initialize(go);
		go = Instantiate (go, position, rotation);

		NetworkIdentity ni = go.GetComponent<NetworkIdentity> ();
		NetworkServer.SpawnWithClientAuthority (go, connectionToClient);

		//UnitNew unit = go.GetComponent<UnitNew> ();
		// go.GetComponent<Unit> ().graphics.GetComponent<Renderer> ().material.color = LobbyManager.singleton.GetComponent<LobbyManager>().gameColors.gameColorList()[PO.colorIndex];
		// unit.team = PO.team;
		// unit.playerObject =  PO;
		RpcAssignObject (ni, spawnableObjectIndex);
	}

	[ClientRpc]
	public void RpcAssignObject (NetworkIdentity id, int spawnableObjectIndex) {
		Debug.Log ("RpcAssign");
		GameObject go = id.gameObject;
		GameObject graphics = unitGroup.units[spawnableObjectIndex].graphics;
		Vector3 offset = new Vector3(0,graphics.transform.localScale.y /2,0);
		graphics = Instantiate (graphics, go.transform.position + offset, go.transform.rotation, go.transform);
		graphics.GetComponent<GraphicsHolder> ().colorize (LobbyManager.singleton.GetComponent<LobbyManager> ().gameColors.gameColorList () [PO.colorIndex]);
		//Assigning data
		UnitNew unit = go.GetComponent<UnitNew> ();
		unit.team = PO.team;
		unit.playerObject = PO;
		//Assign data here
	
		go.name = PO.team + " - unit" + go.GetComponent<NetworkIdentity> ().netId;
	
		UnitSelectable unitSelectable = go.AddComponent<UnitSelectable> ();
		unitSelectable.playerObject = PO;
		PO.myUnits.Add (go);
	}

	
}

//ORIGINAL SCRIPT
/*

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class UnitSystem :NetworkBehaviour, ISpawnHandler {
	public UnitGroup unitGroup;
	PlayerObject PO;

	void Awake () {
		PO = GetComponent<PlayerObject> ();
	}
		//Move to new script
	public void spawnUnit (int spawnIndex,Vector3 pos,Quaternion rot) {
		CmdSpawnObject(spawnIndex,pos,rot);

	}

[Command]
   public void CmdSpawnObject(int spawnableObjectIndex, Vector3 position, Quaternion rotation)
    {
       	GameObject go = NetworkManager.singleton.spawnPrefabs[spawnableObjectIndex];
		go = Instantiate (go, position, rotation);
		NetworkIdentity ni = go.GetComponent<NetworkIdentity> ();
		NetworkServer.SpawnWithClientAuthority (go, connectionToClient);
		Unit unit = go.GetComponent<Unit> ();
		go.GetComponent<Unit> ().graphics.GetComponent<Renderer> ().material.color = LobbyManager.singleton.GetComponent<LobbyManager>().gameColors.gameColorList()[PO.colorIndex];
		unit.team = PO.team;
		unit.playerObject =  PO;
		RpcAssignObject(ni);
    }
[ClientRpc]
    public void RpcAssignObject(NetworkIdentity id)
    {
        	Debug.Log("RpcAssign");
		GameObject go = id.gameObject;
		Unit unit = go.GetComponent<Unit> ();
		unit.team = PO.team;
		unit.playerObject = PO;
		go.name = PO.team + " - unit" + go.GetComponent<NetworkIdentity>().netId;
		go.GetComponent<Unit> ().graphics.GetComponent<Renderer> ().material.color = LobbyManager.singleton.GetComponent<LobbyManager>().gameColors.gameColorList()[PO.colorIndex];

		if (go.GetComponent<CharStats> () != null)
			go.GetComponent<CharStats> ().netPlayer = PO;

		go.AddComponent<UnitSelectable> ();
		go.GetComponent<UnitSelectable> ().playerObject = PO;
		 PO.myUnits.Add (go);
    }

}
 */