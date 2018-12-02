using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Attacker : MonoAbility {

	[SerializeField]
	public List<AttackSkill> attackAbilities = new List<AttackSkill>();
	void Start(){
		if(attackAbilities.Count > 0){
			Debug.Log("Initialize");
			for (int i = 0; i < attackAbilities.Count; i++)
			{
				attackAbilities[i].Initialize(this.gameObject);
			}
		}
		else{
			Debug.Log("No Initialize");
		}
	}

}
