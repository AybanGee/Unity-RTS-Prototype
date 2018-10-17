using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Interaction Rules", menuName = "Interaction/Rule Set")]
public class RuleSet : ScriptableObject {
public List<InteractionRule> Rules = new List<InteractionRule>();
}
