using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "New Destroy Counter Quest", menuName = "Quests/DestroyCounterQuest")]
public class DestroyCounterQuest : CounterQuest {
	public UnitFramework requiredObject;
	public override void OnQuestTrigger (QuestEventData questEventData) {
		base.OnQuestTrigger (questEventData);
		if (
			questEventData.eventType == QuestEventType.Death &&
			questEventData.muf.playerUnit == requiredObject &&
			questEventData.muf.team != 1) {
			Debug.Log ("Adding to counter");

			//resetquest on Open
			//move quest index on finish
			
			AddCurrentCount ();
		}
	}
}