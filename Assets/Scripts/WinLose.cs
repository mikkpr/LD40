using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinLose : MonoBehaviour {
	
	public bool isLose = true;

	void Start () {
		SoundManager.instance.StopMusic();
		if (isLose) {
			SoundManager.instance.PlayLoseMusic();
		} else {
			SoundManager.instance.PlayMainMenuTheme();
		}
	}

	public void OnRetryButtonClick () {
		SoundManager.instance.StopMainMenuTheme();
		SoundManager.instance.StopLoseMusic();
		SceneManager.LoadScene ("MainMenu");
	}
}
