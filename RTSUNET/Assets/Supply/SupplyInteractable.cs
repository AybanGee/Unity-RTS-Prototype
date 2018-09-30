using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (SupplyStash))]
public class SupplyInteractable : Interactable {
	SupplyStash supplyStash;
	public float pickupDelay = 1.5f;
	new private void Start () {
		base.Start ();
		supplyStash = GetComponent<SupplyStash> ();
	}
	public override void Interact () {
		base.Interact ();
		UnitSupply unitSupply = unit.GetComponent<UnitSupply> ();
		if (unitSupply == null) {
			Debug.LogError ("Unit Supply Component was not found on Interactor");
		}
		unitSupply.supplyInteract = this;


		if (supplyStash.MannaAmount <= 0) {
			supplyStash.MannaAmount = 0;
			unitSupply.stopInteractions ();
			Debug.LogError ("Unit Supply is empty");
			return;
		}

		StartCoroutine(getDelay(unitSupply));
	}

	IEnumerator getDelay(UnitSupply unitSupply){

		yield return new WaitForSeconds(pickupDelay);
			if (supplyStash.MannaAmount >= unitSupply.mannaCapacity) {
			supplyStash.MannaAmount -= unitSupply.mannaCapacity;
			unitSupply.mannaAmount = unitSupply.mannaCapacity;
		} else {
			unitSupply.mannaAmount = supplyStash.MannaAmount;
			supplyStash.MannaAmount = 0;
		}
		unitSupply.supplyChainInteract = null;
		unitSupply.StartBehaviour ();
	}
}