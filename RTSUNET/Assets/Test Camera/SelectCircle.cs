﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SelectCircle : MonoBehaviour {
	float size;
	bool sizeIsSet = false;
	// Use this for initialization
	void Start () {
		gameObject.transform.localPosition = new Vector3 (0, .1f, 0);
		StartCoroutine (SetCircleSize ());
		transform.localScale = new Vector3 (size, size, 0);

		gameObject.SetActive (false);
	}

	IEnumerator SetCircleSize () {
		while (!sizeIsSet) {
			if (gameObject.GetComponentInParent<CapsuleCollider> () != null)
				size = gameObject.GetComponentInParent<CapsuleCollider> ().radius;
			if (gameObject.GetComponentInParent<NavMeshObstacle> () != null) {
				size = gameObject.GetComponentInParent<NavMeshObstacle> ().size.x / 2;
				Debug.Log ("size x: " + gameObject.GetComponentInParent<NavMeshObstacle> ().size.x);
			}
			yield return null;
		}
		yield return null;
	}

	// Update is called once per frame
	void Update () {

	}
}