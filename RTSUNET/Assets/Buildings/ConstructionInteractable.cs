using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ConstructionInteractable : Interactable {
	public float constructionTime;
	public Unit assignedBuilder;
	// Use this for initialization
	public int buildingIndex;
	public int team;

	public PlayerObject playerObject;


	public override void Interact (Unit interactor) {
		base.Interact(interactor);
		if (assignedBuilder == null && interactor != null) {
			if (interactor.team == team)
				assignedBuilder = interactor;
			else{
				Debug.Log("NOPE not your team boy!");
				return;

			}
		}
			if(interactor == null){
			Debug.Log("OH NO");
			return;
			}
	
		constructionTime -= Time.deltaTime;
		if (constructionTime <= 0) {
			hasInteracted = true;
			Finished ();
		}
	}
	public override void OnDefocused () {
		base.OnDefocused ();
		assignedBuilder = null;

	}

	public void Finished () {
		Debug.Log ("Construction Complete");
		if(playerObject.hasAuthority){
		playerObject.CmdSpawnBuilding(buildingIndex,this.transform.position,this.transform.rotation);
		CmdDestroyMe();
		}else
		Debug.Log("No Authority to Create Building");
	}
	[Command]
	void CmdDestroyMe(){
		Destroy(this.gameObject);
	}

	
	new void Update () {

	if(isFocus && !hasInteracted){
	//	Debug.Log("Going to interact");
		float distance = Vector3.Distance(unit.position,interactionTransform.position);
		if(distance <= radius){
			Interact(unit.GetComponent<Unit>());
			//hasInteracted = true;
		}
	}


	}
}