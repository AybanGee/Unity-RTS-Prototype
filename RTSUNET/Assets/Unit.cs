using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
[RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]
public class Unit : NetworkBehaviour {
public Camera cam;
	UnityEngine.AI.NavMeshAgent agent;
	public GameObject graphics;
	public LayerMask movementMask;
	// Use this for initialization
 //public NetworkIdentity netIdNiPo;
 //public bool hasAuth = false ;
	//PlayerObject po;
	void Start () {


		agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
		cam = Camera.main;
		
	}

	void Awake(){

		//AssignMyFvckingPO(netIdNiPo);
	}
	public void MoveToPoint(Vector3 point){
		if(hasAuthority == false){
				return;
		}
		agent.SetDestination(point);
		CmdMove(point);
	}

	[Command]
	void CmdMove(Vector3 point){
		agent.SetDestination(point);
		RpcMove(point);
	}
	[ClientRpc]
	void RpcMove(Vector3 point){
		agent.SetDestination(point);
	}
	/*
	public void AssignMyFvckingPO(NetworkIdentity id){
			if( !hasAuth || id == null ){
				Debug.Log("FVCK3");
				return;
			}
			

			po = NetworkServer.FindLocalObject(id.netId).GetComponent<PlayerObject>();
			po.myUnit = this.gameObject;
	}

	 */
	private void Update() {

/* 
		
		if(hasAuthority == false){
				return;
		}
		if(Input.GetMouseButtonDown(1)){
			Ray ray = cam.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
				Debug.Log("C L I C K !");

			if(Physics.Raycast(ray,out hit,10000,movementMask)){
				Debug.Log("We Hit" +  hit.collider.name + " " + hit.point );
				//Point of Click and location of selected Unit = angle, for formation
				
				// Move our player to what we hit
				//Get angle
				//	ang = Vector3.Angle(selectedUnits[0].GetComponent<Transform>().position,hit.point);
				//	Debug.Log("Angle:" + (ang*10));
				//moveUnits(hit.point);
				MoveToPoint(hit.point);
				//Stop focusing other objects
			}
		}
		*/
	}
}

