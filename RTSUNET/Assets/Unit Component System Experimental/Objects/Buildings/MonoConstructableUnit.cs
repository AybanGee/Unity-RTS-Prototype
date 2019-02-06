﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MonoConstructableUnit : MonoUnitFramework {

    public Building assignedBuilding;
    public int buildingIndex;


    public void InitializeConstructable (Building bldg, int spawnIndex, int _team, PlayerObject _PO) {
        Debug.Log ("Creating Constructable");
        Debug.Log("MonoConstructableUnit :: team : " + _team);

        Constructable constructable = gameObject.GetComponent<Constructable> ();
        assignedBuilding = bldg;
        name = bldg.name + " construction";
        constructable.constructionTime = bldg.creationTime;
        constructable.isTeamDependent = false;
        constructable.isInteractable = constructable.isOnlyFriendly = true;
        constructable.parentUnit = this;
        rangeInfluence = bldg.rangeInfluence;
        team = _team;
        PO = _PO;
        Debug.Log ("Building index" + spawnIndex);
        buildingIndex = spawnIndex;
        abilities = new List<MonoAbility> ();
        abilities.Add (constructable);

    }
    public void SpawnBuilding () {
        Debug.Log ("Spawn Building [index] : " + buildingIndex);
      //  buildingIndex = 
        PO.BuildSys.SpawnBuilding (buildingIndex, this.transform.position, this.transform.rotation);
        StartCoroutine (SelfDestrtuct ());
    }

    IEnumerator SelfDestrtuct () {
        for (int i = 0; i < 3; i++) {
            yield return null;
        }
        CmdSelfDestruct ();
        yield return null;

    }

    [Command]
    void CmdSelfDestruct () {
        Destroy (this.gameObject);
    }
}