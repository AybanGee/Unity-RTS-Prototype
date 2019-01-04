using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu (fileName = "New Unit", menuName = "Units/Unit")]
public class PlayerUnit : UnitFramework {
    public UnitType unitType;
    public float speed = 20;
    public override void Initialize (GameObject go) {
        base.Initialize (go);
        MonoUnit unit = go.GetComponent<MonoUnit> ();
        unit.unitType = unitType;
        UnitMotor motor = go.GetComponent<UnitMotor>();
        motor.speed = speed;

    }
}