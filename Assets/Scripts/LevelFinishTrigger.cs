using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelFinishTrigger : MonoBehaviour
{
    private LevelCompletionHandler completionHandler;

    void Start()
    {
        completionHandler = FindObjectOfType<LevelCompletionHandler>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            completionHandler.CompleteLevel();
        }
    }
}
