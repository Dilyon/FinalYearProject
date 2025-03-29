using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the visibility of a UI screen or panel
/// </summary>
public class ScreenToggle : MonoBehaviour
{
    // Reference to the GameObject (screen/panel) that will be toggled
    public GameObject screenToToggle;

    // Tracks whether the screen is currently visible
    private bool isScreenVisible = false; // Start with screen hidden

    /// <summary>
    /// Called when the script is first enabled
    /// Sets the initial visibility state of the screen
    /// </summary>
    void Start()
    {
        if (screenToToggle != null)
        {
            screenToToggle.SetActive(isScreenVisible);
        }
    }

    /// <summary>
    /// Toggles the visibility of the screen
    /// Can be called from UI buttons or other scripts
    /// </summary>
    public void ToggleScreen()
    {
        isScreenVisible = !isScreenVisible;
        screenToToggle.SetActive(isScreenVisible);
    }
}