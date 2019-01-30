using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour {
	public GameObject uiPrefab;
	public Transform target;
	public Image healthSlider;
	public Transform ui;
	public bool isBuilding;

	Transform cam;
	// Use this for initialization
	void Start () {
		cam = Camera.main.transform;
		foreach (Canvas c in FindObjectsOfType<Canvas> ()) {
			if (c.renderMode == RenderMode.WorldSpace) {
				ui = Instantiate (uiPrefab, c.transform).transform;
				healthSlider = ui.GetChild (0).GetComponent<Image> ();
				break;
			}
		}

		if (isBuilding) {
			target = transform.GetChild (0).GetChild (transform.GetChild (0).transform.childCount - 1).transform;
		}
	}

	// Update is called once per frame
	void Update () {

		if (target == null)
			target = transform.GetChild (0).GetChild (transform.GetChild (0).transform.childCount - 1).transform;
	
		ui.position = target.position;
		ui.forward = -cam.forward;
	}
}