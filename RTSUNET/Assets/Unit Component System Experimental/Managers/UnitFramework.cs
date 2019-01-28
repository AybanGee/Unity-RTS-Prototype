using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class UnitFramework : ScriptableObject {
	public new string name;
	public Factions faction;
	public int tier;
	public BattleType battleType;
	[TextArea (3, 9)]
	public string description;
	public Sprite artwork;
	public int manaCost = 50;
	public float creationTime = 50;
	public float rangeInfluence = 0;
	public GameObject graphics;
	
    

	public List<Ability> abilities = new List<Ability> ();
	public virtual void Initialize (GameObject go) {
		MonoUnitFramework monoUnit = go.GetComponent<MonoUnitFramework> ();
		if (monoUnit == null) { Debug.LogWarning ("No mono unit framework fouund on initialization"); return; }
		monoUnit.name = name;
		monoUnit.faction = faction;
		monoUnit.tier = tier;
		monoUnit.battleType = battleType;
		monoUnit.description = description;
		monoUnit.artwork = artwork;
		monoUnit.manaCost = manaCost;
		monoUnit.creationTime = creationTime;
		monoUnit.primitiveAbilities = abilities;
		Debug.Log("SPAWNED INFLU"); 
		monoUnit.rangeInfluence = rangeInfluence; 	
		monoUnit.InitAbilities ();
	}

}