using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour {
	// Use this for initialization
	void Start () {
		Vector3 newSpawnPoint = new Vector3 (this.transform.position.x, 0, this.transform.position.z);
		//Debug.Log("new Spawn Point : "+ newSpawnPoint );
		if (transform.parent.parent.GetComponent<QueueingSystem> () != null)
			transform.parent.parent.GetComponent<QueueingSystem> ().SetSpawnPoint (newSpawnPoint);
	}
}