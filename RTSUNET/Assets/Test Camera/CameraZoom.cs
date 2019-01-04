using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoom : MonoBehaviour {

	// Use this for initialization\
	public Transform targetPosition;
	public Transform startPosition;
	public float speed = 5;
	public float smoothness = .5f;

	private Vector3 velocity = Vector3.zero;
	void Start(){
		targetPosition = transform.parent;
		startPosition = transform.parent.GetChild(0);
		}
		void Update () {
		float scroll = Input.GetAxis("Mouse ScrollWheel");
		if(Input.GetAxis("Mouse ScrollWheel") > 0f){
			transform.position = Vector3.MoveTowards(transform.position,targetPosition.position,speed);
			//transform.position = Vector3.SmoothDamp(transform.position,targetPosition.position,ref velocity,smoothness,speed);
		}
		if(Input.GetAxis("Mouse ScrollWheel") < 0f){
			transform.position = Vector3.MoveTowards(transform.position,startPosition.position,speed);
			//transform.position = Vector3.SmoothDamp(transform.position,startPosition.position,ref velocity,smoothness,speed);
		}
	

		

	}
}
