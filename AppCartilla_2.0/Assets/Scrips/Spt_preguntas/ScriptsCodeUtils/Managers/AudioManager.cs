using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    [Header("Audio Sources")]
    private AudioSource musicSource;
    private AudioSource sfxSource;

    [Header("Clips")]
    public AudioClip songFondo;
    public AudioClip songRuleta;
    public AudioClip songCorrect;
    public AudioClip songIncorrect;

    protected new void Awake()
    {
        base.Awake(); // Llamamos al Awake del Singleton
        musicSource = gameObject.AddComponent<AudioSource>();
        sfxSource = gameObject.AddComponent<AudioSource>();

        musicSource.loop = true;
        DontDestroyOnLoad(gameObject);
    }

    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (clip == null) return;
        musicSource.Stop();
        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.Play();
    }

    public void StopMusic()
    {
        if (musicSource != null) // Evita el error al salir del modo Play
        {
            musicSource.Stop();
        }
    }


    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip);
    }
}
