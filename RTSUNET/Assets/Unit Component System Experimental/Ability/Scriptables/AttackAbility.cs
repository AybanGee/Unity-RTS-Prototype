using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
[CreateAssetMenu (fileName = "New Attack Ability", menuName = "Ability/Attacker")]
public class AttackAbility : Ability {
	public List<AttackSkill> attackAbilities = new List<AttackSkill> ();
	public override void Initialize (GameObject go, int abilityID) {
		Initialize(go.GetComponent<NetworkIdentity>());
		base.Initialize (go,abilityID);

	}
	public void Initialize(NetworkIdentity ni){
		//Gets Component Unit New
		GameObject go = ni.gameObject;
		MonoUnit unit = go.GetComponent<MonoUnit> ();

		Attacker attacker = go.AddComponent<Attacker> ();
		attacker.attackAbilities = attackAbilities;

		//Add ability origin
        attacker.abilityType = abilityType;
        attacker.interactorAbilities = interactorAbilities;	

		//Add it to the unit
		unit.abilities.Add (attacker);
		RpcInitialize(ni);
	}

	[ClientRpc] public void RpcInitialize(NetworkIdentity ni){
			//Gets Component Unit New
		GameObject go = ni.gameObject;
		MonoUnit unit = go.GetComponent<MonoUnit> ();

		Attacker attacker = go.GetComponent<Attacker> ();
		attacker.attackAbilities = attackAbilities;

		//Add ability origin
        attacker.abilityType = abilityType;
        attacker.interactorAbilities = interactorAbilities;	

		//Add it to the unit
		//unit.abilities.Add (attacker);
	}
}