using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Attacker : MonoAbility {

	[SerializeField]
	public List<AttackSkill> attackAbilities = new List<AttackSkill>();
	void Start(){
		if(attackAbilities.Count > 0){
			Debug.Log("Initialize Skills:" + attackAbilities.Count);
			foreach(AttackSkill skill in attackAbilities)
			{
				skill.Initialize(this.gameObject,this);
				Debug.Log(skill.name);
			}
			
		}
		else{
			Debug.Log("No Initialize");
		}
	}



}
