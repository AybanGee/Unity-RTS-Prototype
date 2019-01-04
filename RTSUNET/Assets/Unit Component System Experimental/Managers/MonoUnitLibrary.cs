using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MonoUnitLibrary : NetworkBehaviour {
	//Unit Actions
	//Damage
	[Command] public void CmdDoDamage (NetworkIdentity targerStatsID, int damage) {
		Debug.Log ("Do Damage");
		targerStatsID.gameObject.GetComponent<Damageable> ().TakeDamage (damage);
	}
	//Build
	public void DoBuild (Constructable constructable, int amount) {
		constructable.Construct (amount);
		float percentComplete = (float)constructable.constructionTimeLeft/(float)constructable.constructionTime;
		percentComplete = 100f - (percentComplete*100); 
		Debug.Log ("Do Build:("+percentComplete+"%)");
	}

}