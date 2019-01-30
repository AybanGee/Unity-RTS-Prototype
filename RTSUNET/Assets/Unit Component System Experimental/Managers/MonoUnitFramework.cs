using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public abstract class MonoUnitFramework : NetworkBehaviour {
	 public new string name;
	[HideInInspector] public Factions faction;
	[SyncVar] public int team;
	[HideInInspector] public int tier;
	[HideInInspector] public BattleType battleType;
	[HideInInspector] public string description;
	[HideInInspector] public Sprite artwork;
	[HideInInspector] public int manaCost = 50;
	[HideInInspector] public float creationTime = 50;
	[HideInInspector] public PlayerObject PO;
	/* [HideInInspector] */ public float rangeInfluence = 0;
	public GameObject selectionCircle;




	public List<MonoAbility> abilities = new List<MonoAbility> ();
	public List<Ability> primitiveAbilities = new List<Ability> ();

	[HideInInspector] public PlayerUnit playerUnit;

	
	public void InitAbilities () {
		//nauuna to kesa assign ng value sa primitiveAbilities
		//wait for assign dapat dito
		
		if (primitiveAbilities.Count <= 0) { Debug.LogWarning ("This object does not have any ability!"); return; }
		if (abilities.Count > 0) { Debug.LogWarning ("This object already has initialized abilities!"); return; }
		Debug.Log("Initializing " + primitiveAbilities.Count +	 " abilities");
		//Initialize Abilities
		for (int i = 0; i < primitiveAbilities.Count; i++) {
			primitiveAbilities[i].Initialize (this.gameObject, i);
		}
	}
	public void OnFocused () { }
	public void OnDefocused () { }
	public virtual void SetFocus (MonoUnitFramework newFocus, MonoSkill skill) {
		//if (!newFocus.isValidInteractor (this.GetComponent<Interactable> ())) return;
		//if no target
		if (newFocus == null) { Debug.Log ("Focus is null"); return; }
		if (skill == null) { Debug.Log ("Skill is null"); return; }

		MonoAbility targetAbility = null;
		foreach (MonoAbility ma in newFocus.abilities) {
			if (ma.isValidInteractor (skill.parentAbility)) {
				Debug.Log ("Interactor " + skill.parentAbility.abilityType + " vs Interactable " + ma.abilityType);
				targetAbility = ma;
				break;
			}
		}
//check if target ability is found
		if (targetAbility == null) {
			Debug.Log ("No Applicable Ability");
			return;
		}
		

	}

	public virtual  void RemoveFocus () {
		StopAbilities ();
	}
	public void StopAbilities () {
		if (abilities.Count <= 0) return;
		for (int i = 0; i < abilities.Count; i++) {
			abilities[i].StopSkills ();
		}
	}

	public void OnStartClientAuthority(){
		if(isServer) return;
		playerUnit.Initialize(this.gameObject);
	}

	void Awake () {
		InitAbilities ();
	}

	

	

}