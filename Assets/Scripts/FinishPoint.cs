using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishPoint : MonoBehaviour
{
    [SerializeField] bool goNextLevel;
    [SerializeField] string levelName;

    private void OnTriggerEnter(Collider collision) 
    {
        if (collision.CompareTag("Player"))
        {
            UnlockNewLevel();
            SceneController.instance.NextLevel();
        }
    }

    void UnlockNewLevel()
    {
        int currentUnlocked = PlayerPrefs.GetInt("UnlockedLevel", 1);
        Debug.Log($"Before unlock - UnlockedLevel: {currentUnlocked}");

        if (SceneManager.GetActiveScene().buildIndex >= PlayerPrefs.GetInt("ReachedIndex"))
        {
            PlayerPrefs.SetInt("ReachedIndex", SceneManager.GetActiveScene().buildIndex + 1);
            PlayerPrefs.SetInt("UnlockedLevel", PlayerPrefs.GetInt("UnlockedLevel", 1) + 1);
            PlayerPrefs.Save();

            Debug.Log($"After unlock - UnlockedLevel: {PlayerPrefs.GetInt("UnlockedLevel")}");
            Debug.Log($"Current scene build index: {SceneManager.GetActiveScene().buildIndex}");
        }
        else
        {
            Debug.Log("Level already unlocked - no changes made");
        }
    }
    void Reset()
    {
        PlayerPrefs.SetInt("UnlockedLevel", 15);  // Or whatever number you want
        PlayerPrefs.Save();
    }
}