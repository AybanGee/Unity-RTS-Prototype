using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour {

	public AudioMixer audioMixer;
	public TMPro.TMP_Dropdown resolutionsDropdown;
	public Slider volumeSlider;
	public Toggle fullscreenToggle;

	Resolution[] resolutions;
	int currentResolutionIndex = 0;
	

	void Start () {
		resolutions = Screen.resolutions;

		resolutionsDropdown.ClearOptions ();
		List<string> options = new List<string> ();

		for (int i = 0; i < resolutions.Length; i++) {
			string option = resolutions[i].width + " x " + resolutions[i].height;
			options.Add (option);
			
			if (resolutions[i].width == Screen.width &&
			 resolutions[i].height == Screen.height) 
			{
				currentResolutionIndex = i;
			}

		}

		resolutionsDropdown.AddOptions (options);
		resolutionsDropdown.value = currentResolutionIndex;
		resolutionsDropdown.RefreshShownValue ();

		SetUIToCurrent ();

	}

	public void SetVolume (float volume) {
		//Debug.Log(volume);
		audioMixer.SetFloat ("volume", volume);
	}

	public void SetQuality (int qualityIndex) {
		QualitySettings.SetQualityLevel (qualityIndex);
	}

	public void SetFullscreen (bool isFullscreen) {
		Screen.fullScreen = isFullscreen;
	}

	public void SetResolution (int resolutionIndex) {
		Resolution resolution = resolutions[resolutionIndex];
		Screen.SetResolution (resolution.width, resolution.height, Screen.fullScreen);
		currentResolutionIndex = resolutionIndex;
	}

	public void SetUIToCurrent () {
		float vHolder;
		audioMixer.GetFloat ("volume", out vHolder);

		volumeSlider.value = vHolder;
		resolutionsDropdown.value = currentResolutionIndex;
		resolutionsDropdown.RefreshShownValue ();

		fullscreenToggle.isOn = Screen.fullScreen;
	}

}