using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent (typeof (CharStats))]
public class UnitInteractable : Interactable {
	CharStats myStats;
	UnitCombat enemyCombat;
	bool isAttacking = false;
	new void Start () {
	base.Start();
		myStats = GetComponent<CharStats> ();
	}
	public override void Interact () {
		isAttacking = true;
		enemyCombat = unit.GetComponent<UnitCombat> ();
		enemyCombat.Attack (targetStats:myStats);
		base.Interact ();
	}
	public override void StopInteract () {
		isAttacking = false;
		base.StopInteract ();
		enemyCombat.StopAttack ();
		enemyCombat = null;
	}
	


}