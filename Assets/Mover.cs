using UnityEngine;

public class Mover : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f; // Speed of the character
    private Rigidbody2D rb; // Reference to the Rigidbody2D
    private Vector2 movement; // Stores movement input

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Get the Rigidbody2D component
    }

    void Update()
    {
        // Get input from arrow keys or WASD
        movement.x = Input.GetAxisRaw("Horizontal"); // Left/Right
        movement.y = Input.GetAxisRaw("Vertical");   // Up/Down
    }

    void FixedUpdate()
    {
        // Apply movement to the Rigidbody
        rb.linearVelocity = movement.normalized * moveSpeed;
    }
}
