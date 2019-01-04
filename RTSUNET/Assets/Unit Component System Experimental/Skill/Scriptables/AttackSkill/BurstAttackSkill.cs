using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Target Attack Skill", menuName = "Skill/Attacking/Burst Attack")]
public class BurstAttackSkill : AttackSkill {

public float interval = 1;
public int count = 1;
public Vector3 offset;
	public override void Initialize(GameObject obj,MonoAbility ma)
	{	
	Attacker attacker = obj.GetComponent<Attacker>();

	BurstAttack ba;
    ba  =   obj.AddComponent<BurstAttack>();
	ba.damage = damage;
	ba.coolDownTime = AttackCooldown;
	ba.IsAttackOnce = isAttackOnce;
	ba.interval = interval;
	ba.count = count;

	attacker.skills.Add(ba);

	Debug.Log("Init"+this.name);
	base.Initialize(obj,ma);
	
		
    }

}
