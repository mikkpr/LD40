using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject optionsPanel;
    public Slider volumeSlider;
    public Text volumeLabel;
    public Slider musicVolumeSlider;
    public Text musicVolumeLabel;

    CanvasGroup optionsPanelCanvasGroup;

    // Use this for initialization
    void Awake()
    {
        optionsPanelCanvasGroup = optionsPanel.GetComponent<CanvasGroup>();
        HideOptionsPanel();
    }

    void Start()
    {
        volumeSlider.value = SoundManager.instance.GetVolume();
        musicVolumeSlider.value = SoundManager.instance.GetMusicVolume();

        volumeSlider.onValueChanged.AddListener(VolumeValueChange);
        musicVolumeSlider.onValueChanged.AddListener(MusicVolumeValueChange);
		
		SoundManager.instance.PlayMainMenuTheme();
    }

    void ShowOptionsPanel()
    {
        volumeSlider.value = SoundManager.instance.GetVolume();
        optionsPanelCanvasGroup.gameObject.SetActive(true);
    }

    void HideOptionsPanel()
    {
        optionsPanelCanvasGroup.gameObject.SetActive(false);
    }

    void VolumeValueChange(float volume)
    {
        volumeLabel.text = volumeSlider.value.ToString() + "%";
    }

    void MusicVolumeValueChange(float volume)
    {
        musicVolumeLabel.text = musicVolumeSlider.value.ToString() + "%";
    }

    public void OnStartButtonPress()
    {
        SoundManager.instance.StopMainMenuTheme();
        SceneManager.LoadScene("Fight");
    }

    public void OnOptionsButtonPress()
    {
        ShowOptionsPanel();
    }

    public void OnQuitButtonPress()
    {
        Application.Quit();
    }

    public void OnOKButtonPress()
    {
        SoundManager.instance.SetVolume(volumeSlider.value);
        SoundManager.instance.SetMusicVolume(musicVolumeSlider.value);
        HideOptionsPanel();
    }

    public void OnCancelButtonPress()
    {
        HideOptionsPanel();
    }
}
