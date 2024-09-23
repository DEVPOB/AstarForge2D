using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    private Camera cam;
    public float moveSpeed = 10f; // Movement speed
    private Vector2 movement;
    private Rigidbody2D rb;
    float defaultCamSize;
    void Start()
    {
        // Get the Rigidbody2D component
        rb = GetComponent<Rigidbody2D>();
        // Set Rigidbody to kinematic to allow manual control over movement
        rb.isKinematic = false;
        cam = Camera.main;
        defaultCamSize = cam.orthographicSize;
    }

    // Update is called once per frame
    void Update()
    {
        // Get input
        float moveX = Input.GetAxisRaw("Horizontal"); // Get horizontal input (-1, 0, 1)
        float moveY = Input.GetAxisRaw("Vertical");   // Get vertical input (-1, 0, 1)

        // Prevent diagonal movement by prioritizing one axis
        if (moveX != 0) // If horizontal input exists, prioritize horizontal movement
        {
            moveY = 0;  // Ignore vertical input
        }
        
        
        movement = new Vector2(moveX, moveY).normalized; // Create movement vector
    }

    void FixedUpdate()
    {
        // Move the player by changing the Rigidbody2D's velocity
        rb.velocity = movement * moveSpeed;
    }
}
