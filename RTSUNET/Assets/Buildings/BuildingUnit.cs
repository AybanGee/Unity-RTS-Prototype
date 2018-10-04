using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingUnit : MonoBehaviour {
	public Renderer[] teamColoredGfx;
	public int team;

	public int buildingPrice;
	public int sellingPrice;
	public float constructionTime;

	public BuildingType buildingType;

	Vector3 originalPos;
	Vector3 startingPos;
	private float startTime;
    private float journeyLength;

	public bool AscendOnStart = false;
	void Start(){
		if(AscendOnStart){
		startingPos = originalPos = this.transform.position;
		startingPos.y -= 14;
		transform.position = startingPos;
		startTime = Time.time;
		 journeyLength = Vector3.Distance(startingPos, originalPos);
		StartCoroutine(Ascend());
		}
	}
	[SerializeField]
	float ascendSpeed = 1;
	IEnumerator Ascend(){
		while(startingPos != originalPos){
			if(!AscendOnStart) yield return null;
		float distCovered = (Time.time - startTime) * ascendSpeed;
        float fracJourney = distCovered / journeyLength;
			transform.position = Vector3.Lerp(transform.position,originalPos,fracJourney);
				if(Vector3.Distance(transform.position, originalPos) < 0.5){
					transform.position = originalPos;
					break;
				}
				yield return null;
		}
		Debug.Log("Finished ascension");
		yield return null;
	}

}
