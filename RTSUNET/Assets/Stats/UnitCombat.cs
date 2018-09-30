using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
[RequireComponent (typeof (CharStats))]
public class UnitCombat : NetworkBehaviour {
	public float attackSpeed = 1f;
	CharStats myStats;
	bool isAttacking = false;
	void Start () {

		myStats = GetComponent<CharStats> ();
	}
	public void Attack (UnitStats targetStats) {
Debug.Log("gonna ATTACKING");
		if (targetStats == null) return;
	//	if(!isLocalPlayer) return;

		isAttacking = true;
		StartCoroutine(AttackWithCooldown(targetStats));
	}

	IEnumerator AttackWithCooldown (UnitStats targetStats) {
		while(isAttacking){
			Debug.Log("ATTACKING");
			if(targetStats == null)
			{
				 StopAttack ();
				break;
			}
			
			targetStats.TakeDamage (myStats.damage.GetValue ());

			yield return new WaitForSeconds(attackSpeed);
		}
		yield return null;
	}

	public void StopAttack () {
		isAttacking = false;
		//StopCoroutine ("AttackWithCooldown");
	}

}