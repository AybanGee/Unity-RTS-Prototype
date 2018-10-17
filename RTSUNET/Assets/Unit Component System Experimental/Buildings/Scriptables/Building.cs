using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Building", menuName = "Buildings/Building")]
public class Building : ScriptableObject {

	public new string name;
	public Factions faction;
	public BattleType battleType;
	public BuildingType type;
	[TextArea]
	public string description;
	public float health = 100;
	
	public Sprite artwork;
	public int manaCost=50;
	public int sellPrice=50;
	public int constructionTime=50;
	public GameObject graphics;

	public Vector3 addedColliderScale;
}
