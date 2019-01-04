using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Skill : ScriptableObject {

    public string sName = "New Skill";
    public Sprite sSprite;
    public AudioClip sSound;
    public float range = 3;
    public List<string> animationTriggers;

    public virtual void Initialize(GameObject obj, MonoAbility ma){
        Debug.Log("Initializing skill:" + sName);
        //TODO ensure that you are getting the apprpriate ability
        MonoSkill ms = ma.skills[ma.skills.Count - 1];
        ms.sName = sName;
        ms.sSprite = sSprite;
        ms.sSound = sSound;
        ms.animationTriggers = animationTriggers;
        ms.parentAbility = ma;

        Debug.Log("Animaion Triggers:" + ms.animationTriggers.Count);

    }
}


