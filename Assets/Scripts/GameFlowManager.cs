using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameFlowManager : MonoBehaviour
{
    // Singleton pattern to access the manager from anywhere
    public static GameFlowManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Scene names as constants to avoid typos
    public const string MAIN_MENU = "MainMenu";
    public const string LEVEL_SELECTION = "LevelSelection";
    public const string TUTORIAL = "Tutorial";

    // Track the highest unlocked level (1 is unlocked by default)
    private int highestUnlockedLevel = 1;
    private const string PROGRESS_KEY = "HighestUnlockedLevel";

    private void Start()
    {
        // Load the saved progress
        highestUnlockedLevel = PlayerPrefs.GetInt(PROGRESS_KEY, 1);
    }

    // Call this method when a level is completed
    public void OnLevelComplete(int levelNumber)
    {
        if (levelNumber >= highestUnlockedLevel)
        {
            highestUnlockedLevel = levelNumber + 1;
            PlayerPrefs.SetInt(PROGRESS_KEY, highestUnlockedLevel);
            PlayerPrefs.Save();
        }
        LoadLevelSelection();
    }

    // Scene loading methods
    public void LoadMainMenu()
    {
        SceneManager.LoadScene(MAIN_MENU);
    }

    public void LoadLevelSelection()
    {
        SceneManager.LoadScene(LEVEL_SELECTION);
    }

    public void LoadTutorial()
    {
        SceneManager.LoadScene(TUTORIAL);
    }

    public void LoadLevel(int levelNumber)
    {
        if (levelNumber <= highestUnlockedLevel)
        {
            SceneManager.LoadScene($"Level{levelNumber}");
        }
    }

    public int GetHighestUnlockedLevel()
    {
        return highestUnlockedLevel;
    }
}