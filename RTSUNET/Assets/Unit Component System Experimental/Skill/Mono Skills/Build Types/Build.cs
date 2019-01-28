using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Build : MonoSkill {

	public int buildRate = 1;
	public float buildSpeed = 1f;
	public bool isBuilding = false;
	Coroutine buildCoroutine;
	MonoUnitLibrary builder;
	new void Start () {
		builder = GetComponent<MonoUnitLibrary> ();
	}

	new void Update () {
		base.Update ();

	}

	public override void Stop () {
		base.Stop();
		StopBuild ();
	}

	#region Creation Action
	public virtual void DoBuild (Constructable[] targetConstructable) {
		Debug.Log ("goign to Build");
		if (targetConstructable == null) return;//checks to see if there is something to be built
		if (targetConstructable.Length == 0) return;//checks to see if there is something to be built

		isBuilding = true;
		buildCoroutine = StartCoroutine (BuildContinuous (targetConstructable));
	}
	IEnumerator BuildContinuous (Constructable[] targetConstructable) {
		while (isBuilding) {

		if (targetConstructable.Length <= 0) StopBuild ();

				for (int i = 0; i < targetConstructable.Length; i++) {
				if(targetConstructable[i] == null)continue;

				if(targetConstructable[i].constructionTimeLeft <= 0)
				StopBuild();
				builder.DoBuild (targetConstructable[i], buildRate);

			}

			yield return new WaitForSeconds (buildSpeed);
		}
		yield return null;
	}

	#endregion

	public override void ActOn (GameObject go) {
		Debug.Log ("using Building!");
		Constructable[] targetConstructable = {go.GetComponent<Constructable>()};
		DoBuild ( targetConstructable);
	}
	public void StopBuild () {
		Debug.Log ("Stopped Building");
		isBuilding = false;
		if (buildCoroutine != null)
			StopCoroutine (buildCoroutine);
	}

}