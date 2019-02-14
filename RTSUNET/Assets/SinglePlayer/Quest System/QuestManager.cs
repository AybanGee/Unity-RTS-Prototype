using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour {
	public PlayerObject PO;
	public List<Quest> quests = new List<Quest> ();
	public int currentQuestIndex = -1;
	public Quest currentQuest;

	Coroutine questDoneDetector;

	void Start () {
		PrepareQuests ();
	}
	public void PrepareQuests () {
		if (quests.Count <= 0) { Debug.LogWarning ("no quests set!"); return; }
		foreach (Quest quest in quests) {
			quest.Initialize (PO);
			
		}
		//activate first quest
		NextQuest ();
	}

	public void NextQuest () {
		Debug.LogError ("Next quest");

		currentQuestIndex++;
		if (currentQuestIndex >= quests.Count) {
			EndQuests ();
			return;
		}
		currentQuest = quests[currentQuestIndex];
		currentQuest.ActivateQuest ();

		currentQuest.QuestDoneEvent.AddListener (QuestDone);
		questDoneDetector = StartCoroutine (QuestDoneDetector ());

		//Display UI
		if (PO.uiGameManager.questUI != null){
			
			PO.uiGameManager.questUI.gameObject.SetActive(true);
			PO.uiGameManager.questUI.DisplayQuest (currentQuest);
		}
	}

	public void QuestDone () {
		StopCoroutine (questDoneDetector);

		NextQuest ();
	}

	public void EndQuests () {
		Debug.Log ("You win!!");
		PO.CheckForWinner ();
		PO.SetWinner ();

		// TODO insert win here
	}

	IEnumerator QuestDoneDetector () {
		while (true) {
			if (currentQuest.questIsDone)
				QuestDone ();

			if (PO.uiGameManager.questUI != null){
				Debug.Log("Display Quest");
				PO.uiGameManager.questUI.DisplayQuest (currentQuest);
			}

			yield return new WaitForSeconds (1);
		}
	}
}