using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Unit))]
public class UnitSupply : MonoBehaviour {
	public Interactable supplyChainInteract;
	public Interactable supplyInteract;
	Unit myUnit;
	public int mannaCapacity = 10;

	public int mannaAmount = 0;
	PlayerObject playerObject;
	//status variables
	public bool isGettingSupply;
	public bool isDeliveringSupply;
	public bool isSupplying;

	void Start () {
		myUnit = GetComponent<Unit> ();
		if (myUnit == null) {
			Debug.LogError ("Unit Component cannot be found!");
			return;
		}
		playerObject = myUnit.playerObject;
	}
	// Use this for initialization
	public void setAssignedSupply (Interactable supply) {
		supplyInteract = supply;
	}
	SupplyChainInteractable searchNearestSupplyChain () {
		if (supplyInteract == null) {
			Debug.LogError ("Cannot Search for nearest supply chain if Supply Interact is not assigned");
			return null;
		}
		List<GameObject> supplyChainResults = new List<GameObject> ();
		foreach (GameObject building in playerObject.myBuildings) {
			if (building.GetComponent<BuildingUnit> ().buildingType == BuildingType.SupplyChain) {
				supplyChainResults.Add (building);
			}
		}
		if (supplyChainResults.Count <= 0) {
			Debug.Log ("You do not have a supply chain yet");
			return null;
		}
		//FINDING THE NEAREST
		float topDistance = float.MaxValue;
		int topIndex = -1;
		for (int i = 0; i < supplyChainResults.Count; i++) {
			float currentDistance = Vector3.Distance (supplyChainResults[i].transform.position, supplyInteract.transform.position);
			if (currentDistance < topDistance) {
				topIndex = i;
				topDistance = currentDistance;
			}
		}
		if (topIndex < 0) {
			Debug.LogError ("Did not find nearest Supply Chain");
			return null;
		}
		SupplyChainInteractable resultInteractable = supplyChainResults[topIndex].GetComponent<SupplyChainInteractable> ();
		if (resultInteractable == null) {
			Debug.LogError ("Did not find Interactable Component on Supply Chain");
			return null;
		}

		return resultInteractable;
	}
	void deliverSupply () {
		
		//check if supply chain is assigned , Assigns if not
		if (supplyChainInteract == null) {
			supplyChainInteract = searchNearestSupplyChain ();
			if (supplyChainInteract == null) {
				stopInteractions ();
				return;
			}
		}
		isDeliveringSupply = true;
		isGettingSupply = false;
		myUnit.SetFocus (supplyChainInteract);

	}

	void getSupply () {
		if (supplyInteract == null) {
			Debug.LogError ("No assigned supply stash to Interact");
			return;
		}

		isDeliveringSupply = false;
		isGettingSupply = true;
		myUnit.SetFocus (supplyInteract);
	}

	public void stopInteractions () {
		myUnit.RemoveFocus ();
		isSupplying = isGettingSupply = isDeliveringSupply = false;
		supplyInteract = null;
	}

	//Starts the cycle for getting supplies
	public void StartBehaviour () {
				myUnit.RemoveFocus ();
		isSupplying = true;
		if (mannaAmount > 0) {
			Debug.Log("Unit delivering supply");
			deliverSupply ();
		} else {
			Debug.Log("Unit getting supply");
			getSupply ();
		}
	}
}