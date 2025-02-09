using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectionButton : MonoBehaviour
{
    void Start()
    {
        Debug.Log($"This script is attached to: {gameObject.name}");
    }

    public string sceneName; // Name of the scene to load

    public void LoadScene()
    {
        SceneManager.LoadScene(sceneName);
    }
}
