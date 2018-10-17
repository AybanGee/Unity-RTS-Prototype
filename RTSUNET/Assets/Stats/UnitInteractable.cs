using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class UnitInteractable : Interactable {
	public CharStats myStats;
	public UnitCombat interactorCombat;
public	bool isAttacking = false;
	new void Start () {
		base.Start ();
		myStats = GetComponent<CharStats> ();
	}
	public override void Interact (Interactable interactor) {
		isAttacking = true;
		interactorCombat = interactor.GetComponent<UnitCombat> ();
		if(interactorCombat == null){
			Debug.Log(" NULL INTERACTOR ");
			return;			
		} 
		interactorCombat.Attack (myStats);
		base.Interact (interactor);
	}
	public override void StopInteract (Interactable interactor) {
		isAttacking = false;
		Debug.Log("STOPPING " + interactor);
		base.StopInteract (interactor);
		interactorCombat.StopAttack ();
		interactorCombat = null;
	}

	public override void OnInteractorFocused(Interactable interactable){
		Debug.Log(interactable + " has been focused to " + this);
	}



	public override bool isValidInteractor (Interactable interactor) {
		if (interactor == null) return false;
		Unit unitInteractor = interactor.GetComponent<Unit> ();
		Unit myUnit = GetComponent<Unit> ();
		if (unitInteractor == null || myUnit == null) return false;
		if (unitInteractor.team == myUnit.team) return false;
		if(unitInteractor.unitType == UnitType.Builder) return false;

		return true;
	}
}