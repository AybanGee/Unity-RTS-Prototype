using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetedAttack : Attack {
    public override void ActOn(GameObject go) {
        Debug.Log("using Targeted Attack!");
        Damageable[] targetDamageable = {go.GetComponent<Damageable>()};
        DoAttack (targetDamageable);
    }
    private new void Update() {
	base.Update();
	}
    public override void Stop(){
        base.Stop();
    }
}