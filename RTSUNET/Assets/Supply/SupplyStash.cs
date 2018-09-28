using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupplyStash : MonoBehaviour {
 public int MannaCapacity = 10000;
 public int MannaAmount = 10000;

public bool isEmpty(){
	if(MannaAmount <= 0){
		return true;
	}else{
		return false;
	}
}
}
