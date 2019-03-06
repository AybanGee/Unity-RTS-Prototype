using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AlmanacLoader : MonoBehaviour {

	public GameObject gfxHolder, buttonHolder, buttonPrefab, contentHolder;
	public TextMeshProUGUI name, description;
	public List<UnitFramework> units = new List<UnitFramework> ();
	public Scrollbar verticalScroll;

	// Use this for initialization
	void Start () {
		foreach (UnitFramework unit in units) {
			GameObject button = Instantiate (buttonPrefab, Vector3.zero, Quaternion.identity);
			button.transform.SetParent (buttonHolder.transform);
			button.transform.localScale = new Vector3 (1, 1, 1);

			AlmanacButton btn = button.GetComponent<AlmanacButton> ();

			btn.unit = unit;
			btn.almanacLoader = this;
			btn.buttonName.text = unit.name;

			button.GetComponent<Button> ().onClick.AddListener (delegate {
				btn.OnClick ();
			});
		}

		//ResetSelected();
	}

	public void ChangeSelected (UnitFramework unit) {

		if (gfxHolder.transform.GetChild (0) != null)
			Destroy (gfxHolder.transform.GetChild (0).gameObject);
			
		name.text = unit.name;
		description.text = unit.description + "\n" + unit.descriptionExtra;
		GameObject gfx = Instantiate (unit.graphics, Vector3.zero, Quaternion.identity);

		gfx.transform.SetParent (gfxHolder.transform);

		gfx.transform.position = new Vector3 (0, 0, 0);

		contentHolder.GetComponent<RectTransform>().ForceUpdateRectTransforms();
		Canvas.ForceUpdateCanvases();
		verticalScroll.value = 1f;

	}

	public void ResetSelected () {
		ChangeSelected (units[0]);
	}

	public void RemoveGraphic () {
		gfxHolder.transform.GetChild (0).gameObject.SetActive(false);
	}

}