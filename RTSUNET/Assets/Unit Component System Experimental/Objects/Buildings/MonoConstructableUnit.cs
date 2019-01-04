using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MonoConstructableUnit : MonoUnitFramework {

    protected Building assignedBuilding;
    protected int buildingIndex;

    public void InitializeConstructable (Building bldg,int spawnIndex, int _team, PlayerObject _PO) {
        Debug.Log ("Creating Constructable");
        Constructable constructable = gameObject.GetComponent<Constructable> ();
        assignedBuilding = bldg;
        name = bldg.name + " construction";
        constructable.constructionTime = bldg.creationTime;
        constructable.isTeamDependent = false;
        constructable.isInteractable = constructable.isOnlyFriendly = true;
        constructable.parentUnit = this;
        team = _team;
        PO = _PO;
        buildingIndex = spawnIndex;
        abilities = new List<MonoAbility> ();
        abilities.Add (constructable);

    }
    public void SpawnBuilding () {
       PO.BuildSys.SpawnBuilding(buildingIndex,this.transform.position,this.transform.rotation);
        StartCoroutine(SelfDestrtuct());
     }

    IEnumerator SelfDestrtuct(){
        for (int i = 0; i < 3; i++)
        {
        yield return null;
        }
        CmdSelfDestruct();
        yield return null;
        
    }
    [Command]
	void CmdSelfDestruct(){
		Destroy(this.gameObject);
	}
}