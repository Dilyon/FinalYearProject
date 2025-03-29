using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Controls the ball's movement, game state, and UI interactions
/// </summary>
public class BallControl : MonoBehaviour
{
    // Movement properties
    public float speed = 1.0f;            // Speed multiplier for ball movement
    private Rigidbody rb;                 // Reference to the ball's Rigidbody component
    public Transform respawnPoint;        // Point where the ball respawns if needed

    // Timer properties
    public float timeLimit = 60f;         // Total time allowed for the level in seconds
    private float currentTime;            // Current remaining time

    // UI references
    public TMP_Text timerTextTMP;         // Reference to TextMeshPro timer display
    public Text timerTextLegacy;          // Reference to legacy UI Text timer display
    public GameObject gameOverPanel;      // Panel to show when game is over
    public TMP_Text gameOverTextTMP;      // TextMeshPro text to display game over message
    public Text gameOverTextLegacy;       // Legacy UI text to display game over message
    public Button restartButton;          // Button to restart the game

    // Health system reference
    private HealthBarSystem healthSystem;  // Reference to health system component

    // Game state
    private bool isGameOver = false;      // Tracks if the game is over

    // Transition properties
    public float fadeTime = 1.0f;         // Time for scene transition fade effect

    // Movement script references
    private MonoBehaviour[] movementScripts;  // Array of all movement scripts on this object

    /// <summary>
    /// Initializes components and game state
    /// </summary>
    void Start()
    {
        // Get and configure Rigidbody
        rb = GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        // Initialize timer
        currentTime = timeLimit;

        // Ensure normal time flow
        Time.timeScale = 1f;

        // Get all movement scripts on this object
        movementScripts = GetComponents<MonoBehaviour>();

        // Set up health system connections
        healthSystem = GetComponent<HealthBarSystem>();
        if (healthSystem != null)
        {
            // Listen for health depletion events
            healthSystem.onHealthDepleted.AddListener(OnHealthDepleted);
        }

        // Set up UI elements
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);  // Hide game over panel initially
        if (restartButton != null)
        {
            // Add listener for restart button clicks
            restartButton.onClick.AddListener(RestartGame);
        }
    }

    /// <summary>
    /// Coroutine to handle level transition with fade effect
    /// </summary>
    IEnumerator LoadNextLevel()
    {
        // Freeze ball movement during transition
        rb.isKinematic = true;

        // Wait for fade effect duration
        yield return new WaitForSecondsRealtime(fadeTime);

        // Get current scene index
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // Reset time scale before loading new scene
        Time.timeScale = 1f;

        // Check if there's another level to load
        if (currentSceneIndex + 1 < SceneManager.sceneCountInBuildSettings)
        {
            // Load the next level
            SceneManager.LoadScene(currentSceneIndex + 1);
        }
        else
        {
            // If this was the final level, show completion message
            ShowGameOverScreen("Congratulations!\nYou've completed all levels!");
        }
    }

    /// <summary>
    /// Updates game state every frame
    /// </summary>
    void Update()
    {
        // Only update if game is still active
        if (!isGameOver)
        {
            // Decrease timer
            currentTime -= Time.deltaTime;

            // Update timer display, rounding up to nearest second
            UpdateTimerDisplay(Mathf.Ceil(currentTime).ToString());

            // Check for time expiration
            if (currentTime <= 0)
            {
                GameOver();
            }
        }
    }

    /// <summary>
    /// Handles physics updates at fixed intervals
    /// </summary>
    void FixedUpdate()
    {
        // If game is over, stop all physics movement
        if (isGameOver)
        {
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
            return;
        }

        // Physics-based movement would normally go here
    }

    /// <summary>
    /// Disables all movement scripts to freeze player control
    /// </summary>
    void DisableAllMovementScripts()
    {
        // Stop physics movement
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;  // Make kinematic to prevent further physics
        }

        // Disable all movement scripts except this one
        foreach (MonoBehaviour script in movementScripts)
        {
            if (script != this)
            {
                script.enabled = false;
            }
        }
    }

    /// <summary>
    /// Updates the timer display with the current time
    /// </summary>
    /// <param name="timeText">The time value to display</param>
    private void UpdateTimerDisplay(string timeText)
    {
        // Update TextMeshPro text if available
        if (timerTextTMP != null)
            timerTextTMP.text = "Time: " + timeText;
        // Otherwise update legacy UI Text
        else if (timerTextLegacy != null)
            timerTextLegacy.text = "Time: " + timeText;
    }

    /// <summary>
    /// Updates the game over message text
    /// </summary>
    /// <param name="message">Message to display on game over screen</param>
    private void UpdateGameOverText(string message)
    {
        // Update TextMeshPro text if available
        if (gameOverTextTMP != null)
            gameOverTextTMP.text = message;
        // Otherwise update legacy UI Text
        else if (gameOverTextLegacy != null)
            gameOverTextLegacy.text = message;
    }

    /// <summary>
    /// Handles game over state when player fails
    /// </summary>
    void GameOver()
    {
        // Set game over flag
        isGameOver = true;

        // Freeze game time
        Time.timeScale = 0f;

        // Disable player movement
        DisableAllMovementScripts();

        // Show game over UI with message
        ShowGameOverScreen("Time's Up! You lost!");
    }

    /// <summary>
    /// Displays the game over screen with a specific message
    /// </summary>
    /// <param name="message">Message to display</param>
    void ShowGameOverScreen(string message)
    {
        // Show panel if available
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            // Update text with message
            UpdateGameOverText(message);
        }
    }

    /// <summary>
    /// Restarts the current level
    /// </summary>
    public void RestartGame()
    {
        // Reset time scale
        Time.timeScale = 1f;

        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Event handler for when player health reaches zero
    /// </summary>
    private void OnHealthDepleted()
    {
        // Set game over flag
        isGameOver = true;

        // Freeze game time
        Time.timeScale = 0f;

        // Disable player movement
        DisableAllMovementScripts();

        // Show game over UI with health depleted message
        ShowGameOverScreen("Health Depleted!");
    }

    /// <summary>
    /// Handles successful level completion
    /// </summary>
    public void CompleteLevel()
    {
        // Only proceed if game is still active
        if (!isGameOver)
        {
            // Set game over flag
            isGameOver = true;

            // Disable player movement
            DisableAllMovementScripts();

            // Start transition to next level
            StartCoroutine(LoadNextLevel());
        }
    }

    /// <summary>
    /// Cleanup when object is destroyed
    /// </summary>
    private void OnDestroy()
    {
        // Ensure time scale is reset
        Time.timeScale = 1f;

        // Unsubscribe from health system events
        if (healthSystem != null)
        {
            healthSystem.onHealthDepleted.RemoveListener(OnHealthDepleted);
        }
    }

    /// <summary>
    /// Handles player input
    /// </summary>
    void HandleInput()
    {
        // Skip input processing if game is over
        if (isGameOver)
            return;
    }
}