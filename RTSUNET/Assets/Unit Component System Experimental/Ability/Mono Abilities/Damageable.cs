using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Damageable : MonoAbility {
	//UnityEvent m_Death;

	public int maxHealth = 100;
	[SyncVar (hook = "OnChangeHealth")] public int currentHealth;
	[SyncVar] public int armour = 0;
	public int healthHolder;
	bool isDead = false;

	public HealthUI healthUI;

	public UnityEvent onChangedHealth;
	public UnityEvent onDeath;

	public void Test () {
		Debug.LogError ("Unit Died");
	}

	public void Awake () {
		currentHealth = maxHealth;
		healthHolder = currentHealth;
	}

	public void Start () {
		if (gameObject.GetComponent<HealthUI> () != null)
			healthUI = gameObject.GetComponent<HealthUI> ();

		onChangedHealth = new UnityEvent ();
		onDeath = new UnityEvent ();
		
		if(QuestEventReciever.singleton != null)
		onDeath.AddListener (delegate { QuestEventReciever.singleton.OnReceiveQuestTrigger (new QuestEventData (QuestEventType.Death, parentUnit, this)); });
		//abilityEvent.AddListener (delegate{ QuestEventReciever.singleton.OnReceiveQuestTrigger (new QuestEventData(QuestEventType.Death,parentUnit,this));});

	}

	public void TakeDamage (int damage, NetworkIdentity ni) {

		damage -= armour;
		damage = Mathf.Clamp (damage, 0, int.MaxValue);
		currentHealth -= damage;
		onChangedHealth.Invoke ();
		UpdateHealthUI ();

		if (parentUnit.focus != null) return;
		Attacker attack = GetComponent<Attacker> ();
		Damageable damageable = ni.gameObject.GetComponent<Damageable> ();
		if (attack != null && damageable != null) {
			MonoSkill defaultSkill = attack.defaultSkill ();
			if (defaultSkill == null) return;
			if (Vector3.Distance (attack.transform.position, damageable.transform.position) <= defaultSkill.range) {
				parentUnit.RemoveFocus ();
				parentUnit.SetFocus (damageable.parentUnit, defaultSkill);
			}
		}

		Debug.Log (transform.name + " takes " + damage + " damage.");
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

		if (healthUI == null && !isDead)
			healthUI = gameObject.GetComponent<HealthUI> ();
		else {
			UpdateHealthUI ();
		}

		if (curHealth <= 0 && isDead == false) {

			Die ();
			isDead = true;
		}
	}

	public virtual void Die () {
		Debug.Log (transform.name + " died.");
		onDeath.Invoke ();
		//GetComponent<MonoUnitFramework>().PO.myUnits.Remove(this.gameObject);

		GetComponent<MonoUnitLibrary> ().CmdDeath ();

	}

	void UpdateHealthUI () {
		float fill = (float) currentHealth / (float) maxHealth;
		healthUI.healthSlider.fillAmount = fill;
	}

	/* 	public override void EventSender (QuestEventData questEventData) {
			Debug.Log ("Death :: Local Event");
			base.EventSender (questEventData);
		} */

}