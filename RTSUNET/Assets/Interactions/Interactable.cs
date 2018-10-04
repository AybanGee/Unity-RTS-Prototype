using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public abstract class Interactable : NetworkBehaviour {
	public float rangeRadius = 5f;
	public float influenceRadius = 5f;
	public bool isFocus = false;

	public Transform interactionTransform;
	public List<Interactable> interactors;
	public bool hasInteracted = false;
	public bool isInteracting = false;
	public void Start () {
		if (interactionTransform == null)
			interactionTransform = this.transform;
	}

	public virtual void Interact (Interactable interactor) {
		Debug.Log ("interacting  = true");

		interactor.isInteracting = true;

		//meant to be overriden
		//Debug.Log("Interacting with " + this.name);
	}
	public virtual void StopInteract (Interactable interactor) {

		interactor.hasInteracted = false;
	
	}

	public void Update () {
		//Debug.Log("lol");
		if(interactors.Count > 0)
		foreach (Interactable interactor in interactors) {
//			Debug.Log(interactor.name + " is now interacting");
			if (isFocus && !interactor.hasInteracted) {
				float distance = Vector3.Distance (interactor.transform.position, interactionTransform.position);

			//	Debug.Log ("Going to interact distance = " + distance);
				if (distance - influenceRadius <= interactor.rangeRadius) {
					Debug.Log ("interacting");
					Interact (interactor);
					interactor.hasInteracted = true;
				}
			}
		}

	}

	public void OnFocused (Interactable interactor) {
		Debug.Log("Trying On Focused");
		if (interactor.isInteracting == true){Debug.Log("interactor.isInteracting"); return;}
		if (interactors.Contains (interactor)){Debug.Log("interactors.Contains"); return;}
		Debug.Log ("OnFocused");
		isFocus = true;
		//unit = unitTransform;
		interactor.OnInteractorFocused(this);
		interactors.Add (interactor);
		hasInteracted = false;
	}
	public virtual void OnInteractorFocused(Interactable interactable){
	//	Debug.Log(this.name + " started focusing on " + interactable.name);
	}
	public virtual void OnDefocused (Interactable interactor) {

		Debug.Log ("OnDefocused");

		if (interactor.hasInteracted)
			StopInteract (interactor);
	

		interactor.isInteracting = false;
		interactor.isFocus = false;
		interactor.OnInteractorDefocused(this);
		interactors.Remove (interactor);
	}

	public virtual void OnInteractorDefocused(Interactable interactable){
		//Debug.Log(this.name + " stopped focusing on " + interactable.name);
	}
	void OnDrawGizmosSelected () {
		if (interactionTransform == null)
			interactionTransform = this.transform;
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere (interactionTransform.position, rangeRadius);

		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere (interactionTransform.position, influenceRadius);
	}
	
	public virtual bool isValidInteractor(Interactable interactor){
	return true;
	}
}