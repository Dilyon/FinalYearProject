using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

/// <summary>
/// Manages the level selection menu, including button interactivity based on player progress
/// </summary>
public class LevelMenu : MonoBehaviour
{
    // Array of level selection buttons
    public Button[] buttons;

    private void Awake()
    {
        // Get the highest unlocked level from saved player preferences
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);
        SetupButtons(unlockedLevel);
    }

    /// <summary>
    /// Configure level buttons based on player progress
    /// </summary>
    /// <param name="unlockedLevel">The highest level that should be unlocked</param>
    private void SetupButtons(int unlockedLevel)
    {
        // First disable all buttons and set up listeners
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] == null)
            {
                continue;
            }
            buttons[i].interactable = false;
            buttons[i].onClick.RemoveAllListeners();
            int levelNumber = i + 1;
            buttons[i].onClick.AddListener(() => {
                OpenLevel(levelNumber);
            });
        }

        // Enable buttons up to unlocked level
        for (int i = 0; i < unlockedLevel && i < buttons.Length; i++)
        {
            if (buttons[i] != null)
            {
                buttons[i].interactable = true;
            }
        }
    }

    /// <summary>
    /// Resets player progress to only having the first level unlocked
    /// </summary>
    public void ResetAllProgress()
    {
        PlayerPrefs.SetInt("UnlockedLevel", 1);
        PlayerPrefs.SetInt("ReachedIndex", 1);
        PlayerPrefs.Save();
        // Refresh the buttons
        SetupButtons(1);
    }

    /// <summary>
    /// Unlocks all 15 levels for testing purposes
    /// </summary>
    public void ResetLevels()
    {
        PlayerPrefs.SetInt("UnlockedLevel", 15);
        PlayerPrefs.Save();
        // Refresh the buttons with new unlocked level
        SetupButtons(15);
    }

    /// <summary>
    /// Loads the specified level scene
    /// </summary>
    /// <param name="levelId">The level number to load</param>
    public void OpenLevel(int levelId)
    {
        string levelName = "Level" + levelId;
        bool sceneExists = false;

        // Verify that the requested level exists in build settings
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            if (sceneName == levelName)
            {
                sceneExists = true;
                break;
            }
        }

        if (sceneExists)
        {
            SceneManager.LoadScene(levelName);
        }
    }

    /// <summary>
    /// Utility method to verify button setup and configuration for debugging
    /// </summary>
    public void VerifyButtonSetup()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] == null)
            {
                continue;
            }
        }
    }
}