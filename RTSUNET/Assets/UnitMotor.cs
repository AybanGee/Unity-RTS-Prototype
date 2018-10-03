using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent (typeof (UnityEngine.AI.NavMeshAgent))]
public class UnitMotor : NetworkBehaviour {

	public Transform target;

	UnityEngine.AI.NavMeshAgent agent;
	public float followDelay = .25f;

	void Start () {

		agent = GetComponent<UnityEngine.AI.NavMeshAgent> ();
	}

	IEnumerator corFollowTarget (Transform newTarget) {
		while (true) {
			if (newTarget != null) {
				if (hasAuthority)
					CmdMove (newTarget.position);
				FaceTarget ();
			}
			yield return new WaitForSeconds (followDelay);
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

	public void FollowTarget (Interactable newTarget) {
		if (hasAuthority == false) {
			return;
		}
		agent.stoppingDistance = GetComponent<Interactable>().rangeRadius * .8f;
		agent.updateRotation = false;
		target = newTarget.interactionTransform;
		StartCoroutine (corFollowTarget (target));
		CmdFollowTarget (target.GetComponent<NetworkIdentity> ());
	}
	#region  "followServer"
	[Command]
	void CmdFollowTarget (NetworkIdentity targetNi) {
		RpcFollowTarget (targetNi);
	}

	[ClientRpc]
	void RpcFollowTarget (NetworkIdentity targetNi) {
		if (targetNi == null){
		Debug.LogWarning("Interactable has is not on network");
			return;
		}
		Transform newTarget = null;

		newTarget = targetNi.gameObject.transform;

		if (newTarget == null) return;

		Interactable interactable = newTarget.GetComponent<Interactable> ();
		agent.stoppingDistance =  GetComponent<Interactable>().rangeRadius * .8f;
		agent.updateRotation = false;
		target = interactable.interactionTransform;
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