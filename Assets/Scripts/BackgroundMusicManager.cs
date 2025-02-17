using UnityEngine;
using System.Collections;

public class SceneBackgroundMusic : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] private AudioClip musicTrack;
    [SerializeField] private bool playOnAwake = true;
    [SerializeField] private bool loopMusic = true;
    [SerializeField] private float volume = 0.5f;

    [Header("Fade Settings")]
    [SerializeField] private float fadeInDuration = 2f;
    [SerializeField] private float fadeOutDuration = 2f;

    private AudioSource audioSource;
    private Coroutine fadeCoroutine;

    private void Awake()
    {
        // Set up AudioSource in Awake instead of Start
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = musicTrack;
        audioSource.playOnAwake = false;
        audioSource.loop = loopMusic;
        audioSource.volume = 0f; // Start at 0 for fade in
    }

    private void Start()
    {
        if (playOnAwake && musicTrack != null)
        {
            PlayMusic();
        }
    }

    private void OnDisable()
    {
        // Clean up when scene unloads
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }

    public void PlayMusic()
    {
        if (audioSource == null)
        {
            Debug.LogError("AudioSource not initialized!");
            return;
        }

        if (musicTrack == null)
        {
            Debug.LogError("No music track assigned!");
            return;
        }

        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        fadeCoroutine = StartCoroutine(FadeIn());
    }

    public void StopMusic()
    {
        if (audioSource == null) return;

        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        fadeCoroutine = StartCoroutine(FadeOut());
    }

    public void PauseMusic()
    {
        if (audioSource != null)
        {
            audioSource.Pause();
        }
    }

    public void ResumeMusic()
    {
        if (audioSource != null)
        {
            audioSource.UnPause();
        }
    }

    public void SetVolume(float newVolume)
    {
        if (audioSource == null) return;

        volume = Mathf.Clamp01(newVolume);
        audioSource.volume = volume;
    }

    private IEnumerator FadeIn()
    {
        audioSource.Play();
        float timeElapsed = 0;
        while (timeElapsed < fadeInDuration)
        {
            timeElapsed += Time.deltaTime;
            float newVolume = Mathf.Lerp(0, volume, timeElapsed / fadeInDuration);
            audioSource.volume = newVolume;
            yield return null;
        }
        audioSource.volume = volume;
    }

    private IEnumerator FadeOut()
    {
        float timeElapsed = 0;
        float startVolume = audioSource.volume;
        while (timeElapsed < fadeOutDuration)
        {
            timeElapsed += Time.deltaTime;
            float newVolume = Mathf.Lerp(startVolume, 0, timeElapsed / fadeOutDuration);
            audioSource.volume = newVolume;
            yield return null;
        }
        audioSource.Stop();
        audioSource.volume = 0;
    }
}