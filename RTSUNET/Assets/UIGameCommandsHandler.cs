using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGameCommandsHandler : MonoBehaviour {

	public GameObject abilityPanel, skillUI;
	PlayerObject PO;
	public void Initialize(PlayerObject playerObject){
		PO = playerObject;
	}

	public void ShowAbilities (List<MonoAbility> abilities) {
		ClearAbilities ();
		foreach (MonoAbility ability in abilities) {
			if (ability.skills.Count > 0) {
				Transform abilityTransform = Instantiate (abilityPanel, Vector3.zero, Quaternion.identity, transform).transform;
				foreach (MonoSkill skill in ability.skills) {
				GameObject _skillUI = Instantiate (skillUI, Vector3.zero, Quaternion.identity, abilityTransform);
				Image img = _skillUI.GetComponent<Image>();
				if(img == null){Debug.LogError("NO IMAGE!");continue;}
				img.sprite = skill.sSprite;		
				}
			}
		}
	}
	public void ClearAbilities () {
		foreach (Transform t in transform) {
			Destroy (t.gameObject);
		}
	}
}