using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public abstract class Attack : MonoSkill {
	public int damage = 5;
	public float coolDownTime = 1;
	private float currentTime = 0;
	public bool IsAttackOnce = false;
	bool isAttacking = false;
	bool hasAttacked = false;
	Coroutine attackCoroutine;
	MonoUnitLibrary attacker;
	new void Start(){
		attacker = GetComponent<MonoUnitLibrary>();
	}

	new void Update () {
		base.Update();
		if (hasAttacked && currentTime > 0) {
			currentTime -= Time.deltaTime;
		}
	}

	public override void Stop () {
		StopAttack ();
	}

	#region General Attack Types
	//Attack with known target
	public virtual void DoAttack (Damageable[] targetDamageable) {
		Debug.Log ("gonna ATTACKING");
		if (targetDamageable == null) return;

		isAttacking = true;
		if (IsAttackOnce) AttackOnce (targetDamageable);
		else
			attackCoroutine = StartCoroutine (AttackContinuous (targetDamageable));
	}
	//Attack with known target intervaled
	public virtual void DoAttack (Damageable[] targetDamageable, float interval, int count) {
		Debug.Log ("gonna ATTACKING");
		if (targetDamageable == null) return;

		isAttacking = true;
		if (IsAttackOnce) AttackOnce (targetDamageable);
		else
			attackCoroutine = StartCoroutine (AttackIntervaled (targetDamageable, interval, count));
	}
	#endregion

	#region General Attack Action
	//Attack Once
	void AttackOnce (Damageable[] targetDamageable) {
		if (targetDamageable.Length <= 0) return;
		if (currentTime > 0) {
			return;
		}

		currentTime = coolDownTime;
		hasAttacked = true;

		for (int i = 0; i < targetDamageable.Length; i++) {
			attacker.CmdDoDamage (targetDamageable[i].GetComponent<NetworkIdentity> (), damage);
		}
			
			//DO ANIMATION
			GetComponent<CharacterAnimator>().animator.SetTrigger(pickAnimation());

	}

	//Continuous Attacks
	IEnumerator AttackContinuous (Damageable[] targetDamageable) {
		while (isAttacking) {

			if (targetDamageable.Length <= 0) StopAttack ();

			for (int i = 0; i < targetDamageable.Length; i++) {
				Debug.Log ("ATTACKING");
				if (targetDamageable == null) {

					Debug.Log ("Target possible dead");
					continue;
				} else {
					Debug.Log ("GOINH to take damage");

					attacker.CmdDoDamage (targetDamageable[i].GetComponent<NetworkIdentity> (), damage);
				}

			}
			//DO ANIMATION
			if(GetComponent<CharacterAnimator>() != null)
			GetComponent<CharacterAnimator>().animator.SetTrigger(pickAnimation());

			yield return new WaitForSeconds (coolDownTime);
		}
		yield return null;
	}

	//Intervaled Attack
	IEnumerator AttackIntervaled (Damageable[] targetDamageable, float interval, int count) {
		if (count <= 0) { Debug.LogError ("Cannot have a intervaled attack with a count less than one"); yield return null; }
		if (interval <= 0) { Debug.LogError ("Cannot have a intervaled attack with an interval less than one"); yield return null; }
		if (targetDamageable == null) yield return null;
		if (currentTime > 0) yield return null;

		currentTime = coolDownTime;
		for (int i = 0; i <= count; i++) {

			if (targetDamageable.Length <= 0) StopAttack ();

			for (int j = 0; j < targetDamageable.Length; j++) {
				Debug.Log ("ATTACKING");
				if (targetDamageable == null) {

					Debug.Log ("Target possible dead");
					continue;
				} else {
					Debug.Log ("GOINH to take damage");

					attacker.CmdDoDamage (targetDamageable[j].GetComponent<NetworkIdentity> (), damage);
				}

			}

			yield return new WaitForSeconds (interval);
		}
		hasAttacked = true;
		yield return null;
	}
	#endregion



	public void StopAttack () {
		Debug.Log ("Stopped Attacking");
		isAttacking = false;
		if (attackCoroutine != null)
			StopCoroutine (attackCoroutine);
	}

}