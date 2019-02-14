using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemQuest : Quest {
	public UnitFramework unit;
	public List<Transform> spawnPoints;
	[HideInInspector]
	public List<GameObject> spawnedUnits = new List<GameObject>();

	/// <summary>
	/// Meant to be overriden! Initializes and spawns quest items in the game.
	/// </summary>
	/// <param name="spawnPoints">List of transform where items will be placed</param>
	/// <param name="PO">Reference to the Player Object for spawning</param>
	public override void Initialize(PlayerObject PO){
	
	}
	/// <summary>
	/// Meant to be overriden! Prepares a game object then adds it to the spawned units
	/// </summary>
	/// <param name="go">game object to be prepared</param>
	public virtual void PrepareItem(GameObject go){
		spawnedUnits.Add(go);
	}

	
	

	
}