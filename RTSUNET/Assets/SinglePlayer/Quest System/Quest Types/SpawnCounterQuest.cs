using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "New Spawn Counter Quest", menuName = "Quests/SpawnCounterQuest")]
public class SpawnCounterQuest : CounterQuest {
    public UnitFramework requiredObject;
    public override void OnQuestTrigger (QuestEventData questEventData) {
        base.OnQuestTrigger (questEventData);
        if (questEventData.eventType == QuestEventType.Spawn &&
            questEventData.muf.playerUnit == requiredObject &&
            questEventData.muf.team == 1) {
            Debug.Log ("Adding to counter");
            AddCurrentCount ();
        }
    }
}