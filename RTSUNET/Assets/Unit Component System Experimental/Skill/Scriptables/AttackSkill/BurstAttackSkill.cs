using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Target Attack Skill", menuName = "Skill/Attacking/Burst Attack")]
public class BurstAttackSkill : AttackSkill {

public float interval = 1;
public int count = 1;
public Vector3 offset;
	public override void Initialize(GameObject obj)
	{	
	Attacker attacker = obj.GetComponent<Attacker>();

	BurstAttack ba;
    ba  =   obj.AddComponent<BurstAttack>();
	ba.damage = this.damage;
	ba.coolDownTime = this.AttackCooldown;
	ba.IsAttackOnce = this.isAttackOnce;
	ba.interval = this.interval;
	ba.count = this.count;
	

	attacker.skills.Add(ba);
	
		base.Initialize(obj);
    }

}
