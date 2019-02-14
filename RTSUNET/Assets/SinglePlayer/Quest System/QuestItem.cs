using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestItem : MonoBehaviour {
	QuestEventReciever QER;

	public int indexOf;
	Quest quest;
	private void Start () {
		QER = QuestEventReciever.singleton;
		if (QER == null) {
			Debug.LogError ("Quest Event Receiver not found!");
			return;
		}

	}
	///<summary>
	///Method meant to be overriden. Initializes the QuestItem for interaction
	///</summary>
	public virtual void Prepare (Quest q) {
		Debug.Log ("Quest item initializing!");

		quest = q;
	}

	///<summary>
	///Method meant to be overriden. Activates the object to accept interactions
	///</summary>
	public virtual void Activate () {
		Debug.Log ("Quest item initializing!");
	}

	///<summary>
	///Method meant to be overriden. Triggers the object and sends event to the Quest Event System
	///</summary>
	public virtual void Trigger (QuestEventData questEventData) {
		EventSender (questEventData);
	}
	void EventSender (QuestEventData questEventData) {
		QER.OnReceiveQuestTrigger (questEventData);
	}

}