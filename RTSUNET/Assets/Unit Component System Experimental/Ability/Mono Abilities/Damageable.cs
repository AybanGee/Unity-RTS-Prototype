using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Damageable : MonoAbility {
	public int maxHealth = 100;
	[SyncVar (hook = "OnChangeHealth")] public int currentHealth;
	[SyncVar] public int armour = 0;
	public int healthHolder;

	public HealthUI healthUI;

	public void Awake () {
		currentHealth = maxHealth;
		healthHolder = currentHealth;

	}

	public void Start () {
		if (gameObject.GetComponent<HealthUI> () != null)
			healthUI = gameObject.GetComponent<HealthUI> ();

	}

	public void TakeDamage (int damage) {
		if (!isServer) return;
		damage -= armour;
		damage = Mathf.Clamp (damage, 0, int.MaxValue);
		currentHealth -= damage;
		RpcTakeDamage (GetComponent<NetworkIdentity> (), damage);
		Debug.Log (transform.name + " takes " + damage + " damage.");
	}

	[ClientRpc] public void RpcTakeDamage (NetworkIdentity targerStatsID, int damage) {
		if (!isServer) {
			currentHealth -= damage;
			healthHolder = currentHealth;
		}
	}

	public void TakeHealing (int healValue) {
		if (!isServer) return;
		healValue = Mathf.Clamp (healValue, 0, int.MaxValue);
		currentHealth += healValue;
		healthHolder = currentHealth;

		Debug.Log (transform.name + " takes " + healValue + " healing.");

	}
	public virtual void TakeArmour (int value) {
		if (!isServer) return;
		armour += value;
	}

	public virtual void RemoveArmour (int value) {
		if (!isServer) return;
		armour -= value;
		Mathf.Clamp (armour, 0, int.MaxValue);
	}

	public void OnChangeHealth (int curHealth) {
		//if(!hasAuthority)return;
		if (!isServer)
			currentHealth = healthHolder;

		if (healthUI == null)
			healthUI = gameObject.GetComponent<HealthUI> ();
		else {
			float fill = (float) currentHealth / (float) maxHealth;
			//Debug.Log ("Health Percent : " + fill);
			healthUI.healthSlider.fillAmount = fill;
		}

		if (curHealth <= 0) {
			Destroy (healthUI.ui.gameObject);
			Die ();
		}
	}

	public virtual void Die () {
		Debug.Log (transform.name + " died.");
		//GetComponent<MonoUnitFramework>().PO.myUnits.Remove(this.gameObject);
		CmdDeath ();

	}

	[Command]
	void CmdDeath () {
		RpcDeath ();
		Destroy (this.gameObject);

	}

	[ClientRpc]
	void RpcDeath () {
		//when used unit stays in client afterdeath
		//	GetComponent<MonoUnitFramework>().PO.RemoveUnit(this.gameObject);
		Destroy (this.gameObject);

	}

}