using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToolTip : MonoBehaviour {
	public Image displayPic;
	public TextMeshProUGUI description, name, cost, duration;

	void Start () {
		transform.gameObject.SetActive (false);
	}

	public void ShowToolTip (ToolTipData ttd) {
		InitToolTip ();
		displayPic.sprite = ttd.displayPic;
		description.text = ttd.description;
		name.text = ttd.name;
		if (!string.IsNullOrEmpty (ttd.cost))
			cost.text = "Cost : " + ttd.cost;
		if (!string.IsNullOrEmpty (ttd.duration))
			duration.text = "Build Time: " + ttd.duration;
	}
	public void InitToolTip () {
		displayPic.sprite = null;
		description.text = name.text = cost.text = duration.text = string.Empty; // TODO test me madafaka
	}
	public void HideTooltip () {
		transform.gameObject.SetActive (true);
	}
}