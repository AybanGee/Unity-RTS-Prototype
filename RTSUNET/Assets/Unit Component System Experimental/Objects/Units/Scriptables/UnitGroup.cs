using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Unit Group", menuName = "Units/Unit Group")]
public class UnitGroup : ScriptableObject {
public List<PlayerUnit> units = new List<PlayerUnit>();
}
