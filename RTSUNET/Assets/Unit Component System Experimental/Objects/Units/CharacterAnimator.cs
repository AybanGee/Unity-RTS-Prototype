using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
//using UnityEditor.Animations;
using UnityEngine.Networking;

public class CharacterAnimator : NetworkBehaviour {

	const float locomotionSmoothTime = .1f;

	NavMeshAgent agent;
	public Animator animator;
	
	// Use this for initialization
	protected virtual void Start () {
		agent = GetComponent<NavMeshAgent>();		
		
	}
	
	// Update is called once per frame
	protected virtual void Update () {
		float speedPercent = agent.velocity.magnitude / agent.speed;
		animator.SetFloat("speedPercent",speedPercent, locomotionSmoothTime, Time.deltaTime);
	}

	public void SetTrigger(string triggerName){
		CmdSetTrigger(triggerName);
	}
	[Command]
	public void CmdSetTrigger(string triggerName){
		animator.SetTrigger(triggerName);
		RpcSetTrigger(GetComponent<NetworkIdentity>(),triggerName);
	}
	[ClientRpc]
	public void RpcSetTrigger(NetworkIdentity ni,string triggerName){
		ni.GetComponent<CharacterAnimator>().animator.SetTrigger(triggerName);
	}
}
