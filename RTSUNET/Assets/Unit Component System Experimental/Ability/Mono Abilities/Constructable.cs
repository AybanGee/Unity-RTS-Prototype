using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constructable : MonoAbility {
	public float constructionTime;
	public float constructionTimeLeft;
	private bool isBuilt = false;

	public void Start () {
		constructionTimeLeft = constructionTime;
	}
	public void Construct (int amount) {
		constructionTimeLeft -= amount;
		if (constructionTimeLeft <= 0) {
			if (isBuilt) {
				Debug.Log("This is already built");
				return;
			}

			isBuilt = true;
			EndConstruct ();

		}
	}

	public void EndConstruct () {
		Debug.Log ("Construction Done!");
		GetComponent<MonoConstructableUnit> ().SpawnBuilding ();
	}

}