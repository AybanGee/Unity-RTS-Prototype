using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
[RequireComponent(typeof(UnitMotor))]
public class Unit : NetworkBehaviour {
	public GameObject graphics;

	public int team;
	
	public Character focus;

	public UnitMotor motor;

	// Use this for initialization
 //public NetworkIdentity netIdNiPo;
 //public bool hasAuth = false ;
	//PlayerObject po;
	void Start () {
		motor = GetComponent<UnitMotor>();
		
	}

	public void SetFocus(Character newFocus){
		if(newFocus != focus){
			Debug.Log("focus");
			if(focus != null){
				focus.OnDefocused();
				Debug.Log("OnDefocused");
			}
			focus = newFocus;
			motor.FollowTarget(newFocus);
		}
		newFocus.OnFocused(transform);
	}

	public void RemoveFocus(){
			if(focus != null)
			focus.OnDefocused();
		focus = null;
		 motor.StopFollowingTarget();
	}

public void MoveToPoint(Vector3 vectorMagtanggol){
	motor.MoveToPoint(vectorMagtanggol);
}
	
}
