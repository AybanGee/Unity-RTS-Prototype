using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialPageNavigation : MonoBehaviour {
	public List<Tutorial_Data> pages = new List<Tutorial_Data> ();
	public GameObject nextPageBtn, prevPageBtn;
	public int currPage = 0;
	public Image displayImage;
	public TextMeshProUGUI titleHolder, instructionHolder;

	public void ChangePage (bool NextButton) {
		if (NextButton) {
			currPage++;
		} else {
			currPage--;
		}

		if (currPage > 0) {
			prevPageBtn.SetActive (true);
		} else {
			prevPageBtn.SetActive (false);
		}
		if (currPage < pages.Count - 1) {
			nextPageBtn.SetActive (true);
		} else {
			nextPageBtn.SetActive (false);
		}

		UpdateDisplay ();
	}

	public void ResetPage () {
		currPage = 0;

		if (currPage > 0) {
			prevPageBtn.SetActive (true);
		} else {
			prevPageBtn.SetActive (false);
		}
		if (currPage < pages.Count - 1) {
			nextPageBtn.SetActive (true);
		} else {
			nextPageBtn.SetActive (false);
		}

		UpdateDisplay ();
	}

	public void UpdateDisplay () {
		displayImage.sprite = pages[currPage].artwork;
		titleHolder.text = pages[currPage].title;
		instructionHolder.text = pages[currPage].description;
	}

}