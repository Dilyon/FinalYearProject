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
    private HealthBarSystem healthSystem;

    private bool isGameOver = false;

    // Add fade effect duration for smooth transition
    public float fadeTime = 1.0f;

    // Reference to any player input/movement script
    // Add references to any other movement scripts you have
    private MonoBehaviour[] movementScripts;

    void Start()
    {
        Debug.Log("Game Started");
        rb = GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        currentTime = timeLimit;

        // Ensure time scale is set to 1 at the start of the game
        Time.timeScale = 1f;

        // Find and store references to any movement scripts attached to this object
        // This will help us disable them when the game is over
        movementScripts = GetComponents<MonoBehaviour>();

        // Get the HealthBarSystem component
        healthSystem = GetComponent<HealthBarSystem>();
        if (healthSystem != null)
        {
            // Subscribe to the health depleted event
            healthSystem.onHealthDepleted.AddListener(OnHealthDepleted);
        }
        else
        {
            Debug.LogWarning("HealthBarSystem not found on the player!");
        }

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

    IEnumerator LoadNextLevel()
    {
        // Freeze the ball's movement
        rb.isKinematic = true;

        // Optional: Add a fade effect or transition animation here
        // Wait for fadeTime seconds
        yield return new WaitForSecondsRealtime(fadeTime); // Using WaitForSecondsRealtime to work even when timeScale = 0

        // Get the current scene index
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // Ensure time scale is reset before loading the next scene
        Time.timeScale = 1f;

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
        // Only process game logic if the game is not over
        if (!isGameOver)
        {
            currentTime -= Time.deltaTime;
            UpdateTimerDisplay(Mathf.Ceil(currentTime).ToString());

            if (currentTime <= 0)
            {
                GameOver();
            }

            // Your player movement code might be here
            // It will not execute if isGameOver is true
        }
    }

    void FixedUpdate()
    {
        // If you have physics-based movement, make sure to check isGameOver here too
        if (isGameOver)
        {
            // Ensure the velocity is zero when game is over
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
            return;
        }

        // Your physics-based movement code would be here
    }

    // This method will disable all player movement scripts
    void DisableAllMovementScripts()
    {
        // Disable this script's ability to receive input, but keep the script enabled
        // for the game over logic to work

        // Disable velocity and make kinematic
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        // Disable all other player movement scripts
        // This is a generic approach - you may need to customize it
        // to target specific scripts in your project
        foreach (MonoBehaviour script in movementScripts)
        {
            // Skip disabling this script (BallControl)
            if (script != this)
            {
                script.enabled = false;
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

        // Pause the game by setting timeScale to 0
        Time.timeScale = 0f;

        // Disable player movement completely
        DisableAllMovementScripts();

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

        // Reset time scale before reloading
        Time.timeScale = 1f;

        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnHealthDepleted()
    {
        Debug.Log("Health depleted - Game Over");
        isGameOver = true;

        // Pause the game
        Time.timeScale = 0f;

        // Disable player movement completely
        DisableAllMovementScripts();

        ShowGameOverScreen("Health Depleted!");
    }

    // Optional - Add a method to handle level completion
    public void CompleteLevel()
    {
        if (!isGameOver)
        {
            isGameOver = true;
            DisableAllMovementScripts();
            StartCoroutine(LoadNextLevel());
        }
    }

    private void OnDestroy()
    {
        // Make sure to reset timeScale when this object is destroyed
        Time.timeScale = 1f;

        if (healthSystem != null)
        {
            healthSystem.onHealthDepleted.RemoveListener(OnHealthDepleted);
        }
    }

    // If you're using Input system directly in a separate method, make sure to check isGameOver
    // Example: If you have methods like HandleInput() or similar
    void HandleInput()
    {
        // Only process input if game is not over
        if (isGameOver)
            return;

        // Input handling code would go here
    }
}