
using UnityEngine;
using UnityEngine.Networking;

public class UnitStats : NetworkBehaviour {

	public int maxHealth = 100;
	[SyncVar(hook="OnChangeHealth")]
	public int currentHealth;
	public Stat damage;
	public Stat armor;

	public void Awake(){
		currentHealth = maxHealth;
	}
	public void TakeDamage (int damage)
	{
		if(!isServer)return;
		damage -= armor.GetValue();
		damage = Mathf.Clamp(damage, 0 , int.MaxValue);
		currentHealth -= damage;
		Debug.Log(transform.name + " takes "+damage+" damage.");

		if(currentHealth <= 0)
			Die();
	
	}

	public void OnChangeHealth(int curHealth){
		//if(!hasAuthority)return;
		if(curHealth <= 0)
			Die();
	}

	public virtual void Die(){
		Debug.Log(transform.name + " died.");
	}

}
