using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Builder Skill", menuName = "Skill/Building")]

public class BuilderSkill : Skill
{
    public int buildRate = 1;
    public float buildSpeed = 1f;

    public override void Initialize(GameObject go, MonoAbility ma){
       Builder builder = go.GetComponent<Builder>();
        Build b = go.AddComponent<Build>();
        b.buildRate =  buildRate;
        b.buildSpeed = buildSpeed;
        builder.skills.Add(b);
             Debug.Log("Init:" + b);
         base.Initialize(go,ma);
    }
}
