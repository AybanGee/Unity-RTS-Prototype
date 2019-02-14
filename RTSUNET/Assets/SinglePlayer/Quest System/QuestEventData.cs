using UnityEngine;
public class QuestEventData {
	public QuestEventType eventType;
	public QuestEventDataType dataType = QuestEventDataType.Counter;
	public MonoUnitFramework muf;
	public MonoAbility ability;
	public QuestItem questItem;
	public int indexOf;

	public QuestEventData (QuestEventType et, MonoUnitFramework unit, MonoAbility ab) {
		eventType = et;
		muf = unit;
		ability = ab;
	}

	public QuestEventData (QuestEventType et, MonoUnitFramework unit) {
		eventType = et;
		muf = unit;
	}

	public QuestEventData (QuestEventType et) {
		eventType = et;
	}

	public QuestEventData (QuestEventType et, QuestItem qi, int i) {
		eventType = et;
		dataType = QuestEventDataType.Item;
		questItem = qi;
		indexOf = i;
	}

		public QuestEventData (QuestEventType et, QuestItem qi) {
		eventType = et;
		dataType = QuestEventDataType.Item;
		questItem = qi;
	}

}