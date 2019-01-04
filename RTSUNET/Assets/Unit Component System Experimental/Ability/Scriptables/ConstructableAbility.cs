using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
[CreateAssetMenu (fileName = "New Constructable Ability", menuName = "Ability/Constructable")]
public class ConstructableAbility : Ability {
public int constructionTime;
public Building building;
	public override void Initialize (GameObject go, int abilityID) {
		Initialize (go.GetComponent<NetworkIdentity> ());
		base.Initialize (go, abilityID);
	}

	 public void Initialize (NetworkIdentity ni) {
		GameObject go = ni.gameObject;
		//Gets Component Unit New
		MonoUnit unit = go.GetComponent<MonoUnit> ();

		Constructable constructable = go.AddComponent<Constructable> ();
		constructable.constructionTime = constructionTime;

		//Add ability origin
		constructable.abilityType = abilityType;
		constructable.interactorAbilities = interactorAbilities;

		//Add it to the unit
		unit.abilities.Add (constructable);
		RpcInitialize (ni);
	}

	[ClientRpc] public void RpcInitialize (NetworkIdentity ni) {
		GameObject go = ni.gameObject;
		//Gets Component Unit New
		MonoUnit unit = go.GetComponent<MonoUnit> ();

			Constructable constructable = go.AddComponent<Constructable> ();
		constructable.constructionTime = constructionTime;

		//Add ability origin
		constructable.abilityType = abilityType;
		constructable.interactorAbilities = interactorAbilities;

		//Add it to the unit
		//unit.abilities.Add (damageable);
	}


}
