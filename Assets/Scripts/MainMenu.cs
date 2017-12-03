using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {
    public GameObject optionsPanel;
    public Slider volumeSlider;
    public Text volumeLabel;
    public Slider musicVolumeSlider;
    public Button startButton;
    public Text musicVolumeLabel;

    public Image BGImage;

    CanvasGroup optionsPanelCanvasGroup;

    // Use this for initialization
    void Awake () {
        optionsPanelCanvasGroup = optionsPanel.GetComponent<CanvasGroup> ();
        HideOptionsPanel ();
    }

    void Start () {
        volumeSlider.value = SoundManager.instance.GetVolume ();
        musicVolumeSlider.value = SoundManager.instance.GetMusicVolume ();

        volumeSlider.onValueChanged.AddListener (VolumeValueChange);
        musicVolumeSlider.onValueChanged.AddListener (MusicVolumeValueChange);

        // SoundManager.instance.PlayMainMenuTheme ();
    }

    void Update () {
        UpdateBGAlpha ();
    }

    void UpdateBGAlpha () {
        float alpha = GetBGAlpha ();
        Color c = BGImage.color;
        c.a = Mathf.Clamp(alpha, 0f, 1f);
        BGImage.color = c;
        Debug.Log(c);
    }

    float GetBGAlpha () {
        float screenW = Screen.width;
        float screenH = Screen.height;
        Vector2 startPos = startButton.GetComponent<RectTransform> ().anchoredPosition;
        Vector2 startPosInScreenCoords = new Vector2 (startPos.x + screenW / 2f, startPos.y + screenH / 2f);
        Vector2 mouse2d = new Vector2 (Input.mousePosition.x, Input.mousePosition.y);
        float maxDiff = startPosInScreenCoords.magnitude;
        float diff = Vector2.Distance (startPosInScreenCoords, mouse2d);
        return 1 - diff / maxDiff;
    }

    void ShowOptionsPanel () {
        volumeSlider.value = SoundManager.instance.GetVolume ();
        optionsPanelCanvasGroup.gameObject.SetActive (true);
    }

    void HideOptionsPanel () {
        optionsPanelCanvasGroup.gameObject.SetActive (false);
    }

    void VolumeValueChange (float volume) {
        volumeLabel.text = volumeSlider.value.ToString () + "%";
    }

    void MusicVolumeValueChange (float volume) {
        musicVolumeLabel.text = musicVolumeSlider.value.ToString () + "%";
    }

    public void OnStartButtonPress () {
        SoundManager.instance.StopMainMenuTheme ();
        SceneManager.LoadScene ("Fight");
    }

    public void OnOptionsButtonPress () {
        ShowOptionsPanel ();
    }

    public void OnQuitButtonPress () {
        Application.Quit ();
    }

    public void OnOKButtonPress () {
        SoundManager.instance.SetVolume (volumeSlider.value);
        SoundManager.instance.SetMusicVolume (musicVolumeSlider.value);
        HideOptionsPanel ();
    }

    public void OnCancelButtonPress () {
        HideOptionsPanel ();
    }
}
