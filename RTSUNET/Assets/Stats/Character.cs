using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(CharStats))]
public class Character : Interactable {
		CharStats myStats;
		UnitCombat unitCombat;
		bool isAttacking = false;
		new void Start()
		{
		base.Start();
		// 	if(!isLocalPlayer)
		// {
		// 	this.enabled = false;
		// 	return;
		// }
		myStats = GetComponent<CharStats>();		
		}
	public override void Interact(){
		
		isAttacking = true;
		unitCombat = interactionTransform.GetComponent<UnitCombat>();
			base.Interact();
		
		}
	public override void StopInteract(){
		
			base.StopInteract();
			isAttacking = false;
		unitCombat = null;
	}

	new void Update() {
		base.Update();

		
		if(isInteracting)
		if(isAttacking){
//Debug.Log("aTTACK");
	

			unitCombat.Attack(myStats);
			}
			else{
				Debug.Log("No Unit Combat on Selected Unit");
			}
		}

}

