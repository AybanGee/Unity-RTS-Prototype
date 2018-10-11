using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Building Group", menuName = "Buildings/Building Groups")]
public class BuildingGroups : ScriptableObject {
	public List<Building> buildings = new List<Building>();
}
