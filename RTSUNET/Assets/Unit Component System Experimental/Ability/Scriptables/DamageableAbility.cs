using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Damageable Ability", menuName = "Ability/Damageable")]
public class DamageableAbility : DamageableEnum {
	public int maxHealth = 100;
	public int armour = 0;
	public override void Initialize(GameObject go){
	//Gets Component Unit New
		UnitNew unit = go.GetComponent<UnitNew>();

		Damageable damageable = go.AddComponent<Damageable>();
		damageable.maxHealth = maxHealth;
		damageable.armour = armour;

			//Add it to the unit
		unit.abilities.Add(damageable);
	}
}


[CreateAssetMenu(fileName = "New Attack Ability", menuName = "Ability/Enums/Damageable")]
public class DamageableEnum : Ability{

}
