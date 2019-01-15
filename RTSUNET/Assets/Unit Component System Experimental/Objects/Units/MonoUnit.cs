using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
[RequireComponent (typeof (UnitMotor))]
public class MonoUnit : MonoUnitFramework {
	[HideInInspector] public MonoUnitFramework focus;
	[HideInInspector] public UnitMotor motor;
	[HideInInspector] public UnitType unitType;

	public GameObject selectionCircle;
	public GameObject healthBar;

	//[SerializeField] public List<Ability> primitiveAbilities = new List<Ability> ();

	// Use this for initialization
	//public NetworkIdentity netIdNiPo;
	//public bool hasAuth = false ;
	//PlayerObject po;
	void Start () {
		motor = GetComponent<UnitMotor> ();
	}

	public void SetFocus (MonoUnitFramework newFocus, MonoSkill skill) {
		//if (!newFocus.isValidInteractor (this.GetComponent<Interactable> ())) return;
		//if no target
		if (newFocus == null) { Debug.Log ("Focus is null"); return; }
		if (skill == null) { Debug.Log ("Skill is null"); return; }

		MonoAbility targetAbility = null;
		foreach (MonoAbility ma in newFocus.abilities) {
			if (ma.isValidInteractor (skill.parentAbility)) {
				Debug.Log ("Interactor " + skill.parentAbility.abilityType + " vs Interactable " + ma.abilityType);
				targetAbility = ma;
				break;
			}
		}
//check if target ability is found
		if (targetAbility == null) {
			Debug.Log ("No Applicable Ability");
			return;
		}
		//VALIDATE FOCUS HERE
		if (newFocus != focus) {
			focus = newFocus;
			motor.FollowTarget (newFocus, skill);
			skill.Activate (newFocus.gameObject);
		}

	}

	public void RemoveFocus () {
		StopAbilities ();
		focus = null;
		motor.StopFollowingTarget ();
	}

	public void MoveToPoint (Vector3 vectorMagtanggol) {
		motor.MoveToPoint (vectorMagtanggol);
	}

}