using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenToggle : MonoBehaviour
{
    public GameObject screenToToggle;
    private bool isScreenVisible = false; // Start with screen hidden

    void Start()
    {
        if (screenToToggle != null)
        {
            screenToToggle.SetActive(isScreenVisible);
        }
    }

    public void ToggleScreen()
    {
        isScreenVisible = !isScreenVisible;
        screenToToggle.SetActive(isScreenVisible);
    }
}