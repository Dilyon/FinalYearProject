using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class LevelMenu : MonoBehaviour
{
    public Button[] buttons;
    private void Awake()
    {
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);
        Debug.Log($"UnlockedLevel: {unlockedLevel}");
        Debug.Log($"Total buttons: {buttons.Length}");

        // First disable all buttons
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].interactable = false;
            Debug.Log($"Button {i} initially set to: {buttons[i].interactable}");
        }
        for (int i = 0; i < unlockedLevel; i++)
        {
            buttons[i].interactable = true;
            Debug.Log($"Button {i} enabled, now set to: {buttons[i].interactable}");
        }
    }

    public void ResetAllProgress()
    {
        PlayerPrefs.SetInt("UnlockedLevel", 1);  // Reset to only level 1 unlocked
        PlayerPrefs.SetInt("ReachedIndex", 1);   // Reset reached index
        PlayerPrefs.Save();

        // Refresh the level buttons immediately
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].interactable = false;
        }
        buttons[0].interactable = true;  // Only first level interactable

        Debug.Log("Game progress reset - only Level 1 is now unlocked");
    }

    // Add this method to help reset if needed
    public void ResetLevels()
    {
        PlayerPrefs.SetInt("UnlockedLevel", 5); // Change this number to test different values
        PlayerPrefs.Save();
        Debug.Log("Levels reset - UnlockedLevel is now: " + PlayerPrefs.GetInt("UnlockedLevel"));
    }

    public void OpenLevel(int levelId)
    {
        string levelName = "Level " + levelId;
        SceneManager.LoadScene(levelName);
    }
}