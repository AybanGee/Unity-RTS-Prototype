using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu (fileName = "New Item Quest Destructable", menuName = "Quests/ItemQuestDestructable")]
public class ItemQuestDestructable : ItemQuest {

	int counter = 0;
	PlayerObject PO;
	Coroutine checker;
	public override void Initialize (PlayerObject playerObject) {
		PO = playerObject;
		int spawnIndex = -1;
		if (unit.GetType () == typeof (Building)) {
			spawnIndex = PO.BuildSys.buildingGroups.buildings.IndexOf ((Building) unit);
			if (spawnIndex < 0 || spawnIndex >= PO.BuildSys.buildingGroups.buildings.Count) {
				Debug.Log ("wrong index found!");
				return;
			}

			counter = 0;
			foreach (Transform point in spawnPoints) {
				PO.BuildSys.SpawnBuilding (spawnIndex, point.position, point.rotation);
				GameObject go = PO.BuildSys.lastSpawnedUnit;
				PrepareItem (go);
				counter++;
			}
		} else if (unit.GetType () == typeof (PlayerUnit)) {
			spawnIndex = PO.UnitSys.unitGroup.units.IndexOf ((PlayerUnit) unit);
			if (spawnIndex < 0 || spawnIndex >= PO.UnitSys.unitGroup.units.Count) {
				Debug.Log ("wrong index found!");
				return;
			}

			counter = 0;
			foreach (Transform point in spawnPoints) {
				PO.UnitSys.CmdSpawnObject (spawnIndex, point.position, point.rotation);
				GameObject go = PO.BuildSys.lastSpawnedUnit;
				PrepareItem (go);
				counter++;
			}
		}

	}
	public override void ActivateQuest () {
		base.ActivateQuest ();
		PO.StartCoroutine(checkAllSpawnedIfDead());
	}
	public override void PrepareItem (GameObject go) {
		// Prepare item
		QuestItemDestructable qid = go.AddComponent<QuestItemDestructable> ();
		qid.Prepare (this);
		qid.indexOf = counter;
		base.PrepareItem (go);
	}

	public override void OnQuestTrigger (QuestEventData questEvent) {

		if (questEvent.dataType == QuestEventDataType.Item &&
			questEvent.eventType == QuestEventType.Death &&
			questEvent.questItem != null

		) {
			Debug.Log ("You have triggered an item quest destructable");

		}	

	}
	bool isAllDead = true;
	public IEnumerator checkAllSpawnedIfDead () {

		while (true) {
			isAllDead = true;

			for (int i = 0; i < spawnedUnits.Count; i++) {
				if (spawnedUnits[i] != null) isAllDead = false;

			}
			if (isAllDead) {
				QuestDone ();
			}
			yield return new WaitForSeconds (1);
		}
	}
}