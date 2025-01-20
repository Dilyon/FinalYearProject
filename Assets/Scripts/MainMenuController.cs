using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
public class MainMenuController : MonoBehaviour
{
    [Header("Button References")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button tutorialButton;
    [Header("Animation Settings")]
    [SerializeField] private float hoverScaleMultiplier = 1.1f;
    [SerializeField] private float animationSpeed = 8f;
    [Header("Scene Names")]
    [SerializeField] private string gameSceneName = "GameScene";
    [SerializeField] private string tutorialSceneName = "TutorialScene";
    private Vector3 playButtonOriginalScale;
    private Vector3 tutorialButtonOriginalScale;
    private Vector3 playButtonTargetScale;
    private Vector3 tutorialButtonTargetScale;
    private void Start()
    {
        // Store original scales
        playButtonOriginalScale = playButton.transform.localScale;
        tutorialButtonOriginalScale = tutorialButton.transform.localScale;
        // Initialize target scales
        playButtonTargetScale = playButtonOriginalScale;
        tutorialButtonTargetScale = tutorialButtonOriginalScale;
        // Setup button listeners
        playButton.onClick.AddListener(OnPlayButtonClicked);
        tutorialButton.onClick.AddListener(OnTutorialButtonClicked);
        // Add event trigger components for hover animations
        SetupButtonAnimations(playButton);
        SetupButtonAnimations(tutorialButton);
        // Ensure cursor is visible and not locked
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    private void Update()
    {
        // Smooth button scaling
        playButton.transform.localScale = Vector3.Lerp(
            playButton.transform.localScale,
            playButtonTargetScale,
            Time.deltaTime * animationSpeed
        );
        tutorialButton.transform.localScale = Vector3.Lerp(
            tutorialButton.transform.localScale,
            tutorialButtonTargetScale,
            Time.deltaTime * animationSpeed
        );
    }
    private void SetupButtonAnimations(Button button)
    {
        // Add EventTrigger component if it doesn't exist
        EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>() ??
                             button.gameObject.AddComponent<EventTrigger>();
        // Setup pointer enter event
        EventTrigger.Entry enterEntry = new EventTrigger.Entry();
        enterEntry.eventID = EventTriggerType.PointerEnter;
        enterEntry.callback.AddListener((data) => {
            if (button == playButton)
                playButtonTargetScale = playButtonOriginalScale * hoverScaleMultiplier;
            else
                tutorialButtonTargetScale = tutorialButtonOriginalScale * hoverScaleMultiplier;
        });
        trigger.triggers.Add(enterEntry);
        // Setup pointer exit event
        EventTrigger.Entry exitEntry = new EventTrigger.Entry();
        exitEntry.eventID = EventTriggerType.PointerExit;
        exitEntry.callback.AddListener((data) => {
            if (button == playButton)
                playButtonTargetScale = playButtonOriginalScale;
            else
                tutorialButtonTargetScale = tutorialButtonOriginalScale;
        });
        trigger.triggers.Add(exitEntry);
    }
    private void OnPlayButtonClicked()
    {
        playButtonTargetScale = playButtonOriginalScale * 0.9f;
        Invoke(nameof(LoadGameScene), 0.1f);
    }
    private void OnTutorialButtonClicked()
    {
        tutorialButtonTargetScale = tutorialButtonOriginalScale * 0.9f;
        Invoke(nameof(LoadTutorialScene), 0.1f);
    }
    private void LoadGameScene()
    {
        SceneManager.LoadScene(gameSceneName);
    }
    private void LoadTutorialScene()
    {
        SceneManager.LoadScene(tutorialSceneName);
    }
    private void OnDestroy()
    {
        // Clean up listeners
        playButton.onClick.RemoveAllListeners();
        tutorialButton.onClick.RemoveAllListeners();
    }
}