using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestUIHandler : MonoBehaviour {
	public TextMeshProUGUI questName, questDescription, questCount;
	public GameObject questDisplay;
	

	public void DisplayQuest(Quest quest){
		questName.text = quest.questName + ":";
		questDescription.text = quest.instructions;
	}
	
	public void DisplayQuest(CounterQuest quest){
		questName.text = quest.questName + ":";
		questDescription.text = quest.instructions;
		//questCount.text = quest.currentCount + "/" + quest.requiredCount;
	}
}
