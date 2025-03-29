using UnityEngine;

/// <summary>
/// Controls the rotation of a game board based on mouse input
/// </summary>
public class BoardController : MonoBehaviour
{
    // Rotation settings
    public float rotationSpeed = 50f;    // Base speed for rotation
    public float maxRotation = 10f;      // Maximum angle the board can be tilted

    // Physics settings
    public float physicsUpdateRate = 50f; // How many physics updates per second
    private float physicsTimeStep;        // The time between physics updates

    // Rotation speed limit
    public float maxRotationSpeed = 30f;  // Maximum rotation speed in degrees per second

    // State variables
    private Vector3 targetRotation;       // The desired rotation angles
    private bool isDragging = false;      // Tracks if the player is currently dragging
    private Vector3 lastMousePosition;    // Last frame's mouse position for calculating delta
    private Rigidbody rb;                 // Reference to the Rigidbody component
    private Vector3 previousRotation;     // Last frame's rotation for calculating rotation speed
    private float rotationDelta;          // Amount of rotation change

    /// <summary>
    /// Initializes the board controller and physics settings
    /// </summary>
    void Start()
    {
        // Get or add Rigidbody
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        // Configure Rigidbody for stable physics
        rb.useGravity = false;                                   // Disable gravity since we're manually controlling rotation
        rb.isKinematic = true;                                   // Make kinematic to prevent external physics forces
        rb.interpolation = RigidbodyInterpolation.Interpolate;   // Smooth movement between physics updates
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic; // More accurate collision detection
        rb.mass = 100f;                                          // Heavier mass for stability

        // Configure physics timestep
        physicsTimeStep = 1f / physicsUpdateRate;                // Calculate time step from update rate
        Time.fixedDeltaTime = physicsTimeStep;                   // Set Unity's fixed update interval
        Physics.defaultSolverIterations = 12;                    // Increased solver iterations for better accuracy
        Physics.defaultSolverVelocityIterations = 4;             // Increased velocity iterations for more stable physics

        // Initialize rotation tracking
        targetRotation = transform.localEulerAngles;             // Start with current rotation
        previousRotation = targetRotation;                       // Initialize previous rotation
    }

    /// <summary>
    /// Processes input every frame
    /// </summary>
    void Update()
    {
        HandleInput();  // Process mouse input
    }

    /// <summary>
    /// Applies physics at fixed intervals
    /// </summary>
    void FixedUpdate()
    {
        ApplyPhysicsRotation();  // Apply rotation changes to the physics system
    }

    /// <summary>
    /// Processes mouse input to control board rotation
    /// </summary>
    private void HandleInput()
    {
        // Start dragging when left mouse button is pressed
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            lastMousePosition = Input.mousePosition;
        }
        // Stop dragging when left mouse button is released
        else if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        // Process mouse movement while dragging
        if (isDragging)
        {
            // Calculate mouse movement since last frame
            Vector3 deltaMouse = Input.mousePosition - lastMousePosition;

            // Get current rotation and normalize to -180 to 180 range
            Vector3 currentRotation = targetRotation;
            if (currentRotation.x > 180f) currentRotation.x -= 360f;
            if (currentRotation.z > 180f) currentRotation.z -= 360f;

            // Store previous rotation for speed calculation
            previousRotation = targetRotation;

            // Calculate new target rotation with reduced multiplier for better control
            float rotationMultiplier = 0.3f;  // Reduced for better control
            Vector3 newRotation = new Vector3(
                // X rotation (pitch) based on vertical mouse movement (clamped to max tilt)
                Mathf.Clamp(currentRotation.x + (-deltaMouse.y * rotationSpeed * Time.deltaTime * rotationMultiplier), -maxRotation, maxRotation),
                // Y rotation (unchanged - we're not rotating around Y axis)
                currentRotation.y,
                // Z rotation (roll) based on horizontal mouse movement (clamped to max tilt)
                Mathf.Clamp(currentRotation.z + (deltaMouse.x * rotationSpeed * Time.deltaTime * rotationMultiplier), -maxRotation, maxRotation)
            );

            // Calculate and limit rotation speed
            Vector3 rotationChange = newRotation - previousRotation;
            float rotationMagnitude = rotationChange.magnitude;

            // If rotation is faster than allowed maximum
            if (rotationMagnitude > maxRotationSpeed * Time.deltaTime)
            {
                // Scale down the rotation change to match max speed
                rotationChange = rotationChange.normalized * (maxRotationSpeed * Time.deltaTime);
                newRotation = previousRotation + rotationChange;
            }

            // Set the target rotation
            targetRotation = newRotation;

            // Update mouse position for next frame
            lastMousePosition = Input.mousePosition;
        }
    }

    /// <summary>
    /// Applies the calculated rotation to the rigidbody using physics
    /// </summary>
    private void ApplyPhysicsRotation()
    {
        // Convert Euler angles to Quaternion
        Quaternion targetQuaternion = Quaternion.Euler(targetRotation);

        // Limit the maximum rotation change per physics step
        float maxStepRotation = maxRotationSpeed * physicsTimeStep;

        // Smoothly rotate towards target rotation with speed limit
        Quaternion nextRotation = Quaternion.RotateTowards(rb.rotation, targetQuaternion, maxStepRotation);

        // Apply the rotation to the rigidbody
        rb.MoveRotation(nextRotation);
    }
}