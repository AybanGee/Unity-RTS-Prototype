﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class PlayerObject : NetworkBehaviour {
	
	
	public List<GameObject> selectedUnits = new List<GameObject>();
	public List<GameObject> myUnits = new List<GameObject>();
	public Camera cam;

	public List<Color> selectedColor = new List<Color>();
	
	float ang;
	
	public LayerMask movementMask;

//passed variables
[SyncVar]
	public int team;
[SyncVar]
	public int faction;
[SyncVar]
	public string playerName;


	// Use this for initialization
	void Start () {
		//Is this actually my own local PlayerObject?
		
		if( isLocalPlayer == false){
			//This object belongs to another player.
				return;
		}

		
		//Debug.Log("HAS AUTH?" + hasAuthority);

		gameObject.name = gameObject.name + "NID" + GetComponent<NetworkIdentity>().netId;

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
		 //spawnUnit();
		 //spawnCamera();
		//Debug.Log("PlayerObject::Start -- Spawning my own personal Camera");
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
		
			if(Physics.Raycast(ray,out hit,10000)){
				
				Character interactable = hit.collider.GetComponent<Character>();
				//.Log("JORI JORI AJA AJA " + hit.collider.name);
				if(interactable != null){
					// if(interactable.GetComponent<Unit>().team == team){
					// 	//hard coded stuff;
					// 	Debug.Log("Oops Same team");
					// 	return;
					// }
					Debug.Log("SHIT KALABAN!");
					if(myUnits.Count > 0)
					foreach (GameObject unit in myUnits)
					{
//TO BE REMOVED IF FOUNF SOLUTION ON NOT DELETEiNG OBJECTS
							if(unit == null){
								myUnits.Remove(unit);
								continue;
							}
					
						
						CmdFocus(unit.GetComponent<NetworkIdentity>(),interactable.GetComponent<NetworkIdentity>());
					}
					return;
				}
			}
		
			if(myUnits.Count <= 0)return;
			if(Physics.Raycast(ray,out hit,10000,movementMask)){
				//move movement mask to controller
				//Debug.Log("We Hit" +  hit.collider.name + " " + hit.point );
				//Point of Click and location of selected Unit = angle, for formation
				
				// Move our player to what we hit
				//moveUnits(hit.point);
				moveUnits(hit.point);
				//myUnit.GetComponent<Unit>().MoveToPoint(hit.point);


				//Stop focusing other objects
			}

			


		}		
	}

		[Command]
			void CmdFocus(NetworkIdentity unitID, NetworkIdentity interactionId){
				unitID.GetComponent<Unit>().SetFocus(interactionId.GetComponent<Character>());
				RpcFocus(unitID,interactionId);
			}
	[ClientRpc]
			void RpcFocus(NetworkIdentity unitID, NetworkIdentity interactionId){
				unitID.GetComponent<Unit>().SetFocus(interactionId.GetComponent<Character>());
			}
	
// 			[Command]
// 	void CmdChangeAuthority(NetworkIdentity myNet, NetworkIdentity otherNet){
// 		myNet.AssignClientAuthority(otherNet.connectionToClient);
// 	}

// 		[Command]
// 	void CmdRevertAuthority(NetworkIdentity myNet, NetworkIdentity otherNet){
// 		myNet.RemoveClientAuthority(otherNet.connectionToClient);

// 	}
// public void AttackEnemy(NetworkIdentity targetIdentity, NetworkIdentity myIdentity){
	
// 	CmdAttack(targetIdentity, myIdentity);
// }

[Command]
	public void CmdAttack(NetworkIdentity targetIdentity, NetworkIdentity myIdentity){
		targetIdentity.GetComponent<CharStats>().TakeDamage(myIdentity.GetComponent<CharStats>().damage.GetValue());
	}

	/////////////////////////////////// FUNCTIONS

	#region "movement"
	float offsetSize = 2;
	int perRow = 6;

	void moveUnits(Vector3 hit){
		
		StartCoroutine(coMove(myUnits,hit));
	}
	IEnumerator coMove(List<GameObject> gos,Vector3 hit){
		int rowCount = (-1)*(perRow/2),colCount = 0;
	for (int i = 0; i < gos.Count; i++)
		{	
			float x,z;
			if(rowCount >= perRow/2){
				rowCount = (-1)*(perRow/2);
				colCount ++;
			}	
			
			x = offsetSize * rowCount;
			z = offsetSize * colCount;


			Vector3 offset = new Vector3(x,0,z);
			if(gos[i] == null){
				continue;
			}
			gos[i].GetComponent<Unit>().RemoveFocus();
			gos[i].GetComponent<Unit>().MoveToPoint(hit + offset);

			rowCount ++;
		}
		yield return null;
	}
	#endregion
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
		go.GetComponent<Unit>().team = team;
		go.GetComponent<Unit>().graphics.GetComponent<MeshRenderer>().materials[0].color = selectedColor[team -1];
		 NetworkIdentity ni = go.GetComponent<NetworkIdentity>();
		Debug.Log("Player Object :: --Spawning Unit");
		NetworkServer.Spawn(go);
		bool ToF = go.GetComponent<NetworkIdentity>().AssignClientAuthority(GetComponent<NetworkIdentity>().connectionToClient);
		
		Debug.Log("Player Object :: --Unit Spawned : " + ToF);
		RpcAssignObject(ni, team);
	}



//DYNAMIC IS NOT FEASIBLE... MUST ACCESS GLOBAL VAR TO WORK

[ClientRpc]
	void RpcAssignObject(NetworkIdentity id,int t){
		//	if(!isLocalPlayer)return;


		//Debug.Log(id.netId.Value);
		GameObject spawnHolder = id.gameObject;
		
		spawnHolder.name = team +" - unit - " + netId;
		spawnHolder.GetComponent<Unit>().graphics.GetComponent<MeshRenderer>().materials[0].color = selectedColor[t-1];
		spawnHolder.GetComponent<CharStats>().netPlayer = this;
		spawnHolder.GetComponent<UnitCombat>().netPlayer = this;
		myUnits.Add(spawnHolder);

	
	}

#endregion
#region "DeSpawning"
	public void despawnUnit(GameObject go){
		Debug.Log("ABOUT TO UNSPAWN : " + go.name);

		// if(!isLocalPlayer){
		// 	Debug.Log("Another client tries to command");
		// 	if(!hasAuthority){
		// 	Debug.Log("and Client has no authority");
		// 	return;
		// 	}
		// 	return;
		// }
		// if(!hasAuthority){
		// 	Debug.Log("Client has no authority");
		// 	return;
		// }
		Debug.Log("Unspawning NID" + GetComponent<NetworkIdentity>().netId);

		 CmdDeSpawnObject(go.GetComponent<NetworkIdentity>());
	}

[Command]
	void CmdDeSpawnObject(NetworkIdentity id){
		Debug.Log("CmdDespawnObject");
		
	//id.AssignClientAuthority(other.connectionToClient);
	//myUnits.Remove(id.gameObject);
	NetworkServer.Destroy(id.gameObject);


	}

	




#endregion


// [ClientRpc]
// 	void RpcAssignObject(NetworkIdentity id,int t){
// 		//Debug.Log(id.netId.Value);
// 		NetworkIdentity[] ni = FindObjectsOfType<NetworkIdentity>();
// 	 	for (int i = 0; i < ni.Length; i++)
// 		{
// 		 	if(ni[i].netId == id.netId){
// 				spawnHolder = ni[i].gameObject;
// 				spawnHolder.GetComponent<Unit>().graphics.GetComponent<MeshRenderer>().materials[0].color = selectedColor[t-1];
// 				spawnHolder.GetComponent<CharStats>().netPlayer = this;
// 				myUnits.Add(spawnHolder);
// 			return;
// 		 	}
// 	 	}
	
// 	}

}
