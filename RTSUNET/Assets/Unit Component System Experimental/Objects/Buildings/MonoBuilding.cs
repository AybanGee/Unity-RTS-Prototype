using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonoBuilding : MonoUnitFramework {
	public int buildingPrice;
	public int sellingPrice;
	public float constructionTime;
	[HideInInspector] public BuildingType buildingType;
	[HideInInspector] public MonoUnitFramework focus;

	void Start () {
		//Nasa building System yung assign ng abilities

		if (AscendOnStart) {
			startingPos = originalPos = this.transform.position;
			startingPos.y -= 14;
			transform.position = startingPos;
			startTime = Time.time;
			journeyLength = Vector3.Distance (startingPos, originalPos);
			StartCoroutine (Ascend ());
		}

		NavMeshObstacle navmesh = GetComponent<NavMeshObstacle> ();
		BoxCollider boxCollider = GetComponent<BoxCollider> ();

	}
	#region Ascension

	Vector3 originalPos;
	Vector3 startingPos;
	private float startTime;
	private float journeyLength;

	public bool AscendOnStart = true;

	float ascendSpeed = 1;
	IEnumerator Ascend () {
		while (startingPos != originalPos) {
			if (!AscendOnStart) yield return null;
			float distCovered = (Time.time - startTime) * ascendSpeed;
			float fracJourney = distCovered / journeyLength;
			transform.position = Vector3.Lerp (transform.position, originalPos, fracJourney);
			if (Vector3.Distance (transform.position, originalPos) < 0.5) {
				transform.position = originalPos;
				break;
			}
			yield return null;
		}
		Debug.Log ("Finished ascension");
		yield return null;
	}

	#endregion	
	public override void SetFocus (MonoUnitFramework newFocus, MonoSkill skill) {
		base.SetFocus (newFocus, skill);
		Debug.Log("REACHED POINT OF ACTIVISION");
		Debug.Log("What's inside the new focus? It is none other than" + newFocus);
		Debug.Log("What's inside the focus? It is none other than" + focus);
		//VALIDATE FOCUS HERE
		if (newFocus != focus) {
		Debug.Log("ACTIVISION");

			focus = newFocus;
			//motor.FollowTarget (newFocus, skill);
			skill.Activate (newFocus.gameObject);
		}

	}

	public override void RemoveFocus () {
		base.RemoveFocus ();
		focus = null;
		//motor.StopFollowingTarget ();
	}
}