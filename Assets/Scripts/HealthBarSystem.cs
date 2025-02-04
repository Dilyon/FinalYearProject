using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class HealthBarSystem : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;
    public UnityEvent onHealthDepleted;
    private bool isDead = false;

    [Header("UI Elements")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Transform healthBarParent;
    [SerializeField] private Vector3 offset = new Vector3(0f, 2f, 0f);

    [Header("UI Settings")]
    [SerializeField] private bool alwaysFaceCamera = true;
    [SerializeField] private float displayRange = 20f;

    private Camera mainCamera;


    private void Start()
    {
        Debug.Log("HealthBarSystem Start - Initializing health system");
        currentHealth = maxHealth;  // This line is correct

        // Update this section to be more explicit
        if (healthSlider != null)
        {
            healthSlider.minValue = 0f;         // Add this line to ensure minimum is 0
            healthSlider.maxValue = maxHealth;   // This sets the maximum value
            healthSlider.value = maxHealth;      // Change this to maxHealth instead of currentHealth
            Debug.Log($"Health Slider initialized: Max={maxHealth}, Current={healthSlider.value}");
        }
        else
        {
            Debug.LogError("Health Slider is not assigned!");
        }

        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Main Camera not found!");
        }
    }

    private void LateUpdate()
    {
        if (healthBarParent == null)
        {
            Debug.LogError("Health Bar Parent is not assigned!");
            return;
        }

        // Update health bar position
        Vector3 worldPosition = transform.position + offset;
        healthBarParent.position = worldPosition;

        if (alwaysFaceCamera && mainCamera != null)
        {
            healthBarParent.LookAt(mainCamera.transform);
            healthBarParent.Rotate(0, 180, 0);
        }
    }

    public void TakeDamage(float damage)
    {
        if (damage < 0 || isDead)
        {
            return;
        }

        currentHealth = Mathf.Max(currentHealth - damage, 0);

        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }

        // Check if health reached 0
        if (currentHealth <= 0 && !isDead)
        {
            isDead = true;
            if (onHealthDepleted != null)
            {
                onHealthDepleted.Invoke();
            }
        }
    }
}