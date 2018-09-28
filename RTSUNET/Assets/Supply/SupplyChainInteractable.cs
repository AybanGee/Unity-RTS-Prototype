using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SupplyChainInteractable : BuildingInteractable {

	[Command]
	void CmdUpdateManna (NetworkIdentity id, int mannaAmount) {
		id.gameObject.GetComponent<Unit> ().playerObject.manna += mannaAmount;
	}
	public override void Interact () {
		Unit interactor = unit.GetComponent<Unit> ();
		if (interactor.team != GetComponent<BuildingUnit> ().team) {
			Debug.Log("Attacking this bldg Interactor team:" + interactor.team  +" -- Building Team:" + GetComponent<BuildingUnit> ().team);
			base.Interact ();
			return;
		}
		if (interactor.unitType == UnitType.Builder && interactor.team == GetComponent<BuildingUnit> ().team) {
			Debug.Log("Supplying");
			//base.Interact();
			UnitSupply supplier = interactor.GetComponent<UnitSupply> ();
			if (supplier == null) {
				Debug.LogError ("Builders Unit Supply component missing!");
				return;
			}
			if (supplier.mannaAmount <= 0)
				supplier.StartBehaviour ();

			CmdUpdateManna (interactor.GetComponent<NetworkIdentity> (), supplier.mannaAmount);
			supplier.mannaAmount = 0;

			supplier.StartBehaviour ();
		}

	}

}