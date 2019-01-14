using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueueingSystem : MonoBehaviour {

	//queue for spawning units
	public List<Unit> spawnQueue = new List<Unit> ();
	//available units in this building
	public List<Unit> spawnableUnits = new List<Unit> ();
	//
	public Vector3 spawnPoint;
	public Vector3 rallyPoint;

	void Start () {
		//Set Spawn Point and Rally Point

	}

	// IEnumerator FindingBaseLocation () {
	// 	foreach (Unit u in spawnQueue) {
	// 		while (spawnTime! < 1) { //ano meaning nung !< ? 
	// 			//Spawn Unit

	// 			//Move Unit to Rallypoint

	// 			yield return null;
	// 		}
	// 		yield return null;
	// 	}
	// }
}