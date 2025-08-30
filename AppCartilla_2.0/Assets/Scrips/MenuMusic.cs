using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [System.Serializable]
    public class SceneMusic
    {
        public string[] sceneNames;   // Escenas que usan esta música
        public AudioClip musicClip;   // Música asociada
    }

    [Header("Configuración")]
    [SerializeField] private List<SceneMusic> sceneMusicList;
    [SerializeField] private float fadeDuration = 1f;

    private AudioSource audioSource;
    private AudioClip currentClip;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.loop = true;

            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AudioClip newClip = GetMusicForScene(scene.name);

        // Si no hay música asignada a esta escena -> parar
        if (newClip == null)
        {
            StopMusic();
            return;
        }

        // Si es la misma canción que ya suena -> no hacer nada
        if (newClip == currentClip)
            return;

        // Cambiar a la nueva música
        StartCoroutine(FadeToNewMusic(newClip));
    }

    private AudioClip GetMusicForScene(string sceneName)
    {
        foreach (var entry in sceneMusicList)
        {
            foreach (var s in entry.sceneNames)
            {
                if (s == sceneName)
                    return entry.musicClip;
            }
        }
        return null;
    }

    private System.Collections.IEnumerator FadeToNewMusic(AudioClip newClip)
    {
        // Fade out
        float startVolume = audioSource.volume;
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(startVolume, 0, t / fadeDuration);
            yield return null;
        }
        audioSource.volume = 0;

        // Cambiar clip y reproducir
        audioSource.clip = newClip;
        audioSource.Play();
        currentClip = newClip;

        // Fade in
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(0, 1, t / fadeDuration);
            yield return null;
        }
        audioSource.volume = 1;
    }

    private void StopMusic()
    {
        StartCoroutine(FadeOutAndStop());
        currentClip = null;
    }

    private System.Collections.IEnumerator FadeOutAndStop()
    {
        float startVolume = audioSource.volume;
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(startVolume, 0, t / fadeDuration);
            yield return null;
        }
        audioSource.Stop();
        audioSource.volume = 1;
    }
}
