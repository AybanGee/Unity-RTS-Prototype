using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class QuestItemDestructable : QuestItem {

	public override void Prepare (Quest q) {
		base.Prepare (q);
		Damageable d = GetComponent<Damageable> ();
		if(d == null){Debug.LogError("No Damageable found on unit!"); return;}
		GetComponent<Damageable> ().isInteractable = false;
		GetComponent<Damageable> ().onDeath.AddListener(delegate{
			Trigger(new QuestEventData(QuestEventType.Death,this));
		});

	}
	public override void Activate () {
		base.Activate ();
		GetComponent<Damageable> ().isInteractable = true;
	}
}