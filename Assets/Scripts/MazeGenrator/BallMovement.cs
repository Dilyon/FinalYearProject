using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMovement : MonoBehaviour
{
    [Header("Ball Properties")]
    public float maxSpeed = 10f;
    public float rollSpeed = 5f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Set up physics properties
        rb.maxAngularVelocity = maxSpeed;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        // Tag the ball as "Player" for finish point detection
        gameObject.tag = "Player";
    }

    void FixedUpdate()
    {
        // No direct input needed - ball moves based on board tilt physics

        // Optional: Add speed limit
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
    }

    // Optional: Add effects or sounds on collision
    void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude > 2)
        {
            // Add sound or particle effects here
        }
    }
}
