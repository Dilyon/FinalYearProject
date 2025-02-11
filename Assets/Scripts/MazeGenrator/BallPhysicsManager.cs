using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Attach this to the ball GameObject
public class BallPhysicsManager : MonoBehaviour
{
    private Rigidbody rb;
    private SphereCollider sphereCollider;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        sphereCollider = GetComponent<SphereCollider>();

        // Optimize ball physics settings
        if (rb != null)
        {
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.maxDepenetrationVelocity = 10f; // Limit penetration recovery speed
            rb.constraints = RigidbodyConstraints.None;
            rb.mass = 1f;
            rb.drag = 0.5f;
            rb.angularDrag = 0.5f;
        }

        // Optimize collider settings
        if (sphereCollider != null)
        {
            sphereCollider.material = CreateBouncyPhysicsMaterial();
            sphereCollider.contactOffset = 0.01f; // Tighter contact generation
        }
    }

    private PhysicMaterial CreateBouncyPhysicsMaterial()
    {
        PhysicMaterial material = new PhysicMaterial("BallPhysicsMaterial");
        material.bounciness = 0.1f;         // Slight bounce
        material.staticFriction = 0.6f;     // Good grip when not moving
        material.dynamicFriction = 0.6f;    // Good grip while moving
        material.frictionCombine = PhysicMaterialCombine.Average;
        material.bounceCombine = PhysicMaterialCombine.Minimum;
        return material;
    }
}

// Attach this to each floor tile in your maze
public class FloorPhysicsManager : MonoBehaviour
{
    void Start()
    {
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        if (boxCollider != null)
        {
            boxCollider.material = CreateFloorPhysicsMaterial();
            boxCollider.contactOffset = 0.01f; // Tighter contact generation
        }

        // Make floor slightly thicker
        Vector3 currentScale = transform.localScale;
        currentScale.y = 0.2f; // Increase thickness
        transform.localScale = currentScale;
    }

    private PhysicMaterial CreateFloorPhysicsMaterial()
    {
        PhysicMaterial material = new PhysicMaterial("FloorPhysicsMaterial");
        material.bounciness = 0f;           // No bounce
        material.staticFriction = 0.6f;     // Good grip
        material.dynamicFriction = 0.6f;    // Good grip
        material.frictionCombine = PhysicMaterialCombine.Average;
        material.bounceCombine = PhysicMaterialCombine.Minimum;
        return material;
    }
}
