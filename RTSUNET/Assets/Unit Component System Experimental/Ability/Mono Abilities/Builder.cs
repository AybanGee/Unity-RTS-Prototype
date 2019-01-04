using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Builder : MonoAbility {

	[SerializeField]
	public List<BuilderSkill> builderAbilities = new List<BuilderSkill> ();
	void Start () {
		if (builderAbilities.Count > 0) {
			Debug.Log ("Initialize");
			for (int i = 0; i < builderAbilities.Count; i++) {
				builderAbilities[i].Initialize (this.gameObject,this);
				Debug.Log(builderAbilities[i].name);
			}
		} else {
			Debug.Log ("No Initialize");
		}
	}
}