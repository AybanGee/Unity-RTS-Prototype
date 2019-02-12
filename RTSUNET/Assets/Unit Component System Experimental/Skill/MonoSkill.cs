using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public abstract class MonoSkill : NetworkBehaviour {
	public string sName;
	public Sprite sSprite;
	public string description;
	public AudioClip sSound;
	public float range;
	public List<string> animationTriggers;

	public MonoAbility parentAbility;
	private bool isActive = false;
	private bool isActing = false;
	private GameObject skillTarget;

	public virtual void ActOn (GameObject go) {

		isActing = true;
	}

	public virtual void Act () {
		Debug.Log ("ACT!");
		isActing = true;

	}

	public virtual void Stop () { isActing = false; isActive = false; }
	
	public bool isTargetInRange (Transform target) {
		MonoUnitFramework targetUnit = target.gameObject.GetComponent<MonoUnitFramework> ();
		float influence = 0;
		if (targetUnit != null) {
			influence = targetUnit.rangeInfluence;
		}
		Debug.Log ("Influence " + influence);

		float distance = Mathf.Sqrt ((this.transform.position - target.position).sqrMagnitude);

		if (distance - influence <= range) {
			//Debug.Log("Target in range distance:" + distance + " - " + influence + " = " + (distance - influence) +" req range:" + range);
			return true;
		}
		//Debug.Log("Target not in range distance: " + distance + " - " + influence + " = " + (distance - influence) + " req range:" + range);
		return false;
	}

	public void Update () {
		//activates a skill
		if (isActive && skillTarget != null) {
			Debug.Log ("Skill is Active");

			if (isTargetInRange (skillTarget.transform)) {
				Debug.Log ("Target is in Range");
				ActOn (skillTarget);
				Deactivate ();
			}

		} else if (isActive && skillTarget == null) {
			Act ();
			Deactivate ();
		}
		//checks if target is still in range
		if (isActing && skillTarget != null) {
			if (!isTargetInRange (skillTarget.transform)) {
				Debug.Log ("Out of Range	");

				Stop ();
			}
		}
	}

	#region Activation
	public void Activate () {
		Debug.Log ("SKill engaged!");
		isActive = true;
	}
	public void Activate (GameObject target) {
		Debug.Log ("targeted SKill engaged!");

		skillTarget = target;
		isActive = true;
	}
	private void Deactivate () {
		skillTarget = null;
		isActive = false;
	}

	public string pickAnimation () {
		int randomAnimation = Random.Range (0, animationTriggers.Count);
		Debug.Log ("randomAnimation:" + randomAnimation);
		return animationTriggers[randomAnimation];
	}
	#endregion

	public bool GetIsActing(){
		return isActing;
	}	
	public void SetIsActing(bool input){
		isActing = input;
	}

}