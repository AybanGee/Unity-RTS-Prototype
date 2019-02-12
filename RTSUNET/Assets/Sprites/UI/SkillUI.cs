using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SkillUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
	PlayerUnit unit;
	Building building;
	Skill skill;
	public ToolTip tooltip;//one object
	public ToolTipData ttd;
	public bool OnHover = false;

	float screenWidth;
	float screenHeight;
	private void Start () {
		//set tooltip reference
		screenWidth = Screen.width;
		screenHeight = Screen.height;
	}
	private void Update () {
		if (OnHover) {
			Vector2 mousePos = Input.mousePosition;
			tooltip.transform.position = mousePos;

			if (mousePos.x > screenWidth / 2 && mousePos.y > screenHeight / 2) {
				tooltip.gameObject.GetComponent<RectTransform> ().pivot = new Vector2 (1, 1);
			} else if (mousePos.x <= screenWidth / 2 && mousePos.y > screenHeight / 2) {

				tooltip.gameObject.GetComponent<RectTransform> ().pivot = new Vector2 (0, 1);
			} else if (mousePos.x > screenWidth / 2 && mousePos.y < screenHeight / 2) {

				tooltip.gameObject.GetComponent<RectTransform> ().pivot = new Vector2 (1, 0);
			} else if (mousePos.x <= screenWidth / 2 && mousePos.y <= screenHeight / 2) {

				tooltip.gameObject.GetComponent<RectTransform> ().pivot = new Vector2 (0, 0);
			}

		}
	}
	public void OnPointerEnter (PointerEventData eventData) {
		Debug.Log ("Enter");
		OnHover = true;
		tooltip.ShowToolTip(ttd);
		tooltip.gameObject.SetActive (true);
	}

	public void OnPointerExit (PointerEventData eventData) {
		Debug.Log ("Exit");
		OnHover = false;
		tooltip.gameObject.SetActive (false);

	}



}