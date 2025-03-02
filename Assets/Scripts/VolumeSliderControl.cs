using UnityEngine;
using UnityEngine.UI;
using TMPro; // Add TextMeshPro namespace

public class VolumeSliderController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private SceneBackgroundMusic musicManager;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Image fillImage;
    [SerializeField] private Color minVolumeColor = Color.gray;
    [SerializeField] private Color maxVolumeColor = Color.green;

    [Header("Optional Elements")]
    [SerializeField] private TextMeshProUGUI volumePercentText;
    [SerializeField] private Image volumeIcon;
    [SerializeField] private Sprite muteSprite;
    [SerializeField] private Sprite lowVolumeSprite;
    [SerializeField] private Sprite highVolumeSprite;

    [Header("Persistence")]
    [SerializeField] private string volumePrefsKey = "MusicVolume";
    [SerializeField] private string mutedPrefsKey = "MusicMuted";

    private float lastVolume = 0.5f; // For mute/unmute feature
    private bool isMuted = false;

    private void Start()
    {
        // Find references if not assigned
        if (musicManager == null)
            musicManager = FindObjectOfType<SceneBackgroundMusic>();

        if (volumeSlider == null)
            volumeSlider = GetComponent<Slider>();

        // Set up slider
        volumeSlider.minValue = 0f;
        volumeSlider.maxValue = 1f;

        // Load saved volume
        LoadVolumeSettings();

        // Add listener for slider value change
        volumeSlider.onValueChanged.AddListener(OnSliderValueChanged);

        // Apply initial volume
        ApplyVolumeSettings();
    }

    private void LoadVolumeSettings()
    {
        // Load volume value (default to 0.5 if not found)
        lastVolume = PlayerPrefs.GetFloat(volumePrefsKey, 0.5f);

        // Load muted state
        isMuted = PlayerPrefs.GetInt(mutedPrefsKey, 0) == 1;

        // Set the initial slider value
        volumeSlider.value = isMuted ? 0f : lastVolume;

        // Update the UI to match the loaded value
        UpdateVolumeUI(volumeSlider.value);
    }

    private void UpdateVolumeUI(float volume)
    {
        // Update fill color
        if (fillImage != null)
        {
            fillImage.color = Color.Lerp(minVolumeColor, maxVolumeColor, volume);
        }

        // Update text display
        if (volumePercentText != null)
        {
            int percentage = Mathf.RoundToInt(volume * 100);
            volumePercentText.text = percentage + "%";
        }

        // Update icon
        UpdateVolumeIcon(volume);
    }

    private void ApplyVolumeSettings()
    {
        // This triggers the OnSliderValueChanged event
        volumeSlider.value = isMuted ? 0f : lastVolume;
    }

    public void OnSliderValueChanged(float volume)
    {
        // Update music volume
        if (musicManager != null)
        {
            musicManager.SetVolume(volume);
        }

        // Update UI elements
        UpdateVolumeUI(volume);

        // Update muted state
        isMuted = (volume <= 0);

        // Save non-zero volume for unmuting
        if (volume > 0)
        {
            lastVolume = volume;
        }

        // Save settings whenever they change
        SaveVolumeSettings();
    }

    private void SaveVolumeSettings()
    {
        // Save the actual volume level (not zero when muted)
        PlayerPrefs.SetFloat(volumePrefsKey, lastVolume);

        // Save muted state
        PlayerPrefs.SetInt(mutedPrefsKey, isMuted ? 1 : 0);

        // Make sure the data is written to disk immediately
        PlayerPrefs.Save();
    }

    private void UpdateVolumeIcon(float volume)
    {
        if (volumeIcon != null && muteSprite != null && lowVolumeSprite != null && highVolumeSprite != null)
        {
            if (volume <= 0)
            {
                volumeIcon.sprite = muteSprite;
            }
            else if (volume < 0.5f)
            {
                volumeIcon.sprite = lowVolumeSprite;
            }
            else
            {
                volumeIcon.sprite = highVolumeSprite;
            }
        }
    }

    public void ToggleMute()
    {
        if (volumeSlider.value > 0)
        {
            // Mute
            volumeSlider.value = 0;
        }
        else
        {
            // Unmute
            volumeSlider.value = lastVolume > 0 ? lastVolume : 0.5f;
        }
    }

    public void SetVolume(float newVolume)
    {
        volumeSlider.value = Mathf.Clamp01(newVolume);
    }
}