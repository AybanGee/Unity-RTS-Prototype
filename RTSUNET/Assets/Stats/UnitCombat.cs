using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class UnitCombat : NetworkBehaviour {
	public float attackSpeed = 1f;
	CharStats myStats;
	bool isAttacking = false;
	Coroutine attackCoroutine;
	void Start () {

		myStats = GetComponent<CharStats> ();
	}
	public void Attack (UnitStats targetStats) {
		Debug.Log ("gonna ATTACKING");
		if (targetStats == null) return;
		//	if(!isLocalPlayer) return;

		isAttacking = true;
		attackCoroutine = StartCoroutine (AttackWithCooldown (targetStats));
	}

	IEnumerator AttackWithCooldown (UnitStats targetStats) {
		while (isAttacking) {
			Debug.Log ("ATTACKING");
			if (targetStats == null) {
				StopAttack ();
				Debug.Log ("Target possible dead");
				break;
			} else {
				Debug.Log ("GOINH to take damage");

				CmdTakeDamage (targetStats.GetComponent<NetworkIdentity> (), myStats.damage.GetValue ());
			}

			yield return new WaitForSeconds (attackSpeed);
		}
		yield return null;
	}

	[Command]
	void CmdTakeDamage (NetworkIdentity targerStatsID, int damage) {
		targerStatsID.gameObject.GetComponent<UnitStats> ().TakeDamage (damage);
	}

	public void StopAttack () {
		Debug.Log ("Stopped Attacking");
		isAttacking = false;
		StopCoroutine (attackCoroutine);
		GetComponent<Interactable> ().isInteracting = false;
		GetComponent<Interactable> ().hasInteracted = false;
	}

}