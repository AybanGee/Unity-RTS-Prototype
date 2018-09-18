using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class PlayerObject : NetworkBehaviour {
	
	
	public List<GameObject> selectedUnits = new List<GameObject>();
	public List<GameObject> myUnits = new List<GameObject>();
	public Camera cam;

	public Color selectedColor;
	GameObject spawnHolder;
	float ang;

	// Use this for initialization
	void Start () {
		//Is this actually my own local PlayerObject?
		if( isLocalPlayer == false){
			//This object belongs to another player.
				return;
		}

		//Since the PlayerObject is invisible and not part of the world,
		//give me something to move around!

		Debug.Log("PlayerObject::Start -- Spawning my own personal Unit");

		//Instantiate() only creates an object on the LOCAL COMPUTER.
		//Even if it has a NetworkIdentity it still will NOT exist on
		//the network (and therefore not on any other client) UNLESS
		//NetworkSerever.Spawn() is called on this object.
		
		//Instantiate(PlayerUnitPrefab);
		cam = Camera.main;
		//Command the server to SPAWN our Unit.
		//CmdSpawnMyCamera();
		 spawnUnit();
		 //spawnCamera();
		Debug.Log("PlayerObject::Start -- Spawning my own personal Camera");
		//CmdSpawnMyCamera();

	}

	// Update is called once per frame
	void Update () {
		//Remember: Update runs on EVERYONE's computer, wether or not they own this
		//particular player object.
			if(isLocalPlayer == false  ){
			return;
			}

	//Debug.Log(myUnit.name);
	//	
		if(Input.GetKeyDown(KeyCode.E)){
			spawnUnit();
		}	

		if(Input.GetMouseButtonDown(1)){
			//Slow script
			
			Ray ray = cam.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;

			//There should be at least 1 selected unit.
			if(myUnits.Count < 1){
				Debug.Log("You have no units.");
				return;
			}
			
			if(Physics.Raycast(ray,out hit,10000,myUnits[0].GetComponent<Unit>().movementMask)){
				Debug.Log("We Hit" +  hit.collider.name + " " + hit.point );
				//Point of Click and location of selected Unit = angle, for formation
				
				// Move our player to what we hit
				//moveUnits(hit.point);
				moveUnits(hit.point);
				//myUnit.GetComponent<Unit>().MoveToPoint(hit.point);
				//Stop focusing other objects
			}


		}		
	}
	/////////////////////////////////// FUNCTIONS
	float offsetSize = 2;
	int perRow = 6;

	void moveUnits(Vector3 hit){
		int rowCount = (-1)*(perRow/2),colCount = 0;
		for (int i = 0; i < myUnits.Count; i++)
		{	
			float x,z;
			if(rowCount >= perRow/2){
				rowCount = (-1)*(perRow/2);
				colCount ++;
			}	
			
			x = offsetSize * rowCount;
			z = offsetSize * colCount;


			Vector3 offset = new Vector3(x,0,z);
			myUnits[i].GetComponent<Unit>().MoveToPoint(hit + offset);

			rowCount ++;
		}
	}
#region "Spawn Handler"
//Move to new script
	void spawnUnit(){
		CmdSpawnObject(0);
	}
/* 
	void spawnCamera(){
		CmdSpawnObject(1);
		cam = spawnHolder;
	}
*/
	//////////////////////////////////// COMMANDS
	// Commands are special functions that ONLY get executed on the server.
public GameObject myUnit;
	[Command]
	void CmdSpawnObject(int spawnableObjectIndex){
		//We are guaranteed to be on the severe right now.
		
		//selectedUnits.Add(go);

		//Now that the object exists on the server, propagate it to all
		//the clients(and also wire up the Network Identity)
		
		//Spawn Unit and Assign to a Player
		GameObject go = NetworkManager.singleton.spawnPrefabs[spawnableObjectIndex];
		go = Instantiate(go);

	 	NetworkIdentity ni = go.GetComponent<NetworkIdentity>();
		Debug.Log("Player Object :: --Spawning Unit");
		bool ToF = NetworkServer.SpawnWithClientAuthority(go,connectionToClient);
		Debug.Log("Player Object :: --Unit Spawned : " + ToF);
		RpcAssignObject(ni);
	}



//DYNAMIC IS NOT FEASIBLE... MUST ACCESS GLOBAL VAR TO WORK

[ClientRpc]
	void RpcAssignObject(NetworkIdentity id){
		//Debug.Log(id.netId.Value);
		NetworkIdentity[] ni = FindObjectsOfType<NetworkIdentity>();
	 	for (int i = 0; i < ni.Length; i++)
		{
		 	if(ni[i].netId == id.netId){
				spawnHolder = ni[i].gameObject;
				myUnits.Add(spawnHolder);
			return;
		 	}
	 	}
	
	}

#endregion
/* 
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
}
