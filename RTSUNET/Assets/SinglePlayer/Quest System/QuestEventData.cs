public class QuestEventData {
	public QuestEventType eventType;
	public MonoUnitFramework muf;
	public MonoAbility ability;

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

}