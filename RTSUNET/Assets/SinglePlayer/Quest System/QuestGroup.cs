using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Quest Group", menuName = "Quests/Quest Group")]
public class QuestGroup : ScriptableObject {
public List<Quest> quests = new List<Quest>();
}
