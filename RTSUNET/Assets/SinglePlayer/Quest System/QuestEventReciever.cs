using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class QuestEventReciever : MonoBehaviour {

	public static QuestEventReciever singleton;
	QuestManager QM;
	void Start () {
		if (QuestEventReciever.singleton == null)
			QuestEventReciever.singleton = this;
		else
			Destroy (this);

		QM = GetComponent<QuestManager>();
	}
	///<summary>
	///This method receives a Quest Event Data to be interpreted by a Quest Manager
	///</summary>
	public void OnReceiveQuestTrigger (QuestEventData questEventData) {
		Debug.Log("data type received:" + questEventData.eventType.ToString());
		QM.currentQuest.OnQuestTrigger(questEventData);
	}

}