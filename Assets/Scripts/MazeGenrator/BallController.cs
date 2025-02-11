using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public bool debugMode = true;
    private Rigidbody rb;
    private Vector3 lastVelocity;
    private Vector3 lastPosition;
    private bool wasClickedLastFrame = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        lastPosition = transform.position;
        lastVelocity = rb.velocity;
    }

    void FixedUpdate()
    {
        if (Input.GetMouseButton(0))
        {
            if (!wasClickedLastFrame)
            {
                // Log state when click starts
                Debug.Log($"Click started! Velocity: {rb.velocity.magnitude:F2}, Position: {transform.position}");
            }

            // Check for sudden velocity changes
            if (Vector3.Distance(rb.velocity, lastVelocity) > 1f)
            {
                Debug.LogWarning($"Sudden velocity change detected!\n" +
                    $"Old velocity: {lastVelocity}\n" +
                    $"New velocity: {rb.velocity}\n" +
                    $"Delta: {Vector3.Distance(rb.velocity, lastVelocity):F2}");
            }

            // Check if position is unchanged (frozen)
            if (Vector3.Distance(transform.position, lastPosition) < 0.001f)
            {
                Debug.LogWarning($"Ball appears frozen!\n" +
                    $"Position: {transform.position}\n" +
                    $"Velocity: {rb.velocity}\n" +
                    $"Is Kinematic: {rb.isKinematic}\n" +
                    $"Is Sleeping: {rb.IsSleeping()}\n" +
                    $"Constraints: {rb.constraints}");
            }

            wasClickedLastFrame = true;
        }
        else
        {
            wasClickedLastFrame = false;
        }

        // Store current state for next frame
        lastVelocity = rb.velocity;
        lastPosition = transform.position;
    }

    void OnCollisionStay(Collision collision)
    {
        if (debugMode && Input.GetMouseButton(0))
        {
            Debug.Log($"Collision while clicked:\n" +
                $"Colliding with: {collision.gameObject.name}\n" +
                $"Contact points: {collision.contactCount}\n" +
                $"Relative velocity: {collision.relativeVelocity.magnitude:F2}");
        }
    }
}