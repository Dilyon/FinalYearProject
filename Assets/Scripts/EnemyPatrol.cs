using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls an enemy that patrols back and forth along a platform and deals damage to the player
/// </summary>
public class EnemyPatrol : MonoBehaviour
{
    [Header("Patrol Settings")]
    [SerializeField] private float moveSpeed = 2f;     // How fast the enemy moves
    [SerializeField] private float patrolDistance = 5f; // How far the enemy travels from its starting position
    [SerializeField] private bool startMovingRight = true; // Initial movement direction

    [Header("Platform Settings")]
    [SerializeField] private Transform platformTransform; // The platform the enemy patrols on
    [SerializeField] private bool useLocalSpace = true;   // Whether to use local coordinates relative to the platform

    [Header("Damage Settings")]
    [SerializeField] private float damageAmount = 10f;    // How much damage the enemy deals
    [SerializeField] private bool damageOnStay = false;   // Whether damage occurs continually while touching the enemy
    [SerializeField] private float damageInterval = 1f;   // Time between damage ticks if damageOnStay is true

    private Vector3 localStartPosition;  // Starting position in local space
    private bool movingRight;            // Current movement direction
    private float leftBoundary;          // Left patrol boundary
    private float rightBoundary;         // Right patrol boundary
    private float nextDamageTime;        // When the next damage can be applied
    private Collider enemyCollider;      // Reference to the enemy's collider component

    private void Start()
    {
        // Setup the enemy's parent relationship with the platform
        if (platformTransform != null && transform.parent != platformTransform)
        {
            transform.SetParent(platformTransform);
        }

        // Initialize patrol boundaries based on starting position
        localStartPosition = transform.localPosition;
        movingRight = startMovingRight;
        leftBoundary = localStartPosition.x - patrolDistance;
        rightBoundary = localStartPosition.x + patrolDistance;

        // Setup collision detection
        enemyCollider = GetComponent<Collider>();
        if (enemyCollider != null)
        {
            // Ensure the collider is set as a trigger for OnTrigger events
            enemyCollider.isTrigger = true;
        }
        else
        {
            // Add a collider if one doesn't exist
            Debug.LogError("No Collider component found on enemy!");
            BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
            boxCollider.isTrigger = true;
            enemyCollider = boxCollider;
        }

        Debug.Log($"Enemy initialized - Damage: {damageAmount}, Patrol Distance: {patrolDistance}");
    }

    private void Update()
    {
        // Skip movement if there's no platform
        if (platformTransform == null) return;

        // Calculate how much to move this frame
        float movement = moveSpeed * Time.deltaTime;
        Vector3 newLocalPosition = transform.localPosition;

        // Move in the current direction and rotate to face that direction
        if (movingRight)
        {
            newLocalPosition.x += movement;
            transform.localRotation = Quaternion.Euler(0, 90, 0);  // Rotate to face right
        }
        else
        {
            newLocalPosition.x -= movement;
            transform.localRotation = Quaternion.Euler(0, -90, 0); // Rotate to face left
        }

        transform.localPosition = newLocalPosition;

        // Check if the enemy has reached a boundary and reverse direction if needed
        if (transform.localPosition.x >= rightBoundary)
        {
            movingRight = false;
            // Ensure the enemy doesn't go beyond the boundary
            Vector3 clampedPosition = transform.localPosition;
            clampedPosition.x = rightBoundary;
            transform.localPosition = clampedPosition;
        }
        else if (transform.localPosition.x <= leftBoundary)
        {
            movingRight = true;
            // Ensure the enemy doesn't go beyond the boundary
            Vector3 clampedPosition = transform.localPosition;
            clampedPosition.x = leftBoundary;
            transform.localPosition = clampedPosition;
        }
    }

    // Called when something enters the enemy's trigger collider
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Enemy collision with: {other.gameObject.name}");
        // Check if the colliding object has health
        HealthBarSystem healthSystem = other.GetComponent<HealthBarSystem>();
        if (healthSystem != null)
        {
            DealDamage(healthSystem);
        }
    }

    // Called every frame while something remains in the enemy's trigger collider
    private void OnTriggerStay(Collider other)
    {
        // Only apply continuous damage if the setting is enabled
        if (!damageOnStay) return;

        // Check if enough time has passed since the last damage application
        if (Time.time >= nextDamageTime)
        {
            HealthBarSystem healthSystem = other.GetComponent<HealthBarSystem>();
            if (healthSystem != null)
            {
                DealDamage(healthSystem);
                // Schedule the next damage application
                nextDamageTime = Time.time + damageInterval;
                Debug.Log($"Continuous damage applied. Next damage at: {nextDamageTime}");
            }
        }
    }

    // Helper method to apply damage to a health system
    private void DealDamage(HealthBarSystem healthSystem)
    {
        healthSystem.TakeDamage(damageAmount);
        Debug.Log($"Enemy dealt {damageAmount} damage");
    }

    // Used to visualize the patrol path and damage range in the Unity editor
    private void OnDrawGizmos()
    {
        if (platformTransform == null) return;

        // Draw a red line showing the patrol path
        Gizmos.color = Color.red;
        Vector3 worldLeftPoint = platformTransform.TransformPoint(new Vector3(leftBoundary, transform.localPosition.y, transform.localPosition.z));
        Vector3 worldRightPoint = platformTransform.TransformPoint(new Vector3(rightBoundary, transform.localPosition.y, transform.localPosition.z));

        Gizmos.DrawLine(worldLeftPoint, worldRightPoint);

        // Draw a yellow sphere showing the damage range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }

    // Public method to change the platform this enemy patrols on
    public void SetPlatform(Transform newPlatform)
    {
        platformTransform = newPlatform;
        transform.SetParent(platformTransform);
        transform.localPosition = localStartPosition;
    }
}