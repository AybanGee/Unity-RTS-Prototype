using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MonoUnitLibrary : NetworkBehaviour {
	//Unit Actions
	//Damage
	[Command] public void CmdDoDamage (NetworkIdentity targerStatsID, int damage) {

		Debug.Log ("CMD Do Damage :: target : " + targerStatsID);

		if (targerStatsID.gameObject != null)
			RpcDoDamage (targerStatsID, damage);

	}

	[ClientRpc]
	public void RpcDoDamage (NetworkIdentity targerStatsID, int damage) {
		Debug.Log ("MonoUnitLibrary :: RpcDoDamage : Server :" + isServer);

		if(targerStatsID == null) return;
		if(targerStatsID.gameObject == null) return;

		if (targerStatsID.gameObject != null)
			targerStatsID.gameObject.GetComponent<Damageable> ().TakeDamage (damage, GetComponent<NetworkIdentity> ());

	}

	[Command]
	public void CmdDeath () {
		//Destroy (healthUI.ui.gameObject);

		StartCoroutine (SelfDestruct ());
		//RpcDeath ();
		//Destroy (this.gameObject);

	}

	[ClientRpc]
	public void RpcDeath () {
		//when used unit stays in client afterdeath
		//	GetComponent<MonoUnitFramework>().PO.RemoveUnit(this.gameObject);

		//if (this.gameObject != null) {
		GetComponent<MonoUnitFramework> ().StopAbilities ();
		NetworkServer.Destroy (this.gameObject);
		//}
		//if (isServer) return;

	}

	/* 	[ClientRpc]public void RpcDoDamage (NetworkIdentity targerStatsID, int damage) {
		
			targerStatsID.gameObject.GetComponent<Damageable> ().TakeDamage (damage,GetComponent<NetworkIdentity>());
		} */
	//Build
	public void DoBuild (Constructable constructable, int amount) {
		constructable.Construct (amount);
		float percentComplete = (float) constructable.constructionTimeLeft / (float) constructable.constructionTime;
		percentComplete = 100f - (percentComplete * 100);
		Debug.Log ("Do Build:(" + percentComplete + "%)");
	}

	IEnumerator SelfDestruct () {
		/* NetworkServer.UnSpawn (this.gameObject); */

		yield return new WaitForSeconds (.25f);
		RpcDeath ();

		yield return null;
	}

}