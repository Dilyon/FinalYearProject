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

    [Header("Persistence")]
    [SerializeField] private string volumePrefsKey = "MusicVolume";
    [SerializeField] private string mutedPrefsKey = "MusicMuted";
    [SerializeField] private bool loadSavedVolume = true;

    private AudioSource audioSource;
    private Coroutine fadeCoroutine;
    private bool isPlayingMusic = false;

    private void Awake()
    {
        // Set up AudioSource in Awake instead of Start
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.clip = musicTrack;
        audioSource.playOnAwake = false;
        audioSource.loop = loopMusic;
        audioSource.ignoreListenerPause = true; // Make sure audio plays even when game is paused

        // Load saved volume settings if enabled
        if (loadSavedVolume)
        {
            bool isMuted = PlayerPrefs.GetInt(mutedPrefsKey, 0) == 1;

            if (isMuted)
            {
                volume = 0f;
            }
            else
            {
                volume = PlayerPrefs.GetFloat(volumePrefsKey, volume);
            }
        }

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
            isPlayingMusic = false;
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

        // Only attempt to stop if currently playing
        if (audioSource.isPlaying)
        {
            Debug.Log($"Stopping music on {gameObject.name}");
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }
            fadeCoroutine = StartCoroutine(FadeOut());
        }
        else
        {
            Debug.Log($"Music already stopped on {gameObject.name}");
        }
    }

    public void PauseMusic()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Pause();
        }
    }

    public void ResumeMusic()
    {
        if (audioSource != null && !audioSource.isPlaying && isPlayingMusic)
        {
            audioSource.UnPause();
        }
    }

    public void SetVolume(float newVolume)
    {
        if (audioSource == null) return;
        volume = Mathf.Clamp01(newVolume);
        audioSource.volume = volume;

        // Save volume preference
        PlayerPrefs.SetFloat(volumePrefsKey, volume);
        PlayerPrefs.SetInt(mutedPrefsKey, volume <= 0 ? 1 : 0);
        PlayerPrefs.Save();
    }

    private IEnumerator FadeIn()
    {
        Debug.Log($"Starting fade in music on {gameObject.name}");
        audioSource.Play();
        isPlayingMusic = true;
        float timeElapsed = 0;

        while (timeElapsed < fadeInDuration)
        {
            // Use unscaledDeltaTime to work when game is paused
            timeElapsed += Time.unscaledDeltaTime;
            float newVolume = Mathf.Lerp(0, volume, timeElapsed / fadeInDuration);
            audioSource.volume = newVolume;
            yield return null;
        }

        audioSource.volume = volume;
        Debug.Log($"Fade in complete on {gameObject.name}, volume: {audioSource.volume}");
    }

    private IEnumerator FadeOut()
    {
        Debug.Log($"Starting fade out music on {gameObject.name}");
        float timeElapsed = 0;
        float startVolume = audioSource.volume;

        while (timeElapsed < fadeOutDuration)
        {
            // Use unscaledDeltaTime to work when game is paused
            timeElapsed += Time.unscaledDeltaTime;
            float newVolume = Mathf.Lerp(startVolume, 0, timeElapsed / fadeOutDuration);
            audioSource.volume = newVolume;
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = 0;
        isPlayingMusic = false;
        Debug.Log($"Background music stopped and faded out on {gameObject.name}");
    }
}