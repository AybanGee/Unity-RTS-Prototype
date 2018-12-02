using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Skill : ScriptableObject {

    public string sName = "New Skill";
    public Sprite sSprite;
    public AudioClip sSound;
    public float range = 3;


    public virtual void Initialize(GameObject obj){
        Debug.Log("Initializing skill");
        MonoSkill ms = obj.GetComponent<MonoSkill>();
        ms.sName = sName;
        ms.sSprite = sSprite;
        ms.sSound = sSound;
        MonoAbility ma = obj.GetComponent<MonoAbility>();
        ms.parentAbility = ma;

    }
}
