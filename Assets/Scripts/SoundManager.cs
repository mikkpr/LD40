
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource efxSource;
    public AudioSource musicSource;

    public AudioClip music;
    public static SoundManager instance;

    void Awake()
    {
        //Check if there is already an instance of SoundManager
        if (instance == null)
            //if not, set it to this.
            instance = this;
        //If instance already exists:
        else if (instance != this)
            //Destroy this, this enforces our singleton pattern so there can only be one instance of SoundManager.
            Destroy(gameObject);

        //Set SoundManager to DontDestroyOnLoad so that it won't be destroyed when reloading our scene.
        DontDestroyOnLoad(gameObject);
    }

    public void PlaySound(AudioClip clip)
    {
        //Set the clip of our efxSource audio source to the clip passed in as a parameter.
        efxSource.clip = clip;

        //Play the clip.
        efxSource.Play();
    }

    public void PlayMusic(string name)
    {
        AudioClip clip = null;
        if (name == "Music_1")
        {
            clip = music;
        }
        //Set the clip of our efxSource audio source to the clip passed in as a parameter.
        musicSource.clip = clip;

        //Play the clip.
        if (musicSource.clip)
        {
            musicSource.Play();
        }
    }

    public void StopMusic()
    {
        musicSource.Stop();

        musicSource.clip = null;
    }

    void Start()
    {

    }

    void Update()
    {

    }
}
