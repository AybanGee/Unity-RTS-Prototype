using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (BuildingStats))]
public class BuildingInteractable : Interactable {

	public BuildingStats myStats;
	public UnitCombat enemyCombat;
	public bool isAttacking = false;
	new void Start () {
		base.Start ();
		myStats = GetComponent<BuildingStats> ();
	}
	public override void Interact (Interactable interactor) {
		isAttacking = true;
		enemyCombat = interactor.GetComponent<UnitCombat> ();
		if(enemyCombat == null){
			Debug.Log(" NULL INTERACTOR ");
			return;			
		} 
		enemyCombat.Attack (targetStats:myStats);
		Debug.Log ("Attacking Bldg");
		base.Interact (interactor);

	}
	public override void StopInteract (Interactable interactor) {

		base.StopInteract (interactor);
		isAttacking = false;
		base.StopInteract(interactor);
		enemyCombat.StopAttack ();
		enemyCombat = null;
	}
	public override bool isValidInteractor (Interactable interactor) {
		if (interactor == null) return false;
		//check if team mate
		Unit unitInteractor = interactor.GetComponent<Unit> ();
		BuildingUnit myUnit = GetComponent<BuildingUnit> ();
		if (unitInteractor == null) return false;
		if (myUnit == null) return false;
		//VALIDATE ACCESS TYPE
		Debug.LogWarning ("Unit " +unitInteractor.team+ " is attacking the building "+myUnit.team+"!");
		if (unitInteractor.team == myUnit.team) return false;

		return true;
	}
	
}
