using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Target Attack Skill", menuName = "Skill/Attacking/Target Attack")]
public class TargetedAttackSkill : AttackSkill {

	public override void Initialize(GameObject obj)
	{	
		//Gets component attacker
	Attacker attacker = obj.GetComponent<Attacker>();

	TargetedAttack ta;
    ta  =   obj.AddComponent<TargetedAttack>();
	ta.damage = this.damage;
	ta.coolDownTime = this.AttackCooldown;
	ta.IsAttackOnce = this.isAttackOnce;

	attacker.attackTypes.Add(ta);
    }

}
