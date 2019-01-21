using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SelectCircle : MonoBehaviour {
		float size ;
	// Use this for initialization
	void Start () {
	gameObject.transform.localPosition = new Vector3(0,.1f,0);

	if(gameObject.GetComponentInParent<CapsuleCollider>() != null)
		size = gameObject.GetComponentInParent<CapsuleCollider>().radius;
	if(gameObject.GetComponentInParent<NavMeshObstacle>() != null){
		size = gameObject.GetComponentInParent<NavMeshObstacle>().size.x / 2;
		Debug.Log("size x: " + gameObject.GetComponentInParent<NavMeshObstacle>().size.x);
	}

	transform.localScale = new Vector3(size,size,0);

	gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
