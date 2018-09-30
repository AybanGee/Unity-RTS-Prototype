﻿using System.Collections;
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
		enemyCombat.Attack (targetStats:myStats);
		Debug.Log ("Attacking Bldg");
		base.Interact ();

	}
	public override void StopInteract () {

		base.StopInteract ();
		isAttacking = false;
		enemyCombat.StopAttack ();
		enemyCombat = null;
	}

	
}
