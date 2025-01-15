using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelMenuSystem : MonoBehaviour
{
    [Header("Level Buttons")]
    [SerializeField] private Button[] levelButtons;
    [SerializeField] private string[] levelSceneNames;

    [Header("UI Elements")]
    [SerializeField] private Sprite unlockedLevelSprite;
    [SerializeField] private Sprite lockedLevelSprite;
    [SerializeField] private Color unlockedTextColor = Color.white;
    [SerializeField] private Color lockedTextColor = Color.gray;

    [Header("Button Animation")]
    [SerializeField] private float hoverScaleMultiplier = 1.1f;
    [SerializeField] private float animationSpeed = 8f;

    private Vector3[] originalScales;
    private Vector3[] targetScales;

    private void Start()
    {
        InitializeButtons();
        LoadLevelProgress();
    }

    private void InitializeButtons()
    {
        originalScales = new Vector3[levelButtons.Length];
        targetScales = new Vector3[levelButtons.Length];

        for (int i = 0; i < levelButtons.Length; i++)
        {
            int levelIndex = i; // Capture the index for the lambda expression
            originalScales[i] = levelButtons[i].transform.localScale;
            targetScales[i] = originalScales[i];

            // Add click listener
            levelButtons[i].onClick.AddListener(() => OnLevelButtonClicked(levelIndex));

            // Setup hover animations
            SetupButtonAnimations(levelButtons[i], i);
        }
    }

    private void Update()
    {
        // Update button scales
        for (int i = 0; i < levelButtons.Length; i++)
        {
            levelButtons[i].transform.localScale = Vector3.Lerp(
                levelButtons[i].transform.localScale,
                targetScales[i],
                Time.deltaTime * animationSpeed
            );
        }
    }

    private void LoadLevelProgress()
    {
        int highestUnlockedLevel = PlayerPrefs.GetInt("HighestUnlockedLevel", 0);

        for (int i = 0; i < levelButtons.Length; i++)
        {
            bool isLevelUnlocked = i <= highestUnlockedLevel;
            SetLevelButtonState(i, isLevelUnlocked);
        }
    }

    private void SetLevelButtonState(int levelIndex, bool unlocked)
    {
        Button button = levelButtons[levelIndex];
        Image buttonImage = button.GetComponent<Image>();
        TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();

        // Set button interactability
        button.interactable = unlocked;

        // Set button appearance
        buttonImage.sprite = unlocked ? unlockedLevelSprite : lockedLevelSprite;
        buttonText.color = unlocked ? unlockedTextColor : lockedTextColor;

        // Update button text
        buttonText.text = $"Level {levelIndex + 1}";
    }

    private void SetupButtonAnimations(Button button, int index)
    {
        // Add hover animations only for unlocked levels
        if (button.interactable)
        {
            // Add EventTrigger component if it doesn't exist
            EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>() ??
                                 button.gameObject.AddComponent<EventTrigger>();

            // Setup pointer enter event
            EventTrigger.Entry enterEntry = new EventTrigger.Entry();
            enterEntry.eventID = EventTriggerType.PointerEnter;
            enterEntry.callback.AddListener((data) =>
            {
                targetScales[index] = originalScales[index] * hoverScaleMultiplier;
            });
            trigger.triggers.Add(enterEntry);

            // Setup pointer exit event
            EventTrigger.Entry exitEntry = new EventTrigger.Entry();
            exitEntry.eventID = EventTriggerType.PointerExit;
            exitEntry.callback.AddListener((data) =>
            {
                targetScales[index] = originalScales[index];
            });
            trigger.triggers.Add(exitEntry);
        }
    }

    private void OnLevelButtonClicked(int levelIndex)
    {
        if (levelIndex < levelSceneNames.Length)
        {
            SceneManager.LoadScene(levelSceneNames[levelIndex]);
        }
    }

    // Call this method when a level is completed
    public void OnLevelCompleted(int levelIndex)
    {
        int currentHighestLevel = PlayerPrefs.GetInt("HighestUnlockedLevel", 0);
        if (levelIndex + 1 > currentHighestLevel)
        {
            PlayerPrefs.SetInt("HighestUnlockedLevel", levelIndex + 1);
            PlayerPrefs.Save();
        }
    }

    // Optional: Reset progress (for testing)
    public void ResetProgress()
    {
        PlayerPrefs.DeleteKey("HighestUnlockedLevel");
        PlayerPrefs.Save();
        LoadLevelProgress();
    }

}