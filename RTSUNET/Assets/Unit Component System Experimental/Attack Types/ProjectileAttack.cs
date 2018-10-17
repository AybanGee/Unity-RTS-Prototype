using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ProjectileAttack : Attack {
    public float projectionHeight;
    public GameObject projectileGraphics;
    private Transform projectile;
    public bool isBurst = false;
    
    void Start(){
        CheckProjectileValidity();
    }

    public override void ActOn (GameObject go) {
        //change to command call
        Launch (go.transform);
    }
    //called after instantiating the projectile
    Vector3 LaunchVelocity (Transform target) {
        float gravity = Physics.gravity.y;
        float height = projectionHeight;
        float displacementY = target.position.y - projectile.position.y;
        Vector3 displacementXZ = new Vector3 (target.position.x - projectile.position.x, 0, target.position.z - projectile.position.z);

        Vector3 velocityY = Vector3.up * Mathf.Sqrt (-2 * gravity * height);
        Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt (-2 * height / gravity) + Mathf.Sqrt (2 * (displacementY - height) / gravity));
        return velocityXZ + velocityY;
    }

    void Launch (Transform target) {
            //Network code here for instantiating the projectile game Object

        projectile.GetComponent<Rigidbody> ().velocity = LaunchVelocity (target);
    
    }

    void CheckProjectileValidity () {
        if (projectile == null) { Debug.LogError ("Projectile not set"); return; }
        Attacker attac = projectile.GetComponent<Attacker> ();
        if (attac == null) { Debug.LogError ("Projectile set has no attacker component"); return; }
        if (isBurst) {
            BurstAttackSkill burstAttack = projectile.GetComponent<BurstAttackSkill> ();
            if (burstAttack == null) { Debug.LogError ("Projectile is set as Burst Attack but burst attack component not set"); return; }
        } else {
            TargetedAttackSkill singleAttacc = projectile.GetComponent<TargetedAttackSkill> ();
            if (singleAttacc == null) { Debug.LogError ("Projectile is set as Targeted Attack but Targeted attack component not set"); return; }

        }
    }
}