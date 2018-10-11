using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Game Color Library", menuName = "Colors/Color Library")]
public class GameColorsScriptable : ScriptableObject {
public ColorDictionary gameColors;
public List<Color> gameColorList(){
	List<Color> colorList = new List<Color>();
	foreach (KeyValuePair<string, Color> entry in gameColors)
	{
		colorList.Add(entry.Value);
	}
	return colorList;
}

}

[System.Serializable]
public class ColorDictionary : SerializableDictionary<string, Color> {}