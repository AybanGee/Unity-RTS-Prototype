﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
[CreateAssetMenu (fileName = "New Damageable Ability", menuName = "Ability/Damageable")]
public class DamageableAbility : Ability {
	public int maxHealth = 100;
	public int armour = 0;
	public override void Initialize (GameObject go, int abilityID) {
		Initialize (go.GetComponent<NetworkIdentity> ());
		base.Initialize (go, abilityID);
	}

	 public void Initialize (NetworkIdentity ni) {
		GameObject go = ni.gameObject;
		//Gets Component Unit New
		MonoUnit unit = go.GetComponent<MonoUnit> ();

		Damageable damageable = go.AddComponent<Damageable> ();
		damageable.maxHealth = maxHealth;
		damageable.armour = armour;

		//Add ability origin
		damageable.abilityType = abilityType;
		damageable.interactorAbilities = interactorAbilities;

		//Add it to the unit
		unit.abilities.Add (damageable);
		RpcInitialize (ni);
	}

	[ClientRpc] public void RpcInitialize (NetworkIdentity ni) {
		GameObject go = ni.gameObject;
		//Gets Component Unit New
		MonoUnit unit = go.GetComponent<MonoUnit> ();

		Damageable damageable = go.GetComponent<Damageable> ();
		damageable.maxHealth = maxHealth;
		damageable.armour = armour;

		//Add ability origin
		damageable.abilityType = abilityType;
		damageable.interactorAbilities = interactorAbilities;

		//Add it to the unit
		//unit.abilities.Add (damageable);
	}
}