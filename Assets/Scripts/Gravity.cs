using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SphereCollider))]
public class Gravity : MonoBehaviour
{
    private Rigidbody rb;
    private SphereCollider sphereCollider;

    [Header("Physics Settings")]
    public float gravityMultiplier = 3.0f; // Increase this to make gravity stronger

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        sphereCollider = GetComponent<SphereCollider>();

        // Create and configure PhysicMaterial with absolutely no bounce
        PhysicMaterial physicsMaterial = new PhysicMaterial("DeadBall");
        physicsMaterial.bounciness = 0f;
        physicsMaterial.dynamicFriction = 1f;      // Maximum friction
        physicsMaterial.staticFriction = 1f;       // Maximum friction
        physicsMaterial.frictionCombine = PhysicMaterialCombine.Maximum;
        physicsMaterial.bounceCombine = PhysicMaterialCombine.Minimum;

        // Apply the material to the collider
        sphereCollider.material = physicsMaterial;
        sphereCollider.isTrigger = false;

        // Configure Rigidbody
        rb.useGravity = true;
        rb.mass = 2f;                   // Increased mass
        rb.drag = 0f;                   // No air resistance
        rb.angularDrag = 0.05f;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.constraints = RigidbodyConstraints.None;
    }

    void FixedUpdate()
    {
        // Apply extra gravity force
        Vector3 extraGravity = (Physics.gravity * gravityMultiplier) - Physics.gravity;
        rb.AddForce(extraGravity, ForceMode.Acceleration);
    }
}