using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {
	public GameObject optionsPanel;
	public Slider volumeSlider;
	public Text volumeLabel;
	public Slider musicVolumeSlider;
	public Text musicVolumeLabel;

	private CanvasGroup optionsPanelCanvasGroup;

	// Use this for initialization
	void Awake () {
		optionsPanelCanvasGroup = optionsPanel.GetComponent<CanvasGroup> ();
		HideOptionsPanel ();
	}

	void Start () {
		volumeSlider.value = SoundManager.instance.GetVolume ();
		musicVolumeSlider.value = SoundManager.instance.GetMusicVolume ();
		volumeSlider.onValueChanged.AddListener (delegate { VolumeValueChange (); });
		musicVolumeSlider.onValueChanged.AddListener (delegate { MusicVolumeValueChange (); });
	}

	void ShowOptionsPanel () {
		volumeSlider.value = SoundManager.instance.GetVolume ();
		optionsPanelCanvasGroup.gameObject.SetActive (true);
	}

	void HideOptionsPanel () {
		optionsPanelCanvasGroup.gameObject.SetActive (false);
	}

	void VolumeValueChange () {
		volumeLabel.text = volumeSlider.value.ToString () + "%";
	}

	void MusicVolumeValueChange () {
		musicVolumeLabel.text = musicVolumeSlider.value.ToString () + "%";
	}

	public void onStartButtonPress () {
		Debug.Log ("Pressed Start button");
	}

	public void onOptionsButtonPress () {
		Debug.Log ("Pressed Options button");
		ShowOptionsPanel ();
	}

	public void onQuitButtonPress () {
		Debug.Log ("Pressed Quit button");
	}

	public void onOKButtonPress () {
		Debug.Log ("Pressed OK button");
		SoundManager.instance.SetVolume (volumeSlider.value);
		SoundManager.instance.SetMusicVolume (musicVolumeSlider.value);
		HideOptionsPanel ();
	}

	public void onCancelButtonPress () {
		Debug.Log ("Pressed Cancel button");
		HideOptionsPanel ();
	}
}
