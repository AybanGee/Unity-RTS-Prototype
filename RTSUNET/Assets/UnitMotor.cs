using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

[RequireComponent (typeof (UnityEngine.AI.NavMeshAgent))]
public class UnitMotor : NetworkBehaviour {

	public Transform target;

	public NavMeshAgent agent;
	public float speed;
	public float followDelay = .25f;

	void Start () {

		agent = GetComponent<UnityEngine.AI.NavMeshAgent> ();
		agent.speed = speed;
		//set speed 
	}

	IEnumerator corFollowTarget (Transform newTarget) {
	
		while (true) {
			if (newTarget != null) {
				if (hasAuthority)
					CmdMove (newTarget.position);
				FaceTarget ();
			}else{
				StopFollowingTarget();
			}
			yield return null;
		}
	}

	public void MoveToPoint (Vector3 point) {


		if (hasAuthority == false) {
			return;
		}
		agent.SetDestination (point);
		CmdMove (point);
	}

	#region  "moveServer"
	[Command]
	void CmdMove (Vector3 point) {
		//agent.SetDestination(point);

		RpcMove (point);
	}

	[ClientRpc]
	void RpcMove (Vector3 point) {
		agent.SetDestination (point);
	}
	#endregion

	public void FollowTarget (MonoUnitFramework newTarget, MonoSkill skill) {
		Debug.Log("Unit Motor :: new Target : " + newTarget);

		if (hasAuthority == false) {
			return;
		}
		
		agent.stoppingDistance = skill.range * .9f;
		agent.updateRotation = false;
		target = newTarget.transform;
		//StartCoroutine (corFollowTarget (target));
		CmdFollowTarget (target.GetComponent<NetworkIdentity> (), skill.range);

		Debug.Log("Unit Motor :: FollowTarget : End");
	}
	#region  "followServer"
	[Command]
	void CmdFollowTarget (NetworkIdentity targetNi, float range) {
		RpcFollowTarget (targetNi, range);
	}

	[ClientRpc]
	void RpcFollowTarget (NetworkIdentity targetNi, float range) {
	
		if (targetNi == null) {
			Debug.LogWarning ("Interactable has is not on network");
			return;
		}
		Transform newTarget = null;

		newTarget = targetNi.gameObject.transform;

		if (newTarget == null) return;

		agent.stoppingDistance = range * .9f;
		agent.updateRotation = false;
		target = newTarget;
		StartCoroutine (corFollowTarget (target));
	}
	#endregion
	public void StopFollowingTarget () {
		StopAllCoroutines ();
		agent.stoppingDistance = 0;
		agent.updateRotation = true;
	}
	#region  "stop follow Server"
	[Command]
	void CmdStopFollowingTarget () {
		RpcStopFollowingTarget ();
	}

	[ClientRpc]
	void RpcStopFollowingTarget () {
		StopAllCoroutines ();
		agent.stoppingDistance = 0;
		agent.updateRotation = true;
	}
	#endregion

	void FaceTarget () {
		Vector3 direction = (target.position - transform.position).normalized;
		Quaternion lookRotation = Quaternion.LookRotation (new Vector3 (direction.x, 0, direction.z));
		transform.rotation = Quaternion.Slerp (transform.rotation, lookRotation, Time.deltaTime * 5f);
	}

}