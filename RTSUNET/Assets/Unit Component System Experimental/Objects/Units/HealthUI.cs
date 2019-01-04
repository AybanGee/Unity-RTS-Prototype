using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour {

	public GameObject uiPrefab;
	public Transform target;

	Transform ui;
	Image healthSlider;

	public Transform cam;

	// Use this for initialization
	void Start () {
		//cam = Camera.RTS_Camera.transform;

		foreach (Canvas c in FindObjectsOfType<Canvas>())
		{
			if(c.renderMode ==  RenderMode.WorldSpace)
			{
				ui = Instantiate(uiPrefab,c.transform).transform;
				healthSlider = ui.GetChild(0).GetComponent<Image>();
				break;
			}
		}
	}
	
	// Update is called once per frame
	void LateUpdate () {
		ui.position = target.position;
		ui.forward = -cam.forward;

	}
}
