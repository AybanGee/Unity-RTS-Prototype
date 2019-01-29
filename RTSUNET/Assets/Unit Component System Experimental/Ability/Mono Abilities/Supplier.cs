using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Supplier : MonoAbility {
public float interval;
Coroutine ticker; 
SupplySystem SS;

	// Use this for initialization
	void Start () {

		SS = GetComponent<MonoUnitFramework>().PO.GetComponent<SupplySystem>();

		if(SS == null)
		Debug.LogError("No supply system found in Supplier Ability of " + gameObject.name);

		
		ticker = StartCoroutine(supplyAdder());
	}
	
	IEnumerator supplyAdder(){
		while(true){
			SS.supplyAdd();
			yield return new WaitForSeconds(interval);
		}
	}
}
