using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
[RequireComponent (typeof (UnitInteractable))]
public class CharStats : UnitStats {
	public PlayerObject netPlayer;
	public UnitInteractable unitInteractable;
	public void Start () {
		unitInteractable = GetComponent<UnitInteractable>();
	}
	public override void Die () {

		base.Die ();
		// bool found = netPlayer.myUnits.Remove(this.gameObject);
		// Debug.Log("Success remove "+ found);	
		
		// foreach(UnitInteractable unit in unitInteractable.interactors){
		// 	unit.GetComponent<Unit>().RemoveFocus();
		// }

		CmdDie ();
	}

	[Command]
	void CmdDie () {
		RpcDie ();
		Death ();

	}

	[ClientRpc]
	void RpcDie () {

		netPlayer.myUnits.Remove (this.gameObject);
	}

	void Death () {
		if (isServer == false) {
			Debug.Log ("Client called die");
			return;
		}
		netPlayer.myUnits.Remove (this.gameObject);
		Destroy (gameObject);
	}

}