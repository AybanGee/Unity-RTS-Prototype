using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
[RequireComponent (typeof (UnitMotor))]
public class UnitNew : NetworkBehaviour {
	[HideInInspector] public int team;
	[HideInInspector] public Interactable focus;
	[HideInInspector] public UnitMotor motor;
	[HideInInspector] public UnitType unitType;
	[HideInInspector] public PlayerObject playerObject;

	[SerializeField] public List<Ability> primitiveAbilities = new List<Ability>();
	[HideInInspector] public List<MonoAbility> abilities = new List<MonoAbility>();

	// Use this for initialization
	//public NetworkIdentity netIdNiPo;
	//public bool hasAuth = false ;
	//PlayerObject po;
	void Start () {
		motor = GetComponent<UnitMotor> ();
		if(primitiveAbilities.Count <= 0 ){Debug.LogWarning("This object does not have any ability!");return;}
		//Initialize Abilities
		for (int i = 0; i < primitiveAbilities.Count; i++)
		{
			primitiveAbilities[i].Initialize(this.gameObject);
		}

	}

	public void SetFocus (Interactable newFocus) {
		if (!newFocus.isValidInteractor (this.GetComponent<Interactable> ())) return;
		if (newFocus != focus) {
			//Debug.Log(this.name + " is focusing on " + newFocus);
			if (focus != null) {
				//Debug.Log("FOCUS is null assigning" + GetComponent<Interactable>());
				focus.OnDefocused (GetComponent<Interactable> ());
				Debug.Log ("OnDefocused");
			}
			focus = newFocus;
			motor.FollowTarget (newFocus);
		}
		//Debug.Log("New Focus " + GetComponent<Interactable>());
		newFocus.OnFocused (GetComponent<Interactable> ());
	}

	public void RemoveFocus () {
		if (focus != null) {

			focus.OnDefocused (GetComponent<Interactable> ());
			Debug.Log (" has a focus to be defocused!!!");

		}
		focus = null;
		motor.StopFollowingTarget ();
	}

	public void MoveToPoint (Vector3 vectorMagtanggol) {
		motor.MoveToPoint (vectorMagtanggol);
	}

}