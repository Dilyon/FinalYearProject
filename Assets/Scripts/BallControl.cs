using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    public Button restartButton; // Add reference to restart button

    private bool isGameOver = false;

    void Start()
    {
        Debug.Log("Game Started");
        rb = GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        currentTime = timeLimit;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        // Add listener to restart button if assigned
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

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger Enter:" + other.gameObject.tag);
        if (other.gameObject.tag == "FinishZone")
        {
            if (!isGameOver)
            {
                ShowGameOverScreen("You Win!\nTime Remaining: " + Mathf.Ceil(currentTime).ToString());
            }
            rb.isKinematic = true;
            transform.position = respawnPoint.position;
            rb.isKinematic = false;
        }
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
        currentTime = timeLimit;
        isGameOver = false;

        rb.isKinematic = true;
        transform.position = respawnPoint.position;
        rb.isKinematic = false;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
            Debug.Log("Game Over Panel hidden");
        }
        else
        {
            Debug.LogWarning("Game Over Panel is not assigned!");
        }

        UpdateTimerDisplay(timeLimit.ToString());
        Debug.Log("Game restarted successfully");
    }
}