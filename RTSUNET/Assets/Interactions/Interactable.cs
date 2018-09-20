using UnityEngine;

public class Interactable : MonoBehaviour {
public float radius = 5f;

bool isFocus = false;

public Transform interactionTransform;
Transform unit;
bool hasInteracted = false;

void Start()
{
	if(interactionTransform == null)
	interactionTransform = this.transform;
}
public virtual void Interact(){
	//meant to be overriden
	Debug.Log("Interacting with " + this.name);
}

void Update(){
	if(isFocus && !hasInteracted){
		float distance = Vector3.Distance(unit.position,interactionTransform.position);
		if(distance <= radius){
			Interact();
			hasInteracted = true;
		}
	}
}
public void OnFocused(Transform unitTransform){
	isFocus = true;
	unit = unitTransform;
	hasInteracted =false;
}

public void OnDefocused(){
	isFocus = false;
	unit = null;
	hasInteracted =false;
}
	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(interactionTransform.position,radius);
	}
}
