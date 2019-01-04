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

	Coroutine construction;

	public override void Interact (Interactable interactable) {
		base.Interact(interactable);
		if (assignedBuilder == null && interactable != null) {
			if (interactable.GetComponent<Unit>().team == team)
				assignedBuilder = GetComponent<Unit>();
			else{
				Debug.Log("NOPE not your team boy!");
				return;

			}
		}
			if(interactable == null){
			Debug.Log("OH NO");
			return;
			}
		if(construction == null)
		construction = StartCoroutine(constructBuilding());
	}
	IEnumerator constructBuilding(){
		while(true){
			Debug.Log("Coroutine Cycle");
			constructionTime -= 1 * interactors.Count;
		if (constructionTime <= 0) {
			hasInteracted = true;
			Finished ();
		}
		yield return new WaitForSeconds(1f);
		}
		
	}
	// public override void OnDefocused (Interactable interactor) {
	// 	base.OnDefocused (interactor);
	// 	Debug.Log("CONSTRUCTOR");
	// 	if(interactors.Count == 0 && construction != null){
			
	// 	StopCoroutine(construction);
	// 	}
	// 	assignedBuilder = null;
	// 	construction = null;

	// }
	public void Finished () {
		if(construction!=null)
		StopCoroutine(construction);
		construction = null;
		Debug.Log ("Construction Complete");
		//defocus all constructors
		List<Interactable> constructors = interactors;
		for (int i = 0; i < constructors.Count; i++)
		{
			Unit thisuUnit = constructors[i].GetComponent<Unit>();
			if(thisuUnit !=null)
			thisuUnit.RemoveFocus();
		}
		if(playerObject.hasAuthority){
		playerObject.BuildSys.SpawnBuilding(buildingIndex,this.transform.position,this.transform.rotation);
		CmdDestroyMe();
		}else
		Debug.Log("No Authority to Create Building");
	}
	[Command]
	void CmdDestroyMe(){
		Destroy(this.gameObject);
	}
	public override bool isValidInteractor(Interactable interactor){
		if(interactor == null)return false;
		//check if team mate
		Unit unitInteractor = interactor.GetComponent<Unit>();
		if(unitInteractor == null) return false;
		if(unitInteractor.team != team)return false;
		if(unitInteractor.unitType != UnitType.Builder)return false;



		return true;
	}
}