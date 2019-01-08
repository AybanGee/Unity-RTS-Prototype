using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Map List", menuName = "Maps/Map List")]

public class MapDictionary : ScriptableObject {
	public List<Map> Maps = new List<Map>();
}
