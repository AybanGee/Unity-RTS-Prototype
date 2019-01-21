using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class UIGameCommandsHandler : MonoBehaviour {

	public GameObject abilityPanel, skillUI,queueDisplay;

	PlayerObject PO;
	public void Initialize(PlayerObject playerObject){
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
				Image img = _skillUI.GetComponent<Image>();
				if(img == null){Debug.LogError("NO IMAGE!");continue;}
				if(skill.sSprite == null) Debug.LogWarning("NO SPRITE");
				img.sprite = skill.sSprite;		
				}
			}
//}
	}

	//Show Building Abilities or Spawners Ex. Barracks
	public void ShowAbilities (List<PlayerUnit> units,QueueingSystem queue) {
		//ClearQueue ();
		
		ClearAbilities ();

		//Loop through all Selected Unit
		foreach (PlayerUnit unit in units) {
			
		//Instantiate ButtonPrefab and Assign Sprites
			GameObject _skillUI = Instantiate (skillUI, Vector3.zero, Quaternion.identity, transform);
			Image img = _skillUI.GetComponent<Image>();

			//check if there are sprites
			if(img == null){Debug.LogError("NO IMAGE!");continue;}
			if(unit.artwork == null) Debug.LogWarning("NO SPRITE");
				img.sprite = unit.artwork;		

		//Functions Delegates for buttons
			_skillUI.GetComponent<Button>().onClick.AddListener(delegate {
				int unitIndex = 0;
					unitIndex = units.IndexOf(unit);
				queue.AddToQueue(unitIndex);
			});
		}
	}
	public void ClearAbilities () {
		foreach (Transform t in transform) {
			Destroy (t.gameObject);
		}
	}

	#region SpawningQueue
	public void ShowQueue (List<PlayerUnit> units,QueueingSystem queue) {
		//ClearQueue ();
		
		ClearAbilities ();

		//Loop through all Selected Units
		foreach (PlayerUnit unit in units) {
			
		//Instantiate ButtonPrefab and Assign Sprites
			GameObject _skillUI = Instantiate (skillUI, Vector3.zero, Quaternion.identity, transform);
			Image img = _skillUI.GetComponent<Image>();

			//check if there are sprites
			if(img == null){Debug.LogError("NO IMAGE!");continue;}
			if(unit.artwork == null) Debug.LogWarning("NO SPRITE");
				img.sprite = unit.artwork;		

		//Functions Delegates for buttons
			_skillUI.GetComponent<Button>().onClick.AddListener(delegate {
				int unitIndex = 0;
					unitIndex = units.IndexOf(unit);
				queue.AddToQueue(unitIndex);
			});
		}
	}
	public void ClearQueue () {
		foreach (Transform t in transform) {
			Destroy(t.gameObject);
		}
	}
	#endregion
	public void ShowMultiUnit (List<GameObject> units) {
		ClearQueue ();
		int displayCount = 0;
		Debug.Log(units.Count);
		foreach (GameObject unit in units) {
			Debug.Log("disp Count : " + displayCount);
			if(displayCount <= 31){
				Debug.Log("in IF");
				//Instantiate ButtonPrefab and Assign Sprites
				GameObject _skillUI = Instantiate (skillUI, Vector3.zero, Quaternion.identity, transform);
				Image img = _skillUI.GetComponent<Image>();

				//Check if there are Sprites
				if(img == null){Debug.LogError("NO IMAGE!");continue;}
				if(unit.GetComponent<MonoUnit>().artwork == null) Debug.LogWarning("NO SPRITE");
				img.sprite = unit.GetComponent<MonoUnit>().artwork;

				//Functions Delegates for selecting a Specific Unit
				_skillUI.GetComponent<Button>().onClick.AddListener(delegate {
					GameObject specificUnit = unit.gameObject; 
					PO.DeselectAll(new BaseEventData(EventSystem.current));
					specificUnit.GetComponent<UnitSelectable>().OnSelect(new BaseEventData (EventSystem.current));
					//PO.selectedUnits.Add(specificUnit);
					PO.UpdateUI();
				});

				if(units.Count > 31 && displayCount == 31){
					GameObject limit = _skillUI.transform.GetChild(0).gameObject;
					limit.SetActive(true);
					limit.transform.GetComponentInChildren<TextMeshProUGUI>().text = "+" + (units.Count - 31);
				}
				
				displayCount ++;
			}
			
		}
	}



}