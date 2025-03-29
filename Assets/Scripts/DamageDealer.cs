using UnityEngine;

/// <summary>
/// Handles dealing damage to objects with HealthBarSystem components on collision
/// </summary>
public class DamageDealer : MonoBehaviour
{
    [Header("Damage Settings")]
    [SerializeField] private float damageAmount = 10f;     // Amount of damage to deal on each hit
    [SerializeField] private bool damageOnStay = false;    // Whether to deal continuous damage while object stays in trigger
    [SerializeField] private float damageInterval = 1f;    // Time between damage instances when damageOnStay is true
    private float nextDamageTime;                         // Time when next continuous damage can be applied

    /// <summary>
    /// Initializes the damage dealer and validates component requirements
    /// </summary>
    private void Start()
    {
        // Initialize damage timer
        nextDamageTime = 0f;

        // Verify this object has a trigger collider
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            if (!col.isTrigger)
            {
                // Warning if collider is not set as a trigger
                Debug.LogError("Collider must be set as trigger!");
            }
        }
        else
        {
            // Warning if no collider is found
            Debug.LogError("No Collider component found!");
        }
    }

    /// <summary>
    /// Called when another collider enters this object's trigger area
    /// </summary>
    /// <param name="other">The collider that entered the trigger</param>
    private void OnTriggerEnter(Collider other)
    {
        // Try to get health system from the colliding object
        HealthBarSystem healthSystem = other.GetComponent<HealthBarSystem>();

        if (healthSystem != null)
        {
            // Deal damage if health system exists
            DealDamage(healthSystem);
        }
    }

    /// <summary>
    /// Called every frame while another collider is inside this object's trigger area
    /// </summary>
    /// <param name="other">The collider that is staying in the trigger</param>
    private void OnTriggerStay(Collider other)
    {
        // Skip if continuous damage is disabled
        if (!damageOnStay) return;

        // Check if enough time has passed since last damage
        if (Time.time >= nextDamageTime)
        {
            // Try to get health system from the colliding object
            HealthBarSystem healthSystem = other.GetComponent<HealthBarSystem>();

            if (healthSystem != null)
            {
                // Deal damage if health system exists
                DealDamage(healthSystem);

                // Set time for next damage application
                nextDamageTime = Time.time + damageInterval;
            }
        }
    }

    /// <summary>
    /// Applies damage to the specified health system
    /// </summary>
    /// <param name="healthSystem">The health system to apply damage to</param>
    private void DealDamage(HealthBarSystem healthSystem)
    {
        // Apply the damage to the health system
        healthSystem.TakeDamage(damageAmount);
    }
}