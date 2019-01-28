using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEditor.Animations;
using UnityEngine.Networking;

public class CharacterAnimator : MonoBehaviour {

	const float locomotionSmoothTime = .1f;

	NavMeshAgent agent;
	public Animator animator;
	
	// Use this for initialization
	protected virtual void Start () {
		agent = GetComponent<NavMeshAgent>();		
		//animator = transform.GetChild(0).GetComponentInChildren<Animator>();
		
	}
	
	// Update is called once per frame
	protected virtual void Update () {
		if(animator == null){
		animator = transform.GetChild(0).GetComponentInChildren<Animator>();

		Animator netAnimator = GetComponent<Animator>();
		netAnimator.runtimeAnimatorController = animator.runtimeAnimatorController;
        netAnimator.avatar = animator.avatar;

		return;
		}
		float speedPercent = agent.velocity.magnitude / agent.speed;
		animator.SetFloat("speedPercent",speedPercent, locomotionSmoothTime, Time.deltaTime);
	}
}
