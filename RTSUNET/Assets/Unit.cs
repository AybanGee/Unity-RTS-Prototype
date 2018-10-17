using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
[RequireComponent(typeof(UnitMotor))]
public class Unit : NetworkBehaviour {
	public GameObject graphics;

	public int team;
	
[HideInInspector]	public Interactable focus;
[HideInInspector]	public UnitMotor motor;

	public UnitType unitType;

[HideInInspector]	public PlayerObject playerObject;

	// Use this for initialization
 //public NetworkIdentity netIdNiPo;
 //public bool hasAuth = false ;
	//PlayerObject po;
	void Start () {
		motor = GetComponent<UnitMotor>();
		
	}

	public void SetFocus(Interactable newFocus){
		if(!newFocus.isValidInteractor(this.GetComponent<Interactable>()))return;
		if(newFocus != focus){
			//Debug.Log(this.name + " is focusing on " + newFocus);
			if(focus != null){
				//Debug.Log("FOCUS is null assigning" + GetComponent<Interactable>());
				focus.OnDefocused(GetComponent<Interactable>());
				Debug.Log("OnDefocused");
			}
			focus = newFocus;
			motor.FollowTarget(newFocus);
		}
		//Debug.Log("New Focus " + GetComponent<Interactable>());
		newFocus.OnFocused(GetComponent<Interactable>());
	}

	public void RemoveFocus(){
			if(focus != null){
				
			focus.OnDefocused(GetComponent<Interactable>());
			Debug.Log(" has a focus to be defocused!!!");

			}
		focus = null;
		 motor.StopFollowingTarget();
	}

public void MoveToPoint(Vector3 vectorMagtanggol){
	motor.MoveToPoint(vectorMagtanggol);
}
	
}
