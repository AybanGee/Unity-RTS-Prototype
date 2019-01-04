using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Unit-Faction Group", menuName = "Units/Unit Faction Group")]

public class UnitFactionGroup : ScriptableObject {
public FactionUnitDictionary factionUnitDictionary;
}

[System.Serializable]
public class FactionUnitDictionary : SerializableDictionary<Factions, UnitGroup> {}
