using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurstAttack : Attack {

public float interval = 1;
public int count = 1;
public Vector3 offset;
    private void Start() {
		Initialize();
	}
	public void Initialize(){
	 damage = 5;
	 coolDownTime = 1;
	 IsAttackOnce = false;
	}


    public override void Act()
    {
        base.Act();
        //Get all targets within range
        Collider[] targets  = Physics.OverlapSphere(this.transform.position + offset,range);
        if(targets.Length <= 0){Debug.LogWarning("No targets found on burst attack");return;}
      
        List<Damageable> damageables = new List<Damageable>();
        for (int i = 0; i < targets.Length; i++)
        {
            Damageable damageabletarget = targets[i].gameObject.GetComponent<Damageable>();
            if(damageabletarget != null)
            damageables.Add(damageabletarget);
        }
          Damageable[] targetDamageables = damageables.ToArray();

          DoAttack(targetDamageables,interval,count);
    }
}
