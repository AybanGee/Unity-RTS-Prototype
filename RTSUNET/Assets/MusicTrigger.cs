using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicTrigger : MonoBehaviour {

	AudioManager audioManager;
	public AudioClip audio;
	void Start(){
		audioManager = AudioManager.instance;
		
		audioManager.PlayMusic(audio);
	}
}
