using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class UnitFramework : ScriptableObject {
	public new string name;
	public Factions faction;
	public int tier;
	public BattleType battleType;
	[TextArea(3,9)]
	public string description;
	public Sprite artwork;
	public int manaCost = 50;
	public int creationTime = 50;
	public GameObject graphics;

	public List<Ability> abilities = new List<Ability> ();
	public virtual void Initialize(GameObject go){}


}