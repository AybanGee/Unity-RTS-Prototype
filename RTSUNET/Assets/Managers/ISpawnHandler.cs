using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public interface ISpawnHandler{
void CmdSpawnObject (int spawnableObjectIndex, Vector3 position, Quaternion rotation);
void RpcAssignObject (NetworkIdentity id);
	
}
