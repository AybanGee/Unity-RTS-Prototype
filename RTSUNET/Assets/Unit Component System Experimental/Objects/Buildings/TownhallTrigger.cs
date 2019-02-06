using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownhallTrigger : MonoBehaviour {
	public PlayerObject PO;

	public void SetIsDefeated(){
		PO.ShowNotice("Your Townhall was Destroyed.");
		PO.SetDefeatStatus(true);
	}

}
