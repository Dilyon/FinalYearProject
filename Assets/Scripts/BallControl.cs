using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallControl : MonoBehaviour
{
    public float speed = 1.0f;
    private Rigidbody rb;

    public Transform respawnPoint;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // Increase the collision detection precision
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    // Update is called once per frame
    //void Update()
    //{
    //float moveX = Input.GetAxis("Horizontal");
    //float moveZ = Input.GetAxis("Vertical");

    //Vector3 force = new Vector3(moveX, 0.0f, moveZ);

    //rb.AddForce(force * speed);
    //}

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger Enter:" + other.gameObject.tag);

        if (other.gameObject.tag == "FinishZone")
        {
            rb.isKinematic = true;
            transform.position = respawnPoint.position;
            rb.isKinematic = false;
        }
    }
}
