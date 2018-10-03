using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SupplyChainInteractable : BuildingInteractable {
	public float dropDelay = 1;
	BattleType accessType;
	[Command]
	void CmdUpdateManna (NetworkIdentity id, int mannaAmount) {
		id.gameObject.GetComponent<Unit> ().playerObject.manna += mannaAmount;
	}
	public override void Interact (Interactable interactor) {
		//Unit interactor = unit.GetComponent<Unit> ();
		if (accessType == BattleType.Attacking) {
			Debug.Log ("Attacking this bldg Interactor team:" + interactor.GetComponent<Unit> ().team + " -- Building Team:" + GetComponent<BuildingUnit> ().team);
			base.Interact (interactor);
			return;
		} else {
			Debug.Log ("Supplying");
			interactor.isInteracting = true;
			StartCoroutine (setDelay (interactor, interactor.GetComponent<UnitSupply> ()));
		}

	}

	public override void StopInteract (Interactable interactor) {

		
		if (accessType == BattleType.Attacking) {
			base.StopInteract (interactor);
			isAttacking = false;
			enemyCombat.StopAttack ();
			enemyCombat = null;
		}else{
			interactor.hasInteracted = false;
		}

	}
	IEnumerator setDelay (Interactable interactor, UnitSupply unitSupply) {
		yield return new WaitForSeconds (dropDelay);
		UnitSupply supplier = unitSupply;
		if (supplier == null) {
			Debug.LogError ("Builders Unit Supply component missing!");
			yield return null;
		}
		if (supplier.mannaAmount <= 0)
			supplier.StartBehaviour ();

		CmdUpdateManna (interactor.GetComponent<NetworkIdentity> (), supplier.mannaAmount);
		supplier.mannaAmount = 0;

		supplier.StartBehaviour ();
	}

	//Cannot be accessed by enemy Builder
	//Can only be accessed by Builder
	public override bool isValidInteractor (Interactable interactor) {
		if (interactor == null) return false;
		//check if team mate
		Unit unitInteractor = interactor.GetComponent<Unit> ();
		BuildingUnit myUnit = GetComponent<BuildingUnit> ();
		if (unitInteractor == null) return false;
		if (myUnit == null) return false;
		//VALIDATE ACCESS TYPE
		Debug.LogWarning ("Unit " +unitInteractor.team+ " is attacking the building "+myUnit.team+"!");
		if (unitInteractor.team == myUnit.team) {
			//is team mate
			if (unitInteractor.unitType != UnitType.Builder) return false;
			accessType = BattleType.Sustaining;
		} else {
			Debug.LogWarning ("Unit is attacking the building!");
			//is not team mate
			if (unitInteractor.unitType == UnitType.Builder) return false;
			accessType = BattleType.Attacking;
		}

		return true;
	}

}