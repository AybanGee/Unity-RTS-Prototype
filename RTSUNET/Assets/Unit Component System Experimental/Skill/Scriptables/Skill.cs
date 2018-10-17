using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Skill : ScriptableObject {

    public string aName = "New Ability";
    public Sprite aSprite;
    public AudioClip aSound;
 

    public abstract void Initialize(GameObject obj);
}
