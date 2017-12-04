using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinLose : MonoBehaviour {

	void Start () {
		SoundManager.instance.StopMusic();
		SoundManager.instance.PlayMainMenuTheme();
	}

	public void OnRetryButtonClick () {
		SoundManager.instance.StopMainMenuTheme();
		SceneManager.LoadScene ("MainMenu");
	}
}
