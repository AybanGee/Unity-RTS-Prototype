using UnityEngine;

[CreateAssetMenu (fileName = "Tutorial_Data", menuName = "Tutorial/Tutorial_Data")]
public class Tutorial_Data : ScriptableObject {
	public string title;
	[TextArea (3, 9)]
	public string description;
	public Sprite artwork;
}
