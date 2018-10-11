using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class UnitSystem :NetworkBehaviour, ISpawnHandler {
	
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
		GameObject spawnHolder = id.gameObject;
		Unit unit = spawnHolder.GetComponent<Unit> ();
		unit.team = PO.team;
		unit.playerObject = PO;
		spawnHolder.name = PO.team + " - unit" + spawnHolder.GetComponent<NetworkIdentity>().netId;
		spawnHolder.GetComponent<Unit> ().graphics.GetComponent<Renderer> ().material.color = LobbyManager.singleton.GetComponent<LobbyManager>().gameColors.gameColorList()[PO.colorIndex];

		if (spawnHolder.GetComponent<CharStats> () != null)
			spawnHolder.GetComponent<CharStats> ().netPlayer = PO;

		spawnHolder.AddComponent<UnitSelectable> ();
		spawnHolder.GetComponent<UnitSelectable> ().playerObject = PO;
		 PO.myUnits.Add (spawnHolder);
    }

}
