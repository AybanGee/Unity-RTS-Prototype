using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIGameCommandsHandler : MonoBehaviour {

	public GameObject abilityPanel, skillUI, queueDisplay;
	public Image queueTimer;
	public Sprite baseImage;
	QueueingSystem QS;

	PlayerObject PO;
	public void Initialize (PlayerObject playerObject) {
		PO = playerObject;
	}

	//Show Unit Abilities
	public void ShowAbilities (List<MonoAbility> abilities) {
		ClearAbilities ();
		foreach (MonoAbility ability in abilities) {
			//if (ability.skills.Count > 0) {
			//Transform abilityTransform = Instantiate (abilityPanel, Vector3.zero, Quaternion.identity, transform).transform;
			foreach (MonoSkill skill in ability.skills) {
				GameObject _skillUI = Instantiate (skillUI, Vector3.zero, Quaternion.identity, transform);
				Image img = _skillUI.GetComponent<Image> ();
				if (img == null) { Debug.LogError ("NO IMAGE!"); continue; }
				if (skill.sSprite == null) Debug.LogWarning ("NO SPRITE");
				img.sprite = skill.sSprite;
			}
		}
		//}
	}

	//Show Building Abilities or Spawners Ex. Barracks
	public void ShowAbilities (List<PlayerUnit> units, QueueingSystem queue) {
		//ClearQueue ();

		ClearAbilities ();

		//Loop through all Selected Unit
		foreach (PlayerUnit unit in units) {

			//Instantiate ButtonPrefab and Assign Sprites
			GameObject _skillUI = Instantiate (skillUI, Vector3.zero, Quaternion.identity, transform);
			Image img = _skillUI.GetComponent<Image> ();

			//check if there are sprites
			if (img == null) { Debug.LogError ("NO IMAGE!"); continue; }
			if (unit.artwork == null) Debug.LogWarning ("NO SPRITE");
			img.sprite = unit.artwork;

			//Functions Delegates for buttons
			_skillUI.GetComponent<Button> ().onClick.AddListener (delegate {
				int unitIndex = 0;
				unitIndex = units.IndexOf (unit);
				queue.AddToQueue (unitIndex);
			});
		}
	}
	public void ClearAbilities () {
		QS = null;
		foreach (Transform t in transform) {
			Destroy (t.gameObject);
		}
	}

	#region Spawnable Units List
	public void ShowQueue (List<PlayerUnit> units, QueueingSystem queue) {
		//ClearQueue ();

		ClearAbilities ();

		//Loop through all Selected Units
		foreach (PlayerUnit unit in units) {

			//Instantiate ButtonPrefab and Assign Sprites
			GameObject _skillUI = Instantiate (skillUI, Vector3.zero, Quaternion.identity, transform);
			Image img = _skillUI.GetComponent<Image> ();

			//check if there are sprites
			if (img == null) { Debug.LogError ("NO IMAGE!"); continue; }
			if (unit.artwork == null) Debug.LogWarning ("NO SPRITE");
			img.sprite = unit.artwork;

			//Functions Delegates for buttons
			_skillUI.GetComponent<Button> ().onClick.AddListener (delegate {
				int unitIndex = 0;
				unitIndex = units.IndexOf (unit);
				queue.AddToQueue (unitIndex);
			});
		}
	}
	public void ClearQueue () {
		foreach (Transform t in transform) {
			Destroy (t.gameObject);
		}
		queueTimer.fillAmount = 0f;

	}

	public void ShowProcessQueue (List<PlayerUnit> units, QueueingSystem queue) {
		QS = queue;
		int unitIndex = -1;
		ResetQueueDisplay ();
		//Debug.Log("Show Process Queue list count : "+units.Count);
		//Loop through all Selected Unit
		foreach (PlayerUnit unit in units) {

			unitIndex++;
			//Instantiate ButtonPrefab and Assign Sprites
			GameObject _skillUI = queueDisplay.transform.GetChild (unitIndex).gameObject;
			if (_skillUI == null) {
				Debug.Log ("Skill UI is null on " + unitIndex);
			}
			Image img = _skillUI.GetComponent<Image> ();

			//check if there are sprites
			if (img == null) { Debug.LogError ("NO IMAGE!"); continue; }
			if (unit.artwork == null) Debug.LogWarning ("NO SPRITE");
			img.sprite = unit.artwork;
			img.color = new Color (1f, 1f, 1f, 1f);

			//Functions Delegates for buttons
			Button _btn = _skillUI.GetComponent<Button> ();
			//_btn.onClick.RemoveAllListeners();
			/* 	Debug.Log("Setting button to index "+ unitIndex);
				_btn.onClick.AddListener(delegate {
						//unitIndex = units.IndexOf(unit);
					Debug.LogWarning("removing "+ unitIndex);
					queue.RemoveFromQueue(unitIndex);
				}); */
			//if(unitIndex <= 8)
		}
	}

	public void ResetQueueDisplay () {
		foreach (Transform t in queueDisplay.transform) {
			//Destroy(t.gameObject);
			Image img = t.GetComponent<Image> ();
			img.sprite = baseImage;
			img.color = new Color (0f, 0f, 0f, .5f);

			Button _btn = t.GetComponent<Button> ();

			queueTimer.fillAmount = 0f;
			//	_btn.onClick.RemoveAllListeners();	
		}
	}

	public void RemoveFromQueue (int btnIndex) {
		if (QS == null) return;
		QS.RemoveFromQueue (btnIndex);
		//
	}

	void Update () {
		if (QS != null && QS.spawnQueue.Count > 0) {
			queueTimer.fillAmount = QS.creationTimeHolder / QS.spawnQueue[0].creationTime;
		}
	}
	#endregion

	//Show Multiple Units
	public void ShowMultiUnit (List<GameObject> units) {
		ClearQueue ();
		int displayCount = 0;
		//Debug.Log("Unit Count:" + units.Count);
		foreach (GameObject unit in units) {
			//Debug.Log("disp Count : " + displayCount);
			if (displayCount <= 31) {
//				Debug.Log ("in IF");
				//Instantiate ButtonPrefab and Assign Sprites
				GameObject _skillUI = Instantiate (skillUI, Vector3.zero, Quaternion.identity, transform);
				Image img = _skillUI.GetComponent<Image> ();

				//Check if there are Sprites
				if (img == null) { Debug.LogError ("NO IMAGE!"); continue; }
				if (unit.GetComponent<MonoUnit> ().artwork == null) Debug.LogWarning ("NO SPRITE");
				img.sprite = unit.GetComponent<MonoUnit> ().artwork;

				//Functions Delegates for selecting a Specific Unit
				_skillUI.GetComponent<Button> ().onClick.AddListener (delegate {
					GameObject specificUnit = unit.gameObject;
					PO.DeselectAll (new BaseEventData (EventSystem.current));
					specificUnit.GetComponent<UnitSelectable> ().OnSelect (new BaseEventData (EventSystem.current));
					//PO.selectedUnits.Add(specificUnit);
					PO.UpdateUI ();
				});

				if (units.Count > 31 && displayCount == 31) {
					GameObject limit = _skillUI.transform.GetChild (0).gameObject;
					limit.SetActive (true);
					limit.transform.GetComponentInChildren<TextMeshProUGUI> ().text = "+" + (units.Count - 31);
				}

				displayCount++;
			}

		}
	}
	//Show Buildable Buildings
	public void ShowBuildings (BuildingSystem BS) {
	ClearAbilities ();

		//Loop through all Selected Units
		foreach (Building building in BS.buildingGroups.buildings) {

			//Instantiate ButtonPrefab and Assign Sprites
			GameObject _skillUI = Instantiate (skillUI, Vector3.zero, Quaternion.identity, transform);
			Image img = _skillUI.GetComponent<Image> ();

			//check if there are sprites
			if (img == null) { Debug.LogError ("NO IMAGE!"); continue; }
			if (building.artwork == null) Debug.LogWarning ("NO SPRITE");
			img.sprite = building.artwork;

			//Functions Delegates for buttons
			_skillUI.GetComponent<Button> ().onClick.AddListener (delegate {
			
					BS.selectedBuildingIndex = BS.buildingGroups.buildings.IndexOf(building);
					BS.ToggleBuildMode ();
			});
		}
	}

}