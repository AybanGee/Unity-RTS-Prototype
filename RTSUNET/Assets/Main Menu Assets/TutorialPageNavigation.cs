using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialPageNavigation : MonoBehaviour {
	public List<GameObject> pages = new List<GameObject> ();
	public GameObject nextPageBtn, prevPageBtn;
	public int currPage;

	public void ResetPage(){
		currPage = -1;
		ChangePage(true);
	}

	public void ChangePage (bool NextButton) {
		if (NextButton) {
			currPage++;
		} else {
			currPage--;
		}

		for (int i = 0; i <= pages.Count - 1; i++) {
			if (i == currPage)
				pages[i].SetActive (true);
			else
				pages[i].SetActive (false);
		}

		if (currPage == pages.Count - 1)
			nextPageBtn.SetActive (false);
		else
			nextPageBtn.SetActive (true);

		if (currPage == 0)
			prevPageBtn.SetActive (false);
		else
			prevPageBtn.SetActive (true);

	}

}