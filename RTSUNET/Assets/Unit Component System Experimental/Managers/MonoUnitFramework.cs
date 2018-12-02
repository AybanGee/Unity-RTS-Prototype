using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public abstract class MonoUnitFramework : NetworkBehaviour {
	[HideInInspector] public new string name;
	[HideInInspector] public Factions faction;
	[HideInInspector][SyncVar] public int team;
	[HideInInspector] public int tier;
	[HideInInspector] public BattleType battleType;
	[HideInInspector] public string description;
	[HideInInspector] public Sprite artwork;
	[HideInInspector] public int manaCost = 50;
	[HideInInspector] public int creationTime = 50;

	public List<MonoAbility> abilities = new List<MonoAbility> ();
	public List<Ability> primitiveAbilities = new List<Ability> ();
	
	public void InitAbilities () {
		if (primitiveAbilities.Count <= 0) { Debug.LogWarning ("This object does not have any ability!"); return; }
		if (abilities.Count > 0) { Debug.LogWarning ("This object already has initialized abilities!"); return; }

		//Initialize Abilities
		for (int i = 0; i < primitiveAbilities.Count; i++) {
			Debug.Log ("Initialize ability " + i);
			primitiveAbilities[i].Initialize (this.gameObject, i);
		}
	}
	public void OnFocused () { }
	public void OnDefocused () { }

	public void StopAbilities () {
		if (abilities.Count <= 0) return;
		for (int i = 0; i < abilities.Count; i++) {
			abilities[i].StopSkills ();
		}
	}

}