using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelCompletionHandler : MonoBehaviour
{
    [SerializeField] private int currentLevelIndex;
    [SerializeField] private string levelMenuSceneName = "LevelMenu";

    void Start()
    {
        Debug.Log("LevelCompletionHandler initialized");
    }

    public void CompleteLevel()
    {
        // Find and update level menu system
        var levelMenu = FindObjectOfType<LevelMenuSystem>();
        if (levelMenu != null)
        {
            levelMenu.OnLevelCompleted(currentLevelIndex);
        }
        else
        {
            Debug.LogWarning("LevelMenuSystem not found in scene!");
        }

        // Return to level menu
        SceneManager.LoadScene(levelMenuSceneName);
    }
}