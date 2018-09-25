using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
[RequireComponent (typeof (CharStats))]
public class UnitCombat : NetworkBehaviour {
	public float attackSpeed = 1f;
	public float attackCooldown = 0f;
	CharStats myStats;

	public PlayerObject netPlayer;
	void Start () {

		myStats = GetComponent<CharStats> ();
	}
	public void Attack (CharStats targetStats) {

		if (targetStats == null) return;
		if (attackCooldown <= 0f) {

			//netPlayer.CmdAttack(targetStats.GetComponent<NetworkIdentity>(),this.GetComponent<NetworkIdentity>());
			targetStats.TakeDamage (myStats.damage.GetValue ());
			attackCooldown = 1f / attackSpeed;
		}
		attackCooldown -= Time.deltaTime;
	}

	[Command]
	void CmdAttack (NetworkIdentity ni) {
		//CharStats targetStats = ni.gameObject.GetComponent<CharStats>();

	}

}