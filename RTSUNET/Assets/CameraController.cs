
using UnityEngine;
using UnityEngine.Networking;

public class CameraController : MonoBehaviour {
	public float panSpeed = 50f;
	public float panBorderThickness = 10f;
	public Vector2 panMax;
	public Vector2 panMin;
	public float scrollSpeed = 20f;
	public float minY = 20f ,maxY = 150f;
	// Update is called once per frame
	void LateUpdate () {
if(!Application.isFocused)
return;
		Vector3 pos = transform.position;

		if(Input.GetKey("w")||Input.mousePosition.y >= Screen.height - panBorderThickness){
			Debug.Log("W");
			pos.z += panSpeed * Time.deltaTime;
		}
		if(Input.GetKey("s")||Input.mousePosition.y <= panBorderThickness){
			pos.z -= panSpeed * Time.deltaTime;
		}
		if(Input.GetKey("d")||Input.mousePosition.x >= Screen.width - panBorderThickness){
			pos.x += panSpeed * Time.deltaTime;
		}
		if(Input.GetKey("a")||Input.mousePosition.x <= panBorderThickness){
			pos.x -= panSpeed * Time.deltaTime;
		}

		float scroll = Input.GetAxis("Mouse ScrollWheel");

		pos.y -= scroll * scrollSpeed * 100f * Time.deltaTime;

		pos.x = Mathf.Clamp(pos.x, panMin.x, panMax.x);
		pos.z = Mathf.Clamp(pos.z,  panMin.y, panMax.y);
		pos.y = Mathf.Clamp(pos.y, minY, maxY);


		transform.position = pos;
	}
}
