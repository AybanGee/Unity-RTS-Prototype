using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public abstract class MonoSkill : NetworkBehaviour {
	public string sName;
	public Sprite sSprite;
	public AudioClip sSound;
    public float range = 3;
	public MonoAbility parentAbility;
	private bool isActive = false;
	private bool isActing = false;
	private GameObject skillTarget;
	public virtual void ActOn (GameObject go) { isActing = true;}
	public virtual void Act () {  isActing = true;}
	public virtual void Stop(){ isActing = false;}
	public bool isTargetInRange(Transform target){
		if(Vector3.Distance(this.transform.position,target.position) <= range) return true;
		return false;
	}
	public void Update() {
		//activates a skill
		if(isActive && skillTarget != null){
			if(isTargetInRange(skillTarget.transform)){
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
	#endregion
}