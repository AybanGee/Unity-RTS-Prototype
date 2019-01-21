using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public abstract class MonoSkill : NetworkBehaviour {
	public string sName;
	public Sprite sSprite;
	public AudioClip sSound;
    public float range = 3;
	 	public List<string> animationTriggers;

	public MonoAbility parentAbility;
	private bool isActive = false;
	private bool isActing = false;
	private GameObject skillTarget;


	public virtual void ActOn (GameObject go) { 

			isActing = true;
		}

	public virtual void Act () {  
		Debug.Log("ACT!");
			isActing = true;
		
		}
	public virtual void Stop(){ isActing = false;}
	public bool isTargetInRange(Transform target){
		MonoUnitFramework targetUnit = target.gameObject.GetComponent<MonoUnitFramework>();
		float influence = 0; 
		if(targetUnit != null){
			influence = targetUnit.rangeInfluence;
		}
		Debug.Log("Influence " + influence);
	

		if(Vector3.Distance(this.transform.position,target.position) - influence <= range) return true;
		return false;
	}
	public void Update() {
		//activates a skill
		if(isActive && skillTarget != null){
			//Debug.Log("Attack is Active");
			
			if(isTargetInRange(skillTarget.transform)){
				Debug.Log("Target is in Range");
			ActOn(skillTarget);
			Deactivate();
			}
		}
		else if(isActive && skillTarget == null){
			Act();
			Deactivate();
		}
		//checks if target is still in range
		if(isActing && skillTarget != null){
			if(!isTargetInRange(skillTarget.transform)){
				Debug.Log("Out of Range	");

				Stop();
			}
		}
	}
	#region Activation
	public void Activate(){
		Debug.Log("SKill engaged!");
		isActive = true;
	}
	public void Activate(GameObject target){
		Debug.Log("targeted SKill engaged!");

		skillTarget = target;
		isActive = true;
	}
	private void Deactivate(){
		skillTarget = null;
		isActive = false;
	}

	public string pickAnimation(){
		int randomAnimation = Random.Range(0,animationTriggers.Count);
		Debug.Log("randomAnimation:" + randomAnimation);
		return animationTriggers[randomAnimation];
	}
	#endregion
}