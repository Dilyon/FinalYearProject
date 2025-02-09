using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

public class LevelMenu : MonoBehaviour
{
    public Button[] buttons;

    private void Awake()
    {
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);
        Debug.Log($"LevelMenu is attached to: {gameObject.name}");
        Debug.Log($"UnlockedLevel: {unlockedLevel}");
        Debug.Log($"Total buttons: {buttons.Length}");

        // First disable all buttons
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] == null)
            {
                Debug.LogError($"Button at index {i} is null! Check the Inspector.");
                continue;
            }

            buttons[i].interactable = false;

            // Remove any existing listeners to prevent duplicates
            buttons[i].onClick.RemoveAllListeners();

            // Add new listener with correct level number (i + 1)
            int levelNumber = i + 1; // Convert from 0-based to 1-based
            buttons[i].onClick.AddListener(() => {
                Debug.Log($"Clicked button index {i} for Level {levelNumber}");
                OpenLevel(levelNumber);
            });

            Debug.Log($"Set up button {i} to load Level {levelNumber}");
        }

        // Enable buttons up to unlocked level
        for (int i = 0; i < unlockedLevel; i++)
        {
            if (buttons[i] != null)
            {
                buttons[i].interactable = true;
                Debug.Log($"Button {i} enabled for Level {i + 1}, interactable: {buttons[i].interactable}");
            }
        }
    }

    public void OpenLevel(int levelId)
    {
        Debug.Log($"OpenLevel called with levelId: {levelId}");
        string levelName = "Level" + levelId;

        // Check if the scene exists in build settings
        bool sceneExists = false;
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
            Debug.Log($"Loading scene: {levelName}");
            SceneManager.LoadScene(levelName);
        }
        else
        {
            Debug.LogError($"Scene '{levelName}' is not in build settings! Please add it via File -> Build Settings");
        }
    }

    public void VerifyButtonSetup()
    {
        Debug.Log("=== Button Setup Verification ===");
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] == null)
            {
                Debug.LogError($"Button {i} is null!");
                continue;
            }

            Debug.Log($"Button {i}:");
            Debug.Log($"- Should load Level {i + 1}");
            Debug.Log($"- Name: {buttons[i].gameObject.name}");
            Debug.Log($"- Interactable: {buttons[i].interactable}");
            Debug.Log($"- onClick listeners: {buttons[i].onClick.GetPersistentEventCount()}");
            Debug.Log($"- Active in hierarchy: {buttons[i].gameObject.activeInHierarchy}");
        }
    }
}