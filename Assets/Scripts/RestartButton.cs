using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RestartButton : MonoBehaviour
{
    [SerializeField] private Button restartButton;

    private void Start()
    {
        // Make sure the button is assigned
        if (restartButton == null)
            restartButton = GetComponent<Button>();

        // Add the restart function to the button's click event
        restartButton.onClick.AddListener(RestartCurrentScene);
    }

    public void RestartCurrentScene()
    {
        // Get the current scene index or name
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // Reload the current scene
        SceneManager.LoadScene(currentSceneIndex);

        // Alternative method using scene name:
        // string currentSceneName = SceneManager.GetActiveScene().name;
        // SceneManager.LoadScene(currentSceneName);
    }
}
