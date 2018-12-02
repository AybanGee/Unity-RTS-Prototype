using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public abstract class Ability : ScriptableObject {

    public AbilityEnum abilityType;
    public List<AbilityEnum> interactorAbilities;
    public bool isTeamDependent = true;
    public bool isInteractable = false;

    public virtual void Initialize (GameObject go,int abilityID) {
        Debug.Log ("Initializing Ability " + abilityID);
        NetworkInit (go,abilityID); 
        RpcInitialize (go.GetComponent<NetworkIdentity> (),abilityID);
    }

    private void NetworkInit (GameObject go,int abilityID) {
        MonoUnitFramework muf = go.GetComponent<MonoUnitFramework> ();
        MonoAbility ma = muf.abilities[abilityID];
        ma.isTeamDependent = isTeamDependent;
        ma.parentUnit = muf;
        ma.isInteractable = isInteractable;
    }

    // [Command] public void CmdInitialize (NetworkIdentity ni,int abilityID) { 
    //     NetworkInit (ni.gameObject,abilityID); 
    //     RpcInitialize (ni,abilityID);
    //      }

    [ClientRpc] public void RpcInitialize (NetworkIdentity ni,int abilityID) { 
        NetworkInit (ni.gameObject,abilityID); 
        }

}