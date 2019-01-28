using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;

[CreateAssetMenu (fileName = "New Unit", menuName = "Units/Unit")]
public class PlayerUnit : UnitFramework {
    public UnitType unitType;
    public float speed = 20;
    public AnimatorController controller;
    public Avatar avatar;


    public override void Initialize (GameObject go) {
        base.Initialize (go);
        MonoUnit unit = go.GetComponent<MonoUnit> ();
        unit.unitType = unitType;
        UnitMotor motor = go.GetComponent<UnitMotor>();
        motor.speed = speed;
        Animator animator = go.GetComponent<Animator>();
        //animator.Controller = controller;
        //animator.Avatar = avatar;

    }
}