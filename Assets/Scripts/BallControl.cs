using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class BallControl : MonoBehaviour
{
    public float speed = 1.0f;
    private Rigidbody rb;
    public Transform respawnPoint;

    public float timeLimit = 60f;
    private float currentTime;

    public TMP_Text timerTextTMP;
    public Text timerTextLegacy;
    public GameObject gameOverPanel;
    public TMP_Text gameOverTextTMP;
    public Text gameOverTextLegacy;
    public Button restartButton;

    private bool isGameOver = false;

    // Add fade effect duration for smooth transition
    public float fadeTime = 1.0f;

    void Start()
    {
        Debug.Log("Game Started");
        rb = GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        currentTime = timeLimit;
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
            Debug.Log("Restart button listener added");
        }
        else
        {
            Debug.LogWarning("Restart button not assigned in inspector!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger Enter:" + other.gameObject.tag);
        if (other.gameObject.CompareTag("FinishZone") && !isGameOver)
        {
            isGameOver = true;
            // Start the level transition
            StartCoroutine(LoadNextLevel());
        }
    }

    IEnumerator LoadNextLevel()
    {
        // Freeze the ball's movement
        rb.isKinematic = true;

        // Optional: Add a fade effect or transition animation here
        // Wait for fadeTime seconds
        yield return new WaitForSeconds(fadeTime);

        // Get the current scene index
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // Check if there's a next level
        if (currentSceneIndex + 1 < SceneManager.sceneCountInBuildSettings)
        {
            // Load the next level
            SceneManager.LoadScene(currentSceneIndex + 1);
        }
        else
        {
            // If this is the last level, show game completion screen
            ShowGameOverScreen("Congratulations!\nYou've completed all levels!");
        }
    }

    void Update()
    {
        if (!isGameOver)
        {
            currentTime -= Time.deltaTime;
            UpdateTimerDisplay(Mathf.Ceil(currentTime).ToString());

            if (currentTime <= 0)
            {
                GameOver();
            }
        }
    }

    private void UpdateTimerDisplay(string timeText)
    {
        if (timerTextTMP != null)
            timerTextTMP.text = "Time: " + timeText;
        else if (timerTextLegacy != null)
            timerTextLegacy.text = "Time: " + timeText;
    }

    private void UpdateGameOverText(string message)
    {
        if (gameOverTextTMP != null)
            gameOverTextTMP.text = message;
        else if (gameOverTextLegacy != null)
            gameOverTextLegacy.text = message;
    }

    void GameOver()
    {
        Debug.Log("Game Over triggered");
        isGameOver = true;
        ShowGameOverScreen("Time's Up! You lost!");
    }

    void ShowGameOverScreen(string message)
    {
        Debug.Log("Showing game over screen");
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            UpdateGameOverText(message);
        }
        else
        {
            Debug.LogWarning("Game Over Panel is not assigned!");
        }
    }

    public void RestartGame()
    {
        Debug.Log("RestartGame function called");
        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}