using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public abstract class MonoAbility : NetworkBehaviour {
    public bool isTeamDependent =true;
    public bool isInteractable = false;
    [SerializeField]
    public List<MonoSkill> skills = new List<MonoSkill> ();

    public AbilityEnum abilityType;

    public List<AbilityEnum> interactorAbilities = new List<AbilityEnum> ();

    [HideInInspector]
    public MonoUnitFramework parentUnit;
    public virtual bool isValidInteractor (MonoAbility interactor) {
        if(!isInteractable){
            Debug.Log("target not interactable!");
            return false;
        }
        if(isTeamDependent){
            if(interactor.parentUnit.team == parentUnit.team){
            Debug.Log("target is same team!");
            return false;
            }
        }
        if (interactorAbilities.Count > 0) {
            foreach (AbilityEnum ability in interactorAbilities) {
                if (ability == interactor.abilityType) return true;
                 Debug.Log("target not valid interactor " + interactor.abilityType + " to " + ability);
            }
        }
        else{
            Debug.Log(this + " has no interactorAbilities!");
        }
        
            Debug.Log("target not valid interactor!");
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

}