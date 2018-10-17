using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Unit", menuName = "Units/Unit")]
public class PlayerUnit : UnitFramework {
    public UnitType unitType;
public override void  Initialize(GameObject go){
UnitNew unit = go.GetComponent<UnitNew>();
unit.unitType = unitType;
unit.primitiveAbilities = abilities;
    }
}
