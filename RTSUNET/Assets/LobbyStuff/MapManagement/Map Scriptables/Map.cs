using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Map", menuName = "Maps/New Map")]
public class Map : ScriptableObject
{
    public string mapName;
	public string sceneName;
	public string description;
	public Sprite displayImage;	
}