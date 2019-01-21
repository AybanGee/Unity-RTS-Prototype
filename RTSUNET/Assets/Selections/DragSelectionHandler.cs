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
	public PlayerObject playerObject;
	

	
	public void AssignPlayerObject(PlayerObject po){
		playerObject = po;

		Color boxColor = LobbyManager.singleton.GetComponent<LobbyManager>().gameColors.gameColorList()[playerObject.colorIndex];
		boxColor.a = 1f;
		selectionBoxImage.color = boxColor;


	}
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
			//HACK!
			if(unit == null)
			continue;
			if (selectionRect.Contains (Camera.main.WorldToScreenPoint (unit.transform.position))) {
				UnitSelectable us = unit.GetComponent<UnitSelectable> ();
				if(us.isOneSelection) continue;
				us.OnSelect (eventData);
			}
		}
		UpdateUI();
		
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
		UpdateUI();
	}
	public void UpdateUI(){
		playerObject.UpdateUI();
	}
}