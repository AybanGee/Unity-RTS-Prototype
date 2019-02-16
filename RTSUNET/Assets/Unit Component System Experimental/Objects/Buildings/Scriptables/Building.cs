using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Building", menuName = "Buildings/Building")]
public class Building : UnitFramework {
	public BuildingType type;
	public int sellPrice=50;
	public Vector3 addedColliderScale;
	public Vector3 rubbleSize,rubbleObstacleSize;

	    public override void Initialize (GameObject go) {
        base.Initialize (go);
    }
}
