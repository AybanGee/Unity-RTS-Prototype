using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetedAttack : Attack {
    public override void ActOn (GameObject go) {
        base.ActOn (go);
        Debug.Log ("using Targeted Attack!");
        Damageable[] targetDamageable = { go.GetComponent<Damageable> () };
        
        if (targetDamageable[0] != null)
            DoAttack (targetDamageable);
    }
    private new void Update () {
        base.Update ();
    }
    public override void Stop () {
        base.Stop ();
    }
}