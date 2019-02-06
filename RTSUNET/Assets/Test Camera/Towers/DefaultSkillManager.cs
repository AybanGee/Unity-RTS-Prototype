using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultSkillManager : MonoBehaviour {
	public Attacker ability;
	// Use this for initialization
	void Start () {
		ability = gameObject.GetComponent<Attacker> ();
		StartCoroutine (GuardMode ());
	}

	IEnumerator GuardMode () {
		while (true) {
			Debug.Log ("Guard Mode :: loop");
			if (ability.defaultSkill ().GetIsActing ()) {
				Debug.Log ("Guard Mode :: isActing");
				if (ability.parentUnit.focus == null) {
					ability.defaultSkill ().SetIsActing (false);
				}

			} else {
				//find target
				Debug.Log ("Guard Mode :: Find New Target");

				Attack attackSkill = (Attack) ability.defaultSkill ();

				attackSkill.parentAbility.parentUnit.focus = null;
				attackSkill.parentAbility.parentUnit.RemoveFocus ();
				attackSkill.SearchForNewTarget ();

				Debug.Log ("Guard Mode :: Attack Skill : " + attackSkill);

			}

			yield return null;
		}

	}
}