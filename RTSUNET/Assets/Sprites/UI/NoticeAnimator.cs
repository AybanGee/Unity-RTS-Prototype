using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NoticeAnimator : MonoBehaviour {
	public RectTransform transform;
	public Image img;
	public TextMeshProUGUI textHolder;
	
	
	// Use this for initialization
	void Start () {
		gameObject.transform.SetSiblingIndex(0);
		StartCoroutine(Fade());
	}


	IEnumerator Fade () {
		
		while (img.GetComponent<CanvasGroup>().alpha > 0) {
			img.GetComponent<CanvasGroup>().alpha -= Time.deltaTime / 1;
			transform.Translate (0f, 2, 0f);

			yield return null;
		}
		Destroy (this.gameObject);

	}
}