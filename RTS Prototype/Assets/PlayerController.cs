using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
	public Camera cam;
	public LayerMask movementMask;
	public List<Unit> selectedUnits = new List<Unit>();
	float ang;
	// Update is called once per frame
	void Update() {


		
		if(Input.GetMouseButtonDown(1)){
			Ray ray = cam.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
				Debug.Log("C L I C K !");

			if(Physics.Raycast(ray,out hit,10000,movementMask)){
				Debug.Log("We Hit" +  hit.collider.name + " " + hit.point );
				//Point of Click and location of selected Unit = angle, for formation
				
				// Move our player to what we hit
				//Get angle
					ang = Vector3.Angle(selectedUnits[0].GetComponent<Transform>().position,hit.point);
					Debug.Log("Angle:" + (ang*10));
				moveUnits(hit.point);
				
				//Stop focusing other objects
			}
		}
	}

	float offsetSize = 2;
	int perRow = 6;
	void moveUnits(Vector3 hit){
		int rowCount = (-1)*(perRow/2),colCount = 0;
		for (int i = 0; i < selectedUnits.Count; i++)
		{	
			float x,z;
			if(rowCount >= perRow/2){
				rowCount = (-1)*(perRow/2);
				colCount ++;
			}	
			
			x = offsetSize * rowCount;
			z = offsetSize * colCount;

			/**float xR= x * Mathf.Cos(ang) - z * Mathf.Sin(ang);
			float zR= z * Mathf.Cos(ang) - x * Mathf.Sin(ang);
			**/

			Vector3 offset = new Vector3(x,0,z);
			selectedUnits[i].MoveToPoint(hit + offset);

			rowCount ++;
		}
	}
}










