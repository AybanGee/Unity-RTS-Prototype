using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public abstract class MonoAbility : NetworkBehaviour {
    public bool isTeamDependent = true;
    public bool isOnlyFriendly = false;
    public bool isInteractable = false;
    [SerializeField]
    public List<MonoSkill> skills = new List<MonoSkill> ();

    public AbilityEnum abilityType;

    public List<AbilityEnum> interactorAbilities = new List<AbilityEnum> ();

    [HideInInspector]
    public MonoUnitFramework parentUnit;

    public int defaultSkillIndex = 0;

    public virtual bool isValidInteractor (MonoAbility interactor) {

        Debug.Log ("Checking if Valid Interactor :");
        Debug.Log ("is Interactable :" + isInteractable);
        Debug.Log ("is TeamDependent :" + isTeamDependent);

        if (!isInteractable) {
            Debug.Log ("target not interactable!" + this);
            return false;
        }

        if (isTeamDependent) {
            //if(interactor.parentUnit.team == null){ Debug.Log("No Interactor Parent Unit team");  return;}
            //if(parentUnit == null) {Debug.Log("No intercourse"); return false;}
            if (interactor.parentUnit.team == parentUnit.team) {
                Debug.Log ("target is same team!");
                return false;
            }
        }

        if (isOnlyFriendly) {
            if (interactor.parentUnit.team != parentUnit.team) {
                Debug.Log ("target is not same team!");
                return false;
            }
        }
        if (interactorAbilities.Count > 0) {
            foreach (AbilityEnum ability in interactorAbilities) {
                if (ability == interactor.abilityType) return true;
                Debug.Log ("target not valid interactor " + interactor.abilityType + " to " + ability);
            }
        } else {
            Debug.Log (this + " has no interactorAbilities!");
        }

        Debug.Log ("target not valid interactor!");
        return false;
    }

    void Start () {
        if (abilityType == null) Debug.LogError ("Ability type not found!");
    }

    public void StopSkills () {
        if (skills.Count <= 0) return;
        for (int i = 0; i < skills.Count; i++) {
            skills[i].Stop ();
        }
    }

    public virtual void SetDefaultSkill (int index) {
        if (index >= skills.Count && index < 0) 
        { Debug.LogError ("Default skill of index " + index + " is out of range"); return; }
        defaultSkillIndex = index;
    }

    public MonoSkill defaultSkill(){
        if(skills.Count == 0)
        return null;
        return skills[defaultSkillIndex];
    }

}