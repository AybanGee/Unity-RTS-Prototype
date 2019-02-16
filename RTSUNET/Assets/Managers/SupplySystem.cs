using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupplySystem : MonoBehaviour {

	public int supplyManna = 5;
	public float supplyChangeTimer = 0.05f;
	public int supplyChange = 1;
	public int supplyMin = 5;
	public int supplyMax = 50;
	float timer = 0;
	public PlayerObject PO;
	void Awake () {
		PO = GetComponent<PlayerObject> ();
	}

	// Update is called once per frame
	void Update () {
		//Debug.Log(Time.deltaTime);
		timer += supplyChangeTimer * Time.deltaTime;
		if (timer >= 1) {
			if(supplyManna + supplyChange < supplyMax)
			supplyManna += supplyChange;
			else if(supplyManna + supplyChange > supplyMax)
			supplyManna = supplyMax;
			
			timer = 0;
		}
	}
	public void supplyAdd () {
		PO.manna += supplyManna;
	}
}