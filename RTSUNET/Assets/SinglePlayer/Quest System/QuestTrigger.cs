using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "New Quest", menuName = "Quests/QuestTrigger")]
public class QuestTrigger : ScriptableObject {

	public GameObject GO;
	public Quest quest;

	public void TriggerQuest () {
		quest.questDone = true;
	}

	public void AddToQuest () {
		quest.counter++;
	}
}