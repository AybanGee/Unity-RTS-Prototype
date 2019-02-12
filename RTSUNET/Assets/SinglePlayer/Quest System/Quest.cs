using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "New Quest", menuName = "Quests/Quest")]

public class Quest : ScriptableObject {
	public string questName;
	public string instructions;
	public int counter;
	public int count;
	public bool questDone = false;
}