using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotation : MonoBehaviour {

    public GameObject target;//the target object
    private float speedMod = 50.0f;//a speed modifier
    private Vector3 point;//the coord to the point where the camera looks at
 
    void Start () {//Set up things on the start method
        point = target.transform.position;//get target's coords
        transform.LookAt(point);//makes the camera look to it	
		
    }
 
    void Update () {
		point = target.transform.position;

		transform.LookAt(point);
		//makes the camera rotate around "point" coords, rotating around its Y axis, 20 degrees per second times the speed modifier
       // if(Input.GetKey("q"))
		//transform.RotateAround (target.transform.position, Vector3.up, speedMod * Time.deltaTime);

		//if(Input.GetKey("e"))
		//transform.RotateAround (target.transform.position, Vector3.up, -speedMod * Time.deltaTime);
	}	

	

}
