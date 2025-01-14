using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationMap : MonoBehaviour
{
    // Degrees per second
    [SerializeField] private float rotationSpeed = 180f;
    // Rotation amount in degrees
    [SerializeField] private float targetRotation = 90f;
    // Reference to the target object to rotate around
    [SerializeField] private Transform rotationTarget; 

    private bool isRotating = false; //Tracks if rotation is in progress
    private float currentRotation = 0f; //Tracks current rotation in progress
    private Vector3 rotationCenter; //Point which rotation occurs
    private Quaternion startRotation; //Initial and final rotation state
    private Quaternion endRotation; //Initial and final object positions
    private Vector3 startPosition;
    private Vector3 endPosition;

    private void Start()
    {
        // If no target is assigned, use the object's center
        if (rotationTarget == null)
        {
            rotationCenter = transform.position;
        }
    }

    private void Update()
    {
        // Check for mouse click
        if (Input.GetMouseButtonDown(0) && !isRotating)
        {
            Vector2 mousePosition = Input.mousePosition;
            bool clickedLeftSide = mousePosition.x < Screen.width / 2f;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit) && hit.transform == this.transform)
            {
                StartRotation(clickedLeftSide);
            }
        }

        // Handle rotation
        if (isRotating)
        {
            currentRotation += rotationSpeed * Time.deltaTime;

            if (currentRotation >= targetRotation)
            {
                // Rotation complete
                transform.rotation = endRotation;
                transform.position = endPosition;
                isRotating = false;
                currentRotation = 0f;
            }
            else
            {
                // Smooth rotation and position
                float t = currentRotation / targetRotation;
                transform.rotation = Quaternion.Lerp(startRotation, endRotation, t);
                transform.position = Vector3.Lerp(startPosition, endPosition, t);
            }
        }
    }

    private void StartRotation(bool rotateLeft)
    {
        isRotating = true;
        currentRotation = 0f;

        // Store initial state
        startRotation = transform.rotation;
        startPosition = transform.position;

        // Get rotation center from target if available
        rotationCenter = rotationTarget != null ? rotationTarget.position : transform.position;

        // Calculate rotation angle
        float rotationAmount = rotateLeft ? targetRotation : -targetRotation;

        // Calculate end rotation
        endRotation = startRotation * Quaternion.Euler(0f, 0f, rotationAmount);

        // Calculate end position by rotating the current position around the target
        Vector3 directionToObject = transform.position - rotationCenter;
        endPosition = rotationCenter + (Quaternion.Euler(0f, 0f, rotationAmount) * directionToObject);
    }

    private void OnGUI()
    {
        Color leftColor = new Color(1f, 0f, 0f, 0.1f);
        Color rightColor = new Color(0f, 0f, 1f, 0.1f);

        GUI.color = leftColor;
        GUI.Box(new Rect(0, 0, Screen.width / 2f, Screen.height), "Rotate Left");

        GUI.color = rightColor;
        GUI.Box(new Rect(Screen.width / 2f, 0, Screen.width / 2f, Screen.height), "Rotate Right");
    }
}