using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestItem : MonoBehaviour {
	QuestEventReciever QER;

	private void Start() {
		QER = QuestEventReciever.singleton;
		GetComponent<Damageable>().abilityEvent.AddListener(delegate{
			EventSender(new QuestEventData(QuestEventType.Death,GetComponent<Damageable>().parentUnit,GetComponent<Damageable>()));
			});
	}

	public void EventSender (QuestEventData questEventData) {
		Debug.Log ("Standard Event");
		QER.OnReceiveQuestTrigger (questEventData);
	}

}