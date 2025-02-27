using UnityEngine;

public class BoardController : MonoBehaviour
{
    public float rotationSpeed = 50f;
    public float maxRotation = 10f;

    // Physics settings
    public float physicsUpdateRate = 50f;
    private float physicsTimeStep;

    // Add max rotation speed limit
    public float maxRotationSpeed = 30f;

    private Vector3 targetRotation;
    private bool isDragging = false;
    private Vector3 lastMousePosition;
    private Rigidbody rb;
    private Vector3 previousRotation;
    private float rotationDelta;

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
        rb.isKinematic = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.mass = 100f;

        // Configure physics timestep
        physicsTimeStep = 1f / physicsUpdateRate;
        Time.fixedDeltaTime = physicsTimeStep;
        Physics.defaultSolverIterations = 12;  // Increased solver iterations for better accuracy
        Physics.defaultSolverVelocityIterations = 4;  // Increased velocity iterations

        targetRotation = transform.localEulerAngles;
        previousRotation = targetRotation;
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

            // Store previous rotation for speed calculation
            previousRotation = targetRotation;

            // Calculate new target rotation with reduced multiplier
            float rotationMultiplier = 0.3f;  // Further reduced for better control

            Vector3 newRotation = new Vector3(
                Mathf.Clamp(currentRotation.x + (-deltaMouse.y * rotationSpeed * Time.deltaTime * rotationMultiplier), -maxRotation, maxRotation),
                currentRotation.y,
                Mathf.Clamp(currentRotation.z + (deltaMouse.x * rotationSpeed * Time.deltaTime * rotationMultiplier), -maxRotation, maxRotation)
            );

            // Calculate and limit rotation speed
            Vector3 rotationChange = newRotation - previousRotation;
            float rotationMagnitude = rotationChange.magnitude;

            if (rotationMagnitude > maxRotationSpeed * Time.deltaTime)
            {
                // Scale down the rotation change to match max speed
                rotationChange = rotationChange.normalized * (maxRotationSpeed * Time.deltaTime);
                newRotation = previousRotation + rotationChange;
            }

            targetRotation = newRotation;
            lastMousePosition = Input.mousePosition;
        }
    }

    private void ApplyPhysicsRotation()
    {
        Quaternion targetQuaternion = Quaternion.Euler(targetRotation);

        // Limit the maximum rotation change per physics step
        float maxStepRotation = maxRotationSpeed * physicsTimeStep;
        Quaternion nextRotation = Quaternion.RotateTowards(rb.rotation, targetQuaternion, maxStepRotation);

        rb.MoveRotation(nextRotation);
    }
}