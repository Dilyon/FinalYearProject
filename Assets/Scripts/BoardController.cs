using UnityEngine;

public class BoardController : MonoBehaviour
{
    public float rotationSpeed = 50f;
    public float maxRotation = 10f;
    private bool isDragging = false;
    private Vector3 lastMousePosition;

    void Update()
    {
        // Check for mouse input
        if (Input.GetMouseButtonDown(0)) // Left mouse button pressed
        {
            isDragging = true;
            lastMousePosition = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0)) // Left mouse button released
        {
            isDragging = false;
        }

        if (isDragging)
        {
            // Calculate mouse movement delta
            Vector3 deltaMouse = Input.mousePosition - lastMousePosition;

            // Get current rotation
            Vector3 currentRotation = transform.localEulerAngles;

            // Convert to -180 to 180 range
            if (currentRotation.x > 180f) currentRotation.x -= 360f;
            if (currentRotation.z > 180f) currentRotation.z -= 360f;

            // Calculate new rotation
            // Invert X and Y for more intuitive control
            float newRotationX = Mathf.Clamp(currentRotation.x + (-deltaMouse.y * rotationSpeed * Time.deltaTime), -maxRotation, maxRotation);
            float newRotationZ = Mathf.Clamp(currentRotation.z + (deltaMouse.x * rotationSpeed * Time.deltaTime), -maxRotation, maxRotation);

            // Apply new rotation
            transform.localEulerAngles = new Vector3(newRotationX, currentRotation.y, newRotationZ);

            // Update last mouse position
            lastMousePosition = Input.mousePosition;
        }
    }
}