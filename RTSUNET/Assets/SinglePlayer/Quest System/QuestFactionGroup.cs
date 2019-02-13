using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu (fileName = "New Quest-Faction Group", menuName = "Quests/Quest Faction Group")]
public class QuestFactionGroup : ScriptableObject {
	public QuestUnitDictionary questUnitDictionary;
}

[System.Serializable]
public class QuestUnitDictionary : SerializableDictionary <Factions, QuestGroup> { }