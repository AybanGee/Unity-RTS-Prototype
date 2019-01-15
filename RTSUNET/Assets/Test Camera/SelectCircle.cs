using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectCircle : MonoBehaviour {

	// Use this for initialization
	void Start () {
	gameObject.transform.localPosition = new Vector3(0,0,0);
		//float size = gameObject.GetComponentInParent<CapsuleCollider>().radius;
		//Debug.Log(gameObject.GetComponentInParent<CapsuleCollider>().radius);
	transform.localScale = new Vector3(.5f,.5f,0);

	gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
