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
	new void Start () {
		attacker = GetComponent<MonoUnitLibrary> ();
	}

	new void Update () {
		base.Update ();
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
		GetComponent<CharacterAnimator> ().SetTrigger (pickAnimation ());

	}

	//Continuous Attacks
	IEnumerator AttackContinuous (Damageable[] targetDamageable) {
		while (isAttacking) {

			if (targetDamageable.Length <= 0) StopAttack ();

			for (int i = 0; i < targetDamageable.Length; i++) {
				Debug.Log ("ATTACKING");
				if (targetDamageable[i] == null) {

					Debug.Log ("Target possible dead");
					if (targetDamageable.Length == 1) {
						//seearch for new target

						parentAbility.parentUnit.RemoveFocus ();

						SearchForNewTarget ();

					}
					continue;
				} else {
					Debug.Log ("GOINH to take damage");

					attacker.CmdDoDamage (targetDamageable[i].GetComponent<NetworkIdentity> (), damage);
					//DO ANIMATION
					if (GetComponent<CharacterAnimator> () != null)
						GetComponent<CharacterAnimator> ().SetTrigger (pickAnimation ());

				}

			}

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
				if (targetDamageable[i] == null) {
					parentAbility.parentUnit.focus = null;
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

	public void SearchForNewTarget () {
		if (parentAbility.parentUnit.focus != null) return;
		// TODO
		// Improvement check if there is target. 
		//if yes dont try to find a new target which means to say return;
		Collider[] hitColliders = Physics.OverlapSphere (transform.position, range + 3);
		List<Damageable> damageables = new List<Damageable> ();

		for (int i = 0; i < hitColliders.Length; i++) {
			Damageable dmgHolder = hitColliders[i].gameObject.GetComponent<Damageable> ();
			if (dmgHolder != null) {
				if (dmgHolder.isValidInteractor (parentAbility))
					damageables.Add (dmgHolder);
			}
		}

		if (damageables.Count > 0) {
			int newTargetIndex = Random.Range (0, damageables.Count);
			GetComponent<MonoUnitFramework> ().SetFocus (damageables[newTargetIndex].parentUnit, this);
		}

	}
	public bool GetIsActive(){
		return isAttacking;
	}

}