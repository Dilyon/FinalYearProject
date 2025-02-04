using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    [Header("Damage Settings")]
    [SerializeField] private float damageAmount = 10f;
    [SerializeField] private bool damageOnStay = false;
    [SerializeField] private float damageInterval = 1f;

    private float nextDamageTime;

    private void Start()
    {
        Debug.Log($"DamageDealer initialized with damage amount: {damageAmount}");
        // Verify this object has a trigger collider
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            if (!col.isTrigger)
            {
                Debug.LogError("Collider must be set as trigger!");
            }
        }
        else
        {
            Debug.LogError("No Collider component found!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Trigger Enter with: {other.gameObject.name}");

        HealthBarSystem healthSystem = other.GetComponent<HealthBarSystem>();
        if (healthSystem != null)
        {
            Debug.Log("Found HealthBarSystem - Dealing damage");
            DealDamage(healthSystem);
        }
        else
        {
            Debug.Log($"No HealthBarSystem found on {other.gameObject.name}");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!damageOnStay) return;

        if (Time.time >= nextDamageTime)
        {
            Debug.Log($"TriggerStay with: {other.gameObject.name}");

            HealthBarSystem healthSystem = other.GetComponent<HealthBarSystem>();
            if (healthSystem != null)
            {
                DealDamage(healthSystem);
                nextDamageTime = Time.time + damageInterval;
                Debug.Log($"Dealt continuous damage. Next damage time: {nextDamageTime}");
            }
        }
    }

    private void DealDamage(HealthBarSystem healthSystem)
    {
        healthSystem.TakeDamage(damageAmount);
        Debug.Log($"Dealt {damageAmount} damage");
    }
}