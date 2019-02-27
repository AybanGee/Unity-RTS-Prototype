using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
[RequireComponent (typeof (UnitMotor))]
public class MonoUnit : MonoUnitFramework {
	[HideInInspector] public UnitMotor motor;
	[HideInInspector] public UnitType unitType;
	

	public GameObject healthBar;

	//[SerializeField] public List<Ability> primitiveAbilities = new List<Ability> ();

	// Use this for initialization
	//public NetworkIdentity netIdNiPo;
	//public bool hasAuth = false ;
	//PlayerObject po;
	void Start () {
		motor = GetComponent<UnitMotor> ();
	}

	public override bool SetFocus (MonoUnitFramework newFocus, MonoSkill skill) {
		bool returnVal = base.SetFocus(newFocus,skill);
		//VALIDATE FOCUS HERE
		if(returnVal)
		if (newFocus != focus) {
			Debug.Log(" Focus being set ");
			focus = newFocus;
			
			Debug.Log(" Focus to be followed ");
			motor.FollowTarget (newFocus, skill);

			Debug.Log(" skill to be activate ");
			skill.Activate (newFocus.gameObject);
		}

		return returnVal;
	}

	public override void RemoveFocus () {
		base. RemoveFocus ();
		focus = null;
		motor.StopFollowingTarget ();
	}

	public void MoveToPoint (Vector3 vectorMagtanggol) {
		motor.MoveToPoint (vectorMagtanggol);
	}

}