using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[CreateAssetMenu (fileName = "New Builder Ability", menuName = "Ability/Builder")]
public class BuilderAbility : Ability {
	public List<BuilderSkill> builderAbilities = new List<BuilderSkill> ();
	public override void Initialize (GameObject go, int abilityID) {
		Initialize (go.GetComponent<NetworkIdentity> ());
		base.Initialize (go, abilityID);

	}
	public void Initialize (NetworkIdentity ni) {
		//Gets Component Unit New
		GameObject go = ni.gameObject;
		MonoUnit unit = go.GetComponent<MonoUnit> ();

		Builder builder = go.AddComponent<Builder> ();
		builder.builderAbilities = builderAbilities;

		//Add ability origin
		builder.abilityType = abilityType;
		builder.interactorAbilities = interactorAbilities;

		//Add it to the unit
		unit.abilities.Add (builder);
		//TODO:Rpc was commented
		//RpcInitialize (ni);
	}

	[ClientRpc] public void RpcInitialize (NetworkIdentity ni) {
		//Gets Component Unit New
		//Gets Component Unit New
		GameObject go = ni.gameObject;
		MonoUnit unit = go.GetComponent<MonoUnit> ();

		Builder builder = go.AddComponent<Builder> ();
		builder.builderAbilities = builderAbilities;

		//Add ability origin
		builder.abilityType = abilityType;
		builder.interactorAbilities = interactorAbilities;

		//Add it to the unit
		//unit.abilities.Add (builder);
	}
}