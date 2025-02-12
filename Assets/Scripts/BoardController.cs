using UnityEngine;

public class BoardController : MonoBehaviour
{
    public float rotationSpeed = 50f;
    public float maxRotation = 10f;

    // Physics settings
    public float physicsUpdateRate = 50f;
    private float physicsTimeStep;
    private Vector3 targetRotation;
    private bool isDragging = false;
    private Vector3 lastMousePosition;

    private Rigidbody rb;

    void Start()
    {
        // Get or add Rigidbody
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        // Configure Rigidbody for stable physics
        rb.useGravity = false;
        rb.isKinematic = true;  // Changed to kinematic
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.mass = 100f;  // High mass for stability

        // Configure physics timestep
        physicsTimeStep = 1f / physicsUpdateRate;
        Time.fixedDeltaTime = physicsTimeStep;
        Physics.defaultSolverIterations = 8;  // Increase solver iterations
        Physics.defaultSolverVelocityIterations = 2;

        targetRotation = transform.localEulerAngles;
    }

    void Update()
    {
        HandleInput();
    }

    void FixedUpdate()
    {
        ApplyPhysicsRotation();
    }

    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            lastMousePosition = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        if (isDragging)
        {
            Vector3 deltaMouse = Input.mousePosition - lastMousePosition;

            Vector3 currentRotation = targetRotation;
            if (currentRotation.x > 180f) currentRotation.x -= 360f;
            if (currentRotation.z > 180f) currentRotation.z -= 360f;

            float rotationMultiplier = 0.5f; // Reduced rotation speed multiplier
            targetRotation = new Vector3(
                Mathf.Clamp(currentRotation.x + (-deltaMouse.y * rotationSpeed * Time.deltaTime * rotationMultiplier), -maxRotation, maxRotation),
                currentRotation.y,
                Mathf.Clamp(currentRotation.z + (deltaMouse.x * rotationSpeed * Time.deltaTime * rotationMultiplier), -maxRotation, maxRotation)
            );

            lastMousePosition = Input.mousePosition;
        }
    }

    private void ApplyPhysicsRotation()
    {
        Quaternion targetQuaternion = Quaternion.Euler(targetRotation);
        rb.MoveRotation(Quaternion.Lerp(rb.rotation, targetQuaternion, physicsTimeStep * physicsUpdateRate));
    }
}