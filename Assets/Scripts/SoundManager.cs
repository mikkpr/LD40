using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour {
    public AudioSource efxSource;

    public AudioClip hitSound;
    public AudioClip stumbleSound;
    public AudioClip clickSound;

    public List<AudioSource> musicSources = new List<AudioSource> ();

    public List<AudioClip> musicLayers = new List<AudioClip> ();

    public AudioSource mainMenuSource;
    public AudioClip mainMenuClip;
    public static SoundManager instance;

    public int activeLayers = 1;
    private int maxLayers = 5;

    private float volumeScale = 100.0f;
    private float musicVolumeScale = 100.0f;

    void Awake () {
        if (instance != null && instance != this) {
            Destroy (this.gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad (this.gameObject);
    }

    public void PlaySound (string name) {
        efxSource.clip = null;
        //Set the clip of our efxSource audio source to the clip passed in as a parameter.
        if (name == "hit") {
            efxSource.clip = hitSound;
        } else if (name == "stumble") {
            efxSource.clip = stumbleSound;
        } else if (name == "click") {
            efxSource.clip = clickSound;
        }

        //Play the clip.
        efxSource.Play ();
    }

    public void PlayMainMenuTheme () {
        mainMenuSource.clip = mainMenuClip;
        mainMenuSource.Play ();
    }

    public void StopMainMenuTheme () {
        mainMenuSource.clip = null;
        mainMenuSource.Stop ();
    }

    public void PlayMusic () {
        for (int i = 0; i < musicSources.Count; i++) {
            AudioSource source = musicSources[i];
            source.clip = musicLayers[i];
            source.Play ();
            if (i > 0) {
                source.volume = 0;
            }
        }
    }

    public void StopMusic () {
        for (int i = 0; i < musicSources.Count; i++) {
            AudioSource source = musicSources[i];
            source.clip = null;
            source.Stop ();
        }
    }

    public bool IsMusicPlaying () {
        AudioSource source = musicSources[0];
        return source.isPlaying;
    }

    public void SetVolume (float scale) {
        volumeScale = scale;
        float actualMusicScale = (volumeScale * musicVolumeScale / 100 / 100);
        efxSource.volume = (volumeScale / 100);
        for (int i = 0; i < musicSources.Count; i++) {
            musicSources[i].volume = actualMusicScale;
            if ((i + 1) > activeLayers) {
                musicSources[i].volume = 0;
            }
            mainMenuSource.volume = actualMusicScale;
        }
    }

    public float GetVolume () {
        return volumeScale;
    }

    public float GetCurrentTime () {
        return musicSources[0].time;
    }

    public void SetMusicVolume (float scale) {
        musicVolumeScale = scale;
        float actualMusicScale = (volumeScale * musicVolumeScale / 100 / 100);
        for (int i = 0; i < musicSources.Count; i++) {
            musicSources[i].volume = actualMusicScale;
        }
    }

    public float GetMusicVolume () {
        return musicVolumeScale;
    }

    public void addMusicLayer () {
        if (activeLayers < maxLayers) {
            activeLayers++;
        }
    }

    public void removeMusicLayer () {
        if (activeLayers > 1) {
            activeLayers--;
        }
    }

    private void updateMusicLayers () {
        for (int i = 0; i < musicSources.Count; i++) {
            AudioSource source = musicSources[i];
            if (source.volume == 0 && (i + 1) <= activeLayers) {
                source.volume = (volumeScale * musicVolumeScale / 100 / 100);
            } else if (source.volume > 0 && (i + 1) > activeLayers) {
                source.volume = 0;
            }
        }
    }

    public void Update () {
        updateMusicLayers ();
    }
}
