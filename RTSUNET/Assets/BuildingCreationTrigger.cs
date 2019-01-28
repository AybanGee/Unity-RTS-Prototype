using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingCreationTrigger : MonoBehaviour {
	//[HideInInspector]
	public int colliderCount;

	void OnTriggerEnter (Collider other) {
		if (other.gameObject.GetComponent<MonoBuilding> () != null ||
			other.gameObject.GetComponent<ConstructionInteractable> () != null ||
			other.gameObject.GetComponent<BuildingCreationTrigger> () != null) {
				
			colliderCount++;
		}
	}
	private void OnTriggerExit (Collider other) {
		if (other.gameObject.GetComponent<MonoBuilding> () != null ||
			other.gameObject.GetComponent<ConstructionInteractable> () != null ||
			other.gameObject.GetComponent<BuildingCreationTrigger> () != null) {

			colliderCount--;
		}
	}
}