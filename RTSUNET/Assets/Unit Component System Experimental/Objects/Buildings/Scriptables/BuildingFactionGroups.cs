using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Building-Faction Group", menuName = "Buildings/Building Faction Groups")]
public class BuildingFactionGroups : ScriptableObject {
public FactionBuildingDictionary buildingFactionDictionary;




}
[System.Serializable]
public class FactionBuildingDictionary : SerializableDictionary<Factions, BuildingGroups> {}



