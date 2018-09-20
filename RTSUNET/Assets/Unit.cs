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

	public int team;
	
	public Interactable focus;
	public Transform target;

	public float followDelay = .25f;
	// Use this for initialization
 //public NetworkIdentity netIdNiPo;
 //public bool hasAuth = false ;
	//PlayerObject po;
	void Start () {


		agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
		cam = Camera.main;
		
	}


	public void SetFocus(Interactable newFocus){
		if(newFocus != focus){
			if(focus != null)
			focus.OnDefocused();
			focus = newFocus;
			FollowTarget(newFocus);
		}
		newFocus.OnFocused(transform);
	}

	public void RemoveFocus(){
			if(focus != null)
			focus.OnDefocused();
		focus = null;
		 StopFollowingTarget();
	}


	

	private void Update() {


	}

	#region "move to a motor script"
	IEnumerator corFollowTarget(Transform newTarget){
		while(true){
				if(newTarget != null){
				CmdMove(newTarget.position);
				FaceTarget();
				}
		yield return new WaitForSeconds(followDelay);
		}
	}

	public void MoveToPoint(Vector3 point){
		if(hasAuthority == false){
				return;
		}
		agent.SetDestination(point);
		CmdMove(point);
	}
	#region  "moveServer"
	[Command]
	void CmdMove(Vector3 point){
		//agent.SetDestination(point);
		
		RpcMove(point);
	}
	[ClientRpc]
	void RpcMove(Vector3 point){
		agent.SetDestination(point);
	}
	#endregion

	public void FollowTarget(Interactable newTarget){
			if(hasAuthority == false){
				return;
		}
		agent.stoppingDistance = newTarget.radius * .8f;
		agent.updateRotation = false;
			target = newTarget.interactionTransform;
			StartCoroutine(corFollowTarget(target));
			CmdFollowTarget(target.GetComponent<NetworkIdentity>());
	}
#region  "followServer"
	[Command]
	void CmdFollowTarget(NetworkIdentity targetNi){
// Transform newTarget = null;
// 	NetworkIdentity[] ni = FindObjectsOfType<NetworkIdentity>();
// 	 	for (int i = 0; i < ni.Length; i++)
// 		{
// 		 	if(ni[i].netId == targetNi.netId){
// 				newTarget = ni[i].transform;
// 			return;
// 		 	}
// 			 i++;
// 	 	}
// 		if(newTarget == null) return;

// 		Interactable interactable = newTarget.GetComponent<Interactable>();
// 		agent.stoppingDistance = interactable.radius * .8f;
// 		agent.updateRotation = false;
// 		target = interactable.interactionTransform;
		 RpcFollowTarget(targetNi);
	}

	[ClientRpc]
	void RpcFollowTarget(NetworkIdentity targetNi){

	NetworkIdentity[] ni = FindObjectsOfType<NetworkIdentity>();
		Transform newTarget = null;
	 	for (int i = 0; i < ni.Length; i++)
		{
		 	if(ni[i].netId == targetNi.netId){
				newTarget = ni[i].transform;
			return;
		 	}
			 i++;
	 	}
		if(newTarget == null) return;

		Interactable interactable = newTarget.GetComponent<Interactable>();
		agent.stoppingDistance = interactable.radius * .5f;
		agent.updateRotation = false;
		target = interactable.interactionTransform;
		StartCoroutine(corFollowTarget(target));
	}
#endregion
	public void StopFollowingTarget(){
	 StopAllCoroutines();
	 agent.stoppingDistance = 0;
	 	agent.updateRotation = true;
	}
#region  "stop follow Server"
[Command]
void CmdStopFollowingTarget(){
RpcStopFollowingTarget();
}
[ClientRpc]
void RpcStopFollowingTarget(){
 StopAllCoroutines();
	 agent.stoppingDistance = 0;
	 	agent.updateRotation = true;
}
#endregion


	void FaceTarget(){
		Vector3 direction = (target.position - target.position).normalized;
		Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x,0,direction.y));
		transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
	}
	
	#endregion
}

