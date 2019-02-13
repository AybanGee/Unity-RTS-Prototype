using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CounterQuest : Quest {
	public int requiredCount;
	public int currentCount;

	public void AddCurrentCount () {
		if (questIsDone)
			return;

		currentCount++;

		if (currentCount >= requiredCount)
			QuestDone ();
	}

	public override void ActivateQuest(){
		base.ActivateQuest();
		currentCount = 0;
	}


}