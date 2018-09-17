using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerObject : NetworkBehaviour {
	
	
	public List<GameObject> selectedUnits = new List<GameObject>();
	float ang;
	// Use this for initialization
	void Start () {
		
		//Is this actually my own local PlayerObject?
		if( isLocalPlayer == false){
			//This object belongs to another player.
				return;
		}
		
		//cam = Camera.main;
		//Since the PlayerObject is invisible and not part of the world,
		//give me something to move around!

		Debug.Log("PlayerObject::Start -- Spawning my own personal Unit");

		//Instantiate() only creates an object on the LOCAL COMPUTER.
		//Even if it has a NetworkIdentity it still will NOT exist on
		//the network (and therefore not on any other client) UNLESS
		//NetworkSerever.Spawn() is called on this object.
		
		//Instantiate(PlayerUnitPrefab);

		//Command the server to SPAWN our Unit.
		//
		//Debug.Log("unit name" + myUnit.name);
		//NetworkIdentity ni = this.GetComponent<NetworkIdentity>();
		//Debug.Log("ID NATEN " + ni.netId.Value);
		CmdSpawnMyUnit();

		Debug.Log("PlayerObject::Start -- Spawning my own personal Camera");
		//CmdSpawnMyCamera();
		//CmdSpawnMySphere();
	//	cam = GetComponentInChildren<Camera>();
	}
	public GameObject PlayerUnitPrefab;
	public GameObject CameraPrefab;

	// Update is called once per frame
	void Update () {
		//Remember: Update runs on EVERYONE's computer, wether or not they own this
		//particular player object.
			if(isLocalPlayer == false  ){
			return;
			}

	//Debug.Log(myUnit.name);
				
		if(Input.GetMouseButtonDown(1)){
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
				Debug.Log("C L I C K !");
				if(myUnit == null){
					Debug.Log("Oh Fu..");
				}
			if(Physics.Raycast(ray,out hit,10000,myUnit.GetComponent<Unit>().movementMask)){
				Debug.Log("We Hit" +  hit.collider.name + " " + hit.point );
				//Point of Click and location of selected Unit = angle, for formation
				
				// Move our player to what we hit
				//Get angle
				//	ang = Vector3.Angle(selectedUnits[0].GetComponent<Transform>().position,hit.point);
				//	Debug.Log("Angle:" + (ang*10));
				//moveUnits(hit.point);
				myUnit.GetComponent<Unit>().MoveToPoint(hit.point);
				//Stop focusing other objects
			}
		}

			//Debug.Log(myUnit.name);
/* 24:58 
		if(Input.GetKeyDown(KeyCode.Space)){
			CmdMoveUnitUp();
		}
 */
		
	}
	/////////////////////////////////// FUNCTIONS
	float offsetSize = 2;
	int perRow = 6;
	void moveUnits(Vector3 hit){
		//int rowCount = (-1)*(perRow/2),colCount = 0;
	/*	for (int i = 0; i < selectedUnits.Count; i++)
		{	
			float x,z;
			if(rowCount >= perRow/2){
				rowCount = (-1)*(perRow/2);
				colCount ++;
			}	
			
			x = offsetSize * rowCount;
			z = offsetSize * colCount;

			/**float xR= x * Mathf.Cos(ang) - z * Mathf.Sin(ang);
			float zR= z * Mathf.Cos(ang) - x * Mathf.Sin(ang);
			**/

			//Vector3 offset = new Vector3(x,0,z);
			myUnit.GetComponent<Unit>().MoveToPoint(hit/* + offset*/);

		//	rowCount ++;
		//}
	}

	//////////////////////////////////// COMMANDS
	// Commands are special functions that ONLY get executed on the server.
public GameObject myUnit;
	[Command]
	void CmdSpawnMyUnit(){
		//We are guaranteed to be on the severe right now.
		//selectedUnits.Add(go);
	//	if(isLocalPlayer == false)
		//return;
		//myUnit = go;
		//Debug.Log("go name" + go.name);
		//Now that the object exists on the server, propagate it to all
		//the clients(and also wire up the Network Identity)
		GameObject go = NetworkManager.singleton.spawnPrefabs[0];
		//go.GetComponent<Unit>().netIdNiPo = thisPOid;
		//go.GetComponent<Unit>().hasAuth = true;

	//		Debug.Log("ID NATEN @ COMMAND " + go.GetComponent<Unit>().netIdNiPo.netId.Value);
		go = Instantiate(go);
	 	NetworkIdentity ni = go.GetComponent<NetworkIdentity>();
		Debug.Log("Player Object :: --Spawning Unit");
		bool ToF = NetworkServer.SpawnWithClientAuthority(go,connectionToClient);
		Debug.Log("Player Object :: --Unit Spawned : " + ToF);
		RpcAssignUnit(ni);
		
	}
	[ClientRpc]
	void RpcAssignUnit(NetworkIdentity id){
		Debug.Log(id.netId.Value);

		NetworkIdentity[] ni = FindObjectsOfType<NetworkIdentity>();
	 for (int i = 0; i < ni.Length; i++)
	 {
		 if(ni[i].netId == id.netId){
			myUnit = ni[i].gameObject;
			break;
		 }
	 }

	}
/* 
	[Command]
	void CmdTrial(GameObject go){
			Debug.Log(go.name);
			Debug.Log("WTF");
	}

	[Command]
	void CmdSpawnMyCamera(){
		//We are guaranteed to be on the severe right now.
		GameObject go = Instantiate(CameraPrefab);
		//Now that the object exists on the server, propagate it to all
		//the clients(and also wire up the Network Identity)
		Debug.Log("Player Object :: --Spawning Camera");
		NetworkServer.SpawnWithClientAuthority(go,connectionToClient);
		cam = go.GetComponent<Camera>();
		Debug.Log("Player Object :: --Camera Spawned");
		Debug.Log("cam = " + cam.name);
	

	}
*/
public GameObject spherePrefab;
public GameObject sphereUnit;

		[Command]
	void CmdSpawnMySphere(){
		//We are guaranteed to be on the severe right now.
		GameObject go = Instantiate(spherePrefab);

		sphereUnit = go;
		//Now that the object exists on the server, propagate it to all
		//the clients(and also wire up the Network Identity)
		Debug.Log("Player Object :: --Spawning Sphere"); 
		NetworkServer.SpawnWithClientAuthority(go,connectionToClient);
		//cam = go.GetComponent<Camera>();
		Debug.Log("Player Object :: --Sphere Spawned :");
		//Debug.Log("cam = " + cam.name);
	

	}


}
