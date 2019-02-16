using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DvdEffect : MonoBehaviour {
	Vector2 pos;
	Vector2 max;
	Vector2 size;
	Vector2 dir;
	public Image logo;
	RectTransform rt;
	public float maxIdleTime = 5;
	float idleTime = 0;

	Vector2 lastMousePos;

	public float speed = 5;
	public Color[] colorPalette;

	public Text txtData;
	public GameObject menu;
	public bool isPlay = false;
	Vector2 Origpos;
	Color origColor;
	//	bool active;
	bool hitX = false;
	bool hitY = false;

	// Use this for initialization
	void Start () {
		txtData.gameObject.SetActive (true);
		menu.gameObject.SetActive (true);
		//active = gameObject.activeSelf;
		rt = GetComponent<RectTransform> ();
		dir = new Vector2 (1, 1);
		size = new Vector2 (rt.rect.width, rt.rect.height);
		pos = new Vector2 (rt.anchoredPosition.x, rt.anchoredPosition.y);
		Origpos = pos;
		origColor = logo.color;
		lastMousePos = Vector2.zero;
	}

	void Reset () {
		rt.anchoredPosition = Origpos;
		logo.color = origColor;
	}

	void Stop () {
		txtData.gameObject.SetActive (false);
		menu.gameObject.SetActive (true);

		isPlay = false;
		Reset ();
	}
	void Play () {
		txtData.gameObject.SetActive (true);
		menu.gameObject.SetActive (false);

		isPlay = true;
	}

	// Update is called once per frame
	void Update () {

		if (lastMousePos == new Vector2 (Input.mousePosition.x, Input.mousePosition.y)) {
			if (!isPlay)
				idleTime += Time.deltaTime;
			if (idleTime >= maxIdleTime) {
				Play ();
				idleTime = 0;

			}
		} else {
			idleTime = 0;
			Stop ();
		}

		if (isPlay) {
			max = new Vector2 (Screen.width, Screen.height);

			hitX = false;
			hitY = false;

			float xnewPos = NewPosition (pos.x, speed, dir.x);
			float ynewPos = NewPosition (pos.y, speed, dir.y);
			if (xnewPos + (size.x * 4) > max.x || xnewPos <= 0) {
				hitX = true;
				dir.x = -dir.x;
				xnewPos = NewPosition (pos.x, speed, dir.x);
				ChangeColor ();
			}

			if (ynewPos + (size.y * -4) < -max.y || ynewPos >= 0) {
				hitY = true;
				dir.y = -dir.y;
				ynewPos = NewPosition (pos.y, speed, dir.y);
				ChangeColor ();
			}
			if (hitX && hitY) {
				CornerHit ();
			}

			pos.x = xnewPos;
			pos.y = ynewPos;
			rt.anchoredPosition = new Vector2 (xnewPos, ynewPos);
		}

		lastMousePos = Input.mousePosition;

	}
	float NewPosition (float position, float speed, float direction) {
		return position + speed * direction * Time.deltaTime;
	}

	void ChangeColor () {
		logo.color = colorPalette[Random.RandomRange (0, colorPalette.Length)];
	}
	int cornerHit = 0;
	void CornerHit () {
		cornerHit++;
		txtData.text = "cor" + cornerHit;
		StartCoroutine (superColor ());
	}

	IEnumerator superColor () {
		int times = 10;
		for (int i = 0; i < times; i++) {
			ChangeColor ();
			yield return new WaitForSeconds (.25f);
		}
	}
}