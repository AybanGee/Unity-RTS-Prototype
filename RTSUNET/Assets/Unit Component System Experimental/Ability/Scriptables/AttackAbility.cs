using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu (fileName = "New Attack Ability", menuName = "Ability/Attacker")]
public class AttackAbility : AttackEnum {
	public List<AttackSkill> attackAbilities = new List<AttackSkill> ();
	public override void Initialize (GameObject go) {
		//Gets Component Unit New
		UnitNew unit = go.GetComponent<UnitNew>();

		Attacker attacker = go.AddComponent<Attacker> ();
		attacker.attackAbilities = attackAbilities;
	//Add it to the unit
		unit.abilities.Add(attacker);
	}
}

[CreateAssetMenu (fileName = "New Attack Ability", menuName = "Ability/Enums/Attacker")]
public class AttackEnum : Ability {

}