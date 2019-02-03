using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingCreationTrigger : MonoBehaviour {
	//[HideInInspector]
	public int colliderCount;
	public bool isOre = false;
	public bool inRange = false;

	void OnTriggerEnter (Collider other) {

		//Debug.Log ("Collided with : " + other.gameObject);
		if (other.gameObject.GetComponent<MonoBuilding> () != null ||
			other.gameObject.GetComponent<ConstructionInteractable> () != null ||
			other.gameObject.GetComponent<BuildingCreationTrigger> () != null) {

			//Debug.Log ("Entered first test");

			if (other.gameObject.GetComponent<BuildingCreationTrigger> ().isOre == false) {

				//Debug.Log ("Entered second test "+ other.gameObject.name);
				colliderCount++;
			}

			if (other.gameObject.GetComponent<BuildingCreationTrigger> ().isOre) {
				inRange = true;
			}
		}

	}
	private void OnTriggerExit (Collider other) {

		//Debug.Log ("Exit Collided with : " + other.gameObject);
		if (other.gameObject.GetComponent<MonoBuilding> () != null ||
			other.gameObject.GetComponent<ConstructionInteractable> () != null ||
			other.gameObject.GetComponent<BuildingCreationTrigger> () != null) {

			if (other.gameObject.GetComponent<BuildingCreationTrigger> ().isOre == false)
				colliderCount--;

			if (other.gameObject.GetComponent<BuildingCreationTrigger> ().isOre) {
				inRange = false;
			}
		}

	}
}