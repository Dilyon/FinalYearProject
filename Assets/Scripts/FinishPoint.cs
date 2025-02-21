using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class FinishPoint : MonoBehaviour
{
    [SerializeField] bool goNextLevel;
    [SerializeField] string levelName;

    private GameObject winPanel;
    private const string WIN_PANEL_NAME = "WinPanel";

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            UnlockNewLevel();

            if (SceneManager.GetActiveScene().buildIndex + 1 < SceneManager.sceneCountInBuildSettings)
            {
                SceneController.instance.NextLevel();
            }
            else
            {
                CreateAndShowWinPanel();
            }
        }
    }

    void CreateAndShowWinPanel()
    {
        // First try to find an existing win panel
        winPanel = GameObject.Find(WIN_PANEL_NAME);

        // If no panel exists, create one
        if (winPanel == null)
        {
            // Create Canvas if it doesn't exist
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                GameObject canvasObj = new GameObject("UICanvas");
                canvas = canvasObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasObj.AddComponent<CanvasScaler>();
                canvasObj.AddComponent<GraphicRaycaster>();
            }

            // Create Win Panel
            winPanel = new GameObject(WIN_PANEL_NAME);
            winPanel.transform.SetParent(canvas.transform, false);

            // Add panel components
            Image panelImage = winPanel.AddComponent<Image>();
            panelImage.color = new Color(0, 0, 0, 0.8f);

            // Set panel size to cover screen
            RectTransform rectTransform = winPanel.GetComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.sizeDelta = Vector2.zero;

            // Create "You Win!" text
            GameObject textObj = new GameObject("WinText");
            textObj.transform.SetParent(winPanel.transform, false);
            TextMeshProUGUI winText = textObj.AddComponent<TextMeshProUGUI>();
            winText.text = "Congratulation! You Win! Thank you for playing the game.";
            winText.fontSize = 30;
            winText.alignment = TextAlignmentOptions.Center;
            winText.color = Color.white;

            // Set text size and position
            RectTransform textRectTransform = textObj.GetComponent<RectTransform>();
            textRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            textRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            textRectTransform.sizeDelta = new Vector2(300, 100);
            textRectTransform.anchoredPosition = new Vector2(0, 50); // Moved up to make room for button

            // Create Main Menu Button
            GameObject buttonObj = new GameObject("MainMenuButton");
            buttonObj.transform.SetParent(winPanel.transform, false);

            // Add button component
            Button menuButton = buttonObj.AddComponent<Button>();
            Image buttonImage = buttonObj.AddComponent<Image>();
            buttonImage.color = new Color(0.2f, 0.2f, 0.2f); // Dark gray background

            // Add button text
            GameObject buttonTextObj = new GameObject("ButtonText");
            buttonTextObj.transform.SetParent(buttonObj.transform, false);
            TextMeshProUGUI buttonText = buttonTextObj.AddComponent<TextMeshProUGUI>();
            buttonText.text = "Main Menu";
            buttonText.fontSize = 24;
            buttonText.alignment = TextAlignmentOptions.Center;
            buttonText.color = Color.white;

            // Set button size and position
            RectTransform buttonRectTransform = buttonObj.GetComponent<RectTransform>();
            buttonRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            buttonRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            buttonRectTransform.sizeDelta = new Vector2(200, 50);
            buttonRectTransform.anchoredPosition = new Vector2(0, -50); // Positioned below the text

            // Set button text size
            RectTransform buttonTextRectTransform = buttonTextObj.GetComponent<RectTransform>();
            buttonTextRectTransform.anchorMin = Vector2.zero;
            buttonTextRectTransform.anchorMax = Vector2.one;
            buttonTextRectTransform.sizeDelta = Vector2.zero;

            // Add button click event
            menuButton.onClick.AddListener(() => {
                Time.timeScale = 1f; // Reset time scale
                SceneManager.LoadScene(0); // Load main menu (scene 0)
            });
        }

        winPanel.SetActive(true);
        Time.timeScale = 0f;
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