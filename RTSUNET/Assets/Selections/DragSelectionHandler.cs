using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;

public class DragSelectionHandler : NetworkBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler {

	[SerializeField]
	Image selectionBoxImage;
	Vector2 startPosition;
	Rect selectionRect;
	public static DragSelectionHandler singleton;
	public PlayerObject playerObject;
	void Awake () {
		singleton = this;
	}
	private void Start () {
		
		
		// GameObject po = LobbyManager.singleton.client.connection.playerControllers[0].gameObject;
	
		// playerObject = po.GetComponent<PlayerObject> ();
		// if (playerObject == null) {
		// 	playerObject = PlayerObject.singleton;
		// }
		// if (playerObject == null) {
		// 	//if player object still still null
		// 	Debug.LogError ("Player Object not found in DragSelectionHandler!");
		// 	return;
		// }
	

	}
	public void AssignPlayerObject(PlayerObject po){
		playerObject = po;

		Color boxColor = LobbyManager.singleton.GetComponent<LobbyManager>().gameColors.gameColorList()[playerObject.colorIndex];
		boxColor.a = 1f;
		selectionBoxImage.color = boxColor;
	}
	// public void LateStart () {
	// 	if (playerObject != null) return;

	// 	PlayerObject[] pos = FindObjectsOfType<PlayerObject> ();
	// 	if (pos.Length <= 0) {
	// 		Debug.LogError ("Cannot find on client");
	// 		return;
	// 	}
	// 	for (int i = 0; i < pos.Length; i++) {
	// 		if (pos[i].singleton != null) {
	// 			playerObject = pos[i].singleton;
	// 			break;
	// 		}
	// 	}

	// 	if (playerObject == null) {
	// 		//if player object still still null
	// 		Debug.LogError ("Player Object not found in CLIENT DragSelectionHandler!");
	// 		return;
	// 	}
	// 	Color boxColor = playerObject.selectedColor[playerObject.team - 1];
	// 	boxColor.a = 1f;
	// 	selectionBoxImage.color = boxColor;
	// }
	public void OnBeginDrag (PointerEventData eventData) {
		if(!Input.GetMouseButton(0))return;
		if (!Input.GetKey (KeyCode.LeftControl) && !Input.GetKey (KeyCode.RightControl))
			playerObject.DeselectAll (new BaseEventData (EventSystem.current));
		selectionBoxImage.gameObject.SetActive (true);
		startPosition = eventData.position;
		selectionRect = new Rect ();
	}

	public void OnDrag (PointerEventData eventData) {
		if(!Input.GetMouseButton(0))return;
		if (eventData.position.x < startPosition.x) {
			selectionRect.xMin = eventData.position.x;
			selectionRect.xMax = startPosition.x;
		} else {
			selectionRect.xMin = startPosition.x;
			selectionRect.xMax = eventData.position.x;
		}

		if (eventData.position.y < startPosition.y) {
			selectionRect.yMin = eventData.position.y;
			selectionRect.yMax = startPosition.y;
		} else {
			selectionRect.yMin = startPosition.y;
			selectionRect.yMax = eventData.position.y;
		}
		selectionBoxImage.rectTransform.offsetMin = selectionRect.min;
		selectionBoxImage.rectTransform.offsetMax = selectionRect.max;

	}

	public void OnEndDrag (PointerEventData eventData) {
		if(!Input.GetMouseButtonUp(0))return;
		selectionBoxImage.gameObject.SetActive (false);
		List<GameObject> units = playerObject.myUnits;
		foreach (GameObject unit in units) {
			if (selectionRect.Contains (Camera.main.WorldToScreenPoint (unit.transform.position))) {
				unit.GetComponent<UnitSelectable> ().OnSelect (eventData);
			}
		}
	}

	public void OnPointerClick (PointerEventData eventData) {
	//	if(!Input.GetMouseButton(0))return;

		List<RaycastResult> results = new List<RaycastResult> ();
		EventSystem.current.RaycastAll (eventData, results);

		float myDistance = 0;

		foreach (RaycastResult result in results) {
			if (result.gameObject == gameObject) {
				myDistance = result.distance;
				break;
			}
		}

		GameObject nextObject = null;
		float maxDistance = Mathf.Infinity;
		foreach (RaycastResult result in results) {
			if (result.distance > myDistance && result.distance < maxDistance) {
				nextObject = result.gameObject;
				maxDistance = result.distance;
			}

		}

		if (nextObject) {
			ExecuteEvents.Execute<IPointerClickHandler> (nextObject, eventData, (x, y) => { x.OnPointerClick ((PointerEventData) y); });
		}
	}
}