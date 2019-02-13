using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Quest : ScriptableObject {

	public UnityEvent QuestDoneEvent = new UnityEvent();
	public QuestType questType;
	public string questName;
	[TextArea(3,7)]
	public string instructions;
	/* 	public int currentCount;
		public int maxCount;
	 */
	public bool questIsDone = false;
	
	public virtual void Initialize(){

	}
	public virtual void OnQuestTrigger (QuestEventData questEvent) {

	}

	public virtual void ActivateQuest(){
		questIsDone = false;
	}

	public virtual void QuestDone () {
		QuestDoneEvent.Invoke();
		Debug.LogError("Quest is Done");
		questIsDone = true;

	}

}