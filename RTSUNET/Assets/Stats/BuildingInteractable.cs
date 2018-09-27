using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingInteractable : Interactable {

	BuildingStats myStats;
	UnitCombat enemyCombat;
	bool isAttacking = false;
	new void Start () {
		base.Start ();

		myStats = GetComponent<BuildingStats> ();
	}
	public override void Interact () {

		isAttacking = true;
		enemyCombat = unit.GetComponent<UnitCombat> ();
		Debug.Log ("Attacking Bldg");
		base.Interact ();

	}
	public override void StopInteract () {

		base.StopInteract ();
		isAttacking = false;
		//	unitCombat = null;
	}

	new void Update () {
		base.Update ();

		if (isInteracting)
			if (isAttacking) {
				if (enemyCombat != null)
					enemyCombat.Attack (targetStats: myStats);
			}
		else {
			Debug.Log ("No Unit Combat on Selected Unit");
		}
	}
}
