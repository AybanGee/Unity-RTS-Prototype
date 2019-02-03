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

	public void TakeDamage (int damage,NetworkIdentity ni) {
		
		if (!isServer) return;
		

		damage -= armour;
		damage = Mathf.Clamp (damage, 0, int.MaxValue);
		currentHealth -= damage;
		RpcTakeDamage (GetComponent<NetworkIdentity> (), damage);
		
		if(parentUnit.focus != null) return;
		Attacker attack = GetComponent<Attacker>();
		Damageable damageable = ni.gameObject.GetComponent<Damageable>();
		if(attack != null && damageable != null){
			MonoSkill defaultSkill = attack.skills[0];
			if(defaultSkill == null) return;
			if(Vector3.Distance(attack.transform.position,damageable.transform.position) <= defaultSkill.range){
				parentUnit.RemoveFocus();
				parentUnit.SetFocus(damageable.parentUnit,defaultSkill);
			}
		}


		Debug.Log (transform.name + " takes " + damage + " damage.");
	}

	[ClientRpc] public void RpcTakeDamage (NetworkIdentity targerStatsID, int damage) {
		if (isServer) return;

			Damageable d = targerStatsID.gameObject.GetComponent<Damageable>();
			d.currentHealth -= damage;
			d.healthHolder = currentHealth;

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
		//if (!isServer)
			currentHealth = healthHolder;

		if (healthUI == null)
			healthUI = gameObject.GetComponent<HealthUI> ();
		else {
			RpcUpdateHealthUI ();
		}

		if (curHealth <= 0) {
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
		//Destroy (healthUI.ui.gameObject);
		RpcDeath ();
		Destroy (healthUI.ui.gameObject);
		Destroy (this.gameObject);

	}

	[ClientRpc]
	void RpcDeath () {
		//when used unit stays in client afterdeath
		//	GetComponent<MonoUnitFramework>().PO.RemoveUnit(this.gameObject);
		if(isServer) return; 
		Destroy (healthUI.ui.gameObject);
		Destroy (this.gameObject);

	}

	[ClientRpc]
	void RpcUpdateHealthUI () {
		float fill = (float) currentHealth / (float) maxHealth;
		healthUI.healthSlider.fillAmount = fill;
	}

}