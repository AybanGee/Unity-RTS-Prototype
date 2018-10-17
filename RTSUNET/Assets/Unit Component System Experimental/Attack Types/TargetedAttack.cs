using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetedAttack : Attack {
    public override void ActOn(GameObject go) {
        Damageable[] targetDamageable = {go.GetComponent<Damageable>()};
        DoAttack (targetDamageable);
    }
}