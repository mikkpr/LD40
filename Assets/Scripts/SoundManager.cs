using UnityEngine;

public class SoundManager : MonoBehaviour {
    public AudioSource efxSource;
    public AudioSource musicSource;

    public AudioClip music;
    public static SoundManager instance;

    private float volumeScale = 100.0f;
    private float musicVolumeScale = 100.0f;

    void Awake () {
        //Check if there is already an instance of SoundManager
        if (instance == null)
            //if not, set it to this.
            instance = this;
        //If instance already exists:
        else if (instance != this)
            //Destroy this, this enforces our singleton pattern so there can only be one instance of SoundManager.
            Destroy (gameObject);

        //Set SoundManager to DontDestroyOnLoad so that it won't be destroyed when reloading our scene.
        DontDestroyOnLoad (gameObject);
    }

    public void PlaySound (AudioClip clip) {
        //Set the clip of our efxSource audio source to the clip passed in as a parameter.
        efxSource.clip = clip;

        //Play the clip.
        efxSource.Play ();
    }

    public void PlayMusic (string name) {
        AudioClip clip = null;
        if (name == "Music_1") {
            clip = music;
        }
        //Set the clip of our efxSource audio source to the clip passed in as a parameter.
        musicSource.clip = clip;

        //Play the clip.
        if (musicSource.clip) {
            musicSource.Play ();
        }
    }

    public void StopMusic () {
        musicSource.Stop ();

        musicSource.clip = null;
    }

    public void SetVolume (float scale) {
        volumeScale = scale;
        efxSource.volume = (volumeScale / 100);
        musicSource.volume = (volumeScale * musicVolumeScale / 100);
        Debug.Log ("Setting volume to" + efxSource.volume.ToString ());
    }

    public float GetVolume () {
        Debug.Log ("Getting volume: " + volumeScale.ToString ());
        return volumeScale;
    }

    public void SetMusicVolume (float scale) {
        musicVolumeScale = scale;
        musicSource.volume = (volumeScale * musicVolumeScale / 100 / 100);
        Debug.Log ("Setting music volume to" + musicSource.volume.ToString ());
    }

    public float GetMusicVolume () {
        Debug.Log ("Getting music volume: " + musicVolumeScale.ToString ());
        return volumeScale;
    }
}
