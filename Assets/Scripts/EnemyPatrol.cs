using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    [Header("Patrol Settings")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float patrolDistance = 5f;
    [SerializeField] private bool startMovingRight = true;

    [Header("Platform Settings")]
    [SerializeField] private Transform platformTransform;
    [SerializeField] private bool useLocalSpace = true;

    [Header("Damage Settings")]
    [SerializeField] private float damageAmount = 10f;
    [SerializeField] private bool damageOnStay = false;
    [SerializeField] private float damageInterval = 1f;

    private Vector3 localStartPosition;
    private bool movingRight;
    private float leftBoundary;
    private float rightBoundary;
    private float nextDamageTime;
    private Collider enemyCollider;

    private void Start()
    {
        // Initialize patrol settings
        if (platformTransform != null && transform.parent != platformTransform)
        {
            transform.SetParent(platformTransform);
        }

        localStartPosition = transform.localPosition;
        movingRight = startMovingRight;
        leftBoundary = localStartPosition.x - patrolDistance;
        rightBoundary = localStartPosition.x + patrolDistance;

        // Initialize damage dealer settings
        enemyCollider = GetComponent<Collider>();
        if (enemyCollider != null)
        {
            enemyCollider.isTrigger = true;
        }
        else
        {
            Debug.LogError("No Collider component found on enemy!");
            // Add a trigger collider if none exists
            BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
            boxCollider.isTrigger = true;
            enemyCollider = boxCollider;
        }

        Debug.Log($"Enemy initialized - Damage: {damageAmount}, Patrol Distance: {patrolDistance}");
    }

    private void Update()
    {
        if (platformTransform == null) return;

        // Handle patrol movement
        float movement = moveSpeed * Time.deltaTime;
        Vector3 newLocalPosition = transform.localPosition;

        if (movingRight)
        {
            newLocalPosition.x += movement;
            transform.localRotation = Quaternion.Euler(0, 90, 0);
        }
        else
        {
            newLocalPosition.x -= movement;
            transform.localRotation = Quaternion.Euler(0, -90, 0);
        }

        transform.localPosition = newLocalPosition;

        // Check patrol boundaries
        if (transform.localPosition.x >= rightBoundary)
        {
            movingRight = false;
            Vector3 clampedPosition = transform.localPosition;
            clampedPosition.x = rightBoundary;
            transform.localPosition = clampedPosition;
        }
        else if (transform.localPosition.x <= leftBoundary)
        {
            movingRight = true;
            Vector3 clampedPosition = transform.localPosition;
            clampedPosition.x = leftBoundary;
            transform.localPosition = clampedPosition;
        }
    }

    // Damage Dealing Methods
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Enemy collision with: {other.gameObject.name}");
        HealthBarSystem healthSystem = other.GetComponent<HealthBarSystem>();
        if (healthSystem != null)
        {
            DealDamage(healthSystem);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!damageOnStay) return;

        if (Time.time >= nextDamageTime)
        {
            HealthBarSystem healthSystem = other.GetComponent<HealthBarSystem>();
            if (healthSystem != null)
            {
                DealDamage(healthSystem);
                nextDamageTime = Time.time + damageInterval;
                Debug.Log($"Continuous damage applied. Next damage at: {nextDamageTime}");
            }
        }
    }

    private void DealDamage(HealthBarSystem healthSystem)
    {
        healthSystem.TakeDamage(damageAmount);
        Debug.Log($"Enemy dealt {damageAmount} damage");
    }

    private void OnDrawGizmos()
    {
        if (platformTransform == null) return;

        // Draw patrol path
        Gizmos.color = Color.red;
        Vector3 worldLeftPoint = platformTransform.TransformPoint(new Vector3(leftBoundary, transform.localPosition.y, transform.localPosition.z));
        Vector3 worldRightPoint = platformTransform.TransformPoint(new Vector3(rightBoundary, transform.localPosition.y, transform.localPosition.z));

        Gizmos.DrawLine(worldLeftPoint, worldRightPoint);

        // Draw damage range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.5f); // Visual indicator of damage range
    }

    public void SetPlatform(Transform newPlatform)
    {
        platformTransform = newPlatform;
        transform.SetParent(platformTransform);
        transform.localPosition = localStartPosition;
    }
}