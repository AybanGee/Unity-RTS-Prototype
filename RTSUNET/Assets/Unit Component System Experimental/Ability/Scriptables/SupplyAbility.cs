using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[CreateAssetMenu (fileName = "New Supply Ability", menuName = "Ability/SupplyAbility")]
public class SupplyAbility : Ability {

	//public int supplyManna = 100;
	public float interval = 5;
	//public float supplyChangeTimer = 0.002; //magpplus ng magpplus hanngang mag 1 tapos mag ddeccrease na si supply mana 
	//public int supplyChange = -1;

	public override void Initialize (GameObject go, int abilityID) {
		Initialize (go.GetComponent<NetworkIdentity> ());
		base.Initialize (go, abilityID);
	}

	public void Initialize (NetworkIdentity ni) {
		GameObject go = ni.gameObject;
		//Gets Component Unit New
		MonoUnitFramework unit = go.GetComponent<MonoUnitFramework> ();

		Supplier supplier = go.AddComponent<Supplier> ();
		supplier.interval = interval;

		//Add ability origin
		supplier.abilityType = abilityType;
		supplier.interactorAbilities = interactorAbilities;

		unit.abilities.Add (supplier);
		RpcInitialize (ni);
	}

	[ClientRpc] public void RpcInitialize (NetworkIdentity ni) {
		GameObject go = ni.gameObject;
		//Gets Component Unit New
		MonoUnitFramework unit = go.GetComponent<MonoUnitFramework> ();

		Supplier supplier = go.AddComponent<Supplier> ();
		supplier.interval = interval;

		//Add ability origin
		supplier.abilityType = abilityType;
		supplier.interactorAbilities = interactorAbilities;

		unit.abilities.Add (supplier);
	}
}