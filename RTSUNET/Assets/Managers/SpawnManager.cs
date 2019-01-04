using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
//WARN:THIS SCRIPT IS NOT USED!!!
public abstract class SpawnManager : NetworkBehaviour {

	#region Spawn Methods
		public void SpawnObject (
		int spawnIndex,
		Vector3 position,
		Quaternion rotation
	) {
		CmdSpawnObject (spawnIndex, position, rotation);
	}
		#region Experimtental
		GameObject _graphics;
		public void SpawnObject (
			int spawnIndex,
			Vector3 position,
			Quaternion rotation,
			GameObject graphics,
			Vector3 graphicsOffset,
			Quaternion graphicsRotaion
		) {
			_graphics = graphics;
			_graphics.transform.position += graphicsOffset;
			_graphics.transform.rotation = graphicsRotaion;
			CmdSpawnObjectWithGraphics (spawnIndex, position, rotation);
		}

		public void SpawnObject (
			int spawnIndex,
			Vector3 position,
			Quaternion rotation,
			GameObject graphics,
			Vector3 graphicsOffset,
			Quaternion graphicsRotaion,
			Color graphicsColor
		) {
			_graphics = graphics;
			_graphics.transform.position += graphicsOffset;
			_graphics.transform.rotation = graphicsRotaion;
			_graphics.GetComponent<GraphicsHolder> ().colorize (graphicsColor);
			CmdSpawnObjectWithGraphics (spawnIndex, position, rotation);
		}
		#endregion
		#endregion
	
	#region Spawn Commands
	[Command]
	public void CmdSpawnObject (int spawnableObjectIndex, Vector3 position, Quaternion rotation) {

		GameObject go = NetworkManager.singleton.spawnPrefabs[spawnableObjectIndex];
		go = Instantiate (go, position, rotation);
		NetworkIdentity ni = go.GetComponent<NetworkIdentity> ();
		NetworkServer.SpawnWithClientAuthority (go, connectionToClient);
		OnSpawn (ni);
	}

	[Command]
	public void CmdSpawnObjectWithGraphics (int spawnableObjectIndex, Vector3 position, Quaternion rotation) {

		GameObject go = NetworkManager.singleton.spawnPrefabs[spawnableObjectIndex];
		go = Instantiate (go, position, rotation);
		GameObject gfx = _graphics;
		if (gfx == null) { Debug.LogError ("No Graphics on " + go.name); return; }
		gfx = Instantiate (gfx);
		NetworkIdentity ni = go.GetComponent<NetworkIdentity> ();
		NetworkServer.SpawnWithClientAuthority (go, connectionToClient);
		OnSpawn (ni);
	}
	#endregion

	#region Events
		

	public virtual void OnSpawn (NetworkIdentity id) {
		//RpcAssignObject (id);
		Debug.Log ("Successfully spawned " + id.gameObject.name + " on the network");
	}
	#endregion


	#region "DeSpawning"
	public void despawnUnit (GameObject go) {
		Debug.Log ("ABOUT TO UNSPAWN : " + go.name);

		Debug.Log ("Unspawning NID:" + GetComponent<NetworkIdentity> ().netId);

		CmdDeSpawnObject (go.GetComponent<NetworkIdentity> ());
	}

	[Command]
	void CmdDeSpawnObject (NetworkIdentity id) {
		Debug.Log ("CmdDespawnObject");

		NetworkServer.Destroy (id.gameObject);

	}

	#endregion

}