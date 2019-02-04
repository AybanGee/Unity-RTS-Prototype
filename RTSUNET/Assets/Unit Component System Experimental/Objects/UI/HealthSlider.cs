using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthSlider : MonoBehaviour {
	public Image healthSlider;
	public GameObject GO;
	// Update is called once per frame
	void Update () {
		if(healthSlider.fillAmount <= 0){
			Destroy(GO);
		}
	}
}