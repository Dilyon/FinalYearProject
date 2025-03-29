using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages scene transitions and ensures only one instance exists at runtime
/// </summary>
public class SceneController : MonoBehaviour
{
    // Singleton instance
    public static SceneController instance;

    /// <summary>
    /// Called when the script instance is being loaded
    /// Implements the singleton pattern to ensure only one SceneController exists
    /// </summary>
    private void Awake()
    {
        // If no instance exists, make this the instance and prevent it from being destroyed between scenes
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        // If an instance already exists, destroy this duplicate
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Loads the next scene in the build index sequence
    /// </summary>
    public void NextLevel()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
    }

    /// <summary>
    /// Loads a scene by its name
    /// </summary>
    /// <param name="sceneName">The name of the scene to load</param>
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadSceneAsync(sceneName);
    }
}