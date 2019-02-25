using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthSlider : MonoBehaviour {
	public Image healthSlider;
	public GameObject GO;
	public GameObject parentUnit;
	// Update is called once per frame
	void LateUpdate () {
		if(healthSlider.fillAmount <= 0 /* || parentUnit == null */){
			Destroy(GO);
		}
	}
}