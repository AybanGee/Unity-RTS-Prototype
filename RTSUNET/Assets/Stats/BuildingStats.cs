using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BuildingStats : UnitStats {

		public PlayerObject netPlayer;

	public override void Die () {

		base.Die ();
		// bool found = netPlayer.myUnits.Remove(this.gameObject);
		// Debug.Log("Success remove "+ found);

		CmdDie ();
	}

	[Command]
	void CmdDie () {
		RpcDie ();
		Death ();

	}

	[ClientRpc]
	void RpcDie () {

		netPlayer.myBuildings.Remove (this.gameObject);
	}

	void Death () {
		if (isServer == false) {
			Debug.Log ("Client called die");
			return;
		}
		netPlayer.myBuildings.Remove (this.gameObject);
		Destroy (gameObject);
	}

}
