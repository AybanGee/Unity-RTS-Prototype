using UnityEngine;
using UnityEngine.Networking;

public class Interactable : NetworkBehaviour {
public float radius = 5f;

public bool isFocus = false;

public Transform interactionTransform;
public Transform unit;
public bool hasInteracted = false;
public bool isInteracting = false;
public void Start()
{
	if(interactionTransform == null)
	interactionTransform = this.transform;
}
public virtual void Interact(){
	isInteracting = true;
	//meant to be overriden
//Debug.Log("Interacting with " + this.name);
}

public virtual void Interact(Unit interactor){
	isInteracting = true;
	//meant to be overriden
//Debug.Log("Interacting with " + this.name);
}
public virtual void StopInteract(){
	hasInteracted =false;
}

public void Update(){
	//Debug.Log("lol");
	if(isFocus && !hasInteracted){
		Debug.Log("Going to interact");
		float distance = Vector3.Distance(unit.position,interactionTransform.position);
		if(distance <= radius){
			Interact();
			hasInteracted = true;
		}
	}
}



public void OnFocused(Transform unitTransform){
	
				Debug.Log("OnFocused" + hasInteracted);
	isFocus = true;
	unit = unitTransform;
	hasInteracted =false;
}

public virtual void OnDefocused(){
	 
	 if(hasInteracted)
	 StopInteract();
	isFocus = false;
	unit = null;
	isInteracting =false;
}
	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(interactionTransform.position,radius);
	}
}
