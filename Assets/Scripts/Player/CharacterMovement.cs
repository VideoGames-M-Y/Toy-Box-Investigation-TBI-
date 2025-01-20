// using UnityEngine;

// public class CharacterMovement : MonoBehaviour
// {
//     [SerializeField] float moveSpeed = 5f; // Speed of the character
//     [SerializeField] Animator animator; // Animator for handling animations

//     private Vector2 movement; // Stores the player's input

//     /*
//      * // void Update()
//      * // {
//      * //     // Get player input
//      * //     movement.x = Input.GetAxisRaw("Horizontal"); // A/D or Left/Right
//      * //     movement.y = Input.GetAxisRaw("Vertical");   // W/S or Up/Down
//      * //
//      * //     // Tell the Animator about the movement
//      * //     try
//      * //     {
//      * //         animator.SetFloat("MoveX", movement.x);
//      * //         animator.SetFloat("MoveY", movement.y);
//      * //         animator.SetBool("IsMoving", movement != Vector2.zero);
//      * //     }
//      * //     catch
//      * //     {
//      * //         Debug.Log("Animator not found");
//      * //     }
//      * //
//      * //     // Move the character
//      * //     transform.Translate(movement * moveSpeed * Time.deltaTime);
//      * // }
//      */

//     void Update()
//     {
//         // Get player input
//         float horizontal = Input.GetAxisRaw("Horizontal"); // A/D or Left/Right
//         float vertical = Input.GetAxisRaw("Vertical");     // W/S or Up/Down

//         // Prioritize horizontal movement if both inputs are active
//         if (horizontal != 0)
//         {
//             movement.x = horizontal;
//             movement.y = 0; // Disable vertical movement when horizontal is active
//         }
//         else if (vertical != 0)
//         {
//             movement.x = 0; // Disable horizontal movement when vertical is active
//             movement.y = vertical;
//         }
//         else
//         {
//             movement = Vector2.zero; // No movement
//         }

//         // Tell the Animator about the movement
//         try
//         {
//             animator.SetFloat("MoveX", movement.x);
//             animator.SetFloat("MoveY", movement.y);
//             animator.SetBool("IsMoving", movement != Vector2.zero);
//         }
//         catch
//         {
//             Debug.Log("Animator not found");
//         }

//         // Move the character
//         transform.Translate(movement * moveSpeed * Time.deltaTime);
//     }

//     public void SetMovement(Vector2 newMovement)
//     {
//         movement = newMovement;
//     }
// }

using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f; // Speed of the character
    [SerializeField] private Animator animator; // Animator for handling animations
    [SerializeField] private Rigidbody2D rb; // Reference to the Rigidbody2D

    private Vector2 movement; // Stores the player's input

    void Start()
    {
        // Ensure Rigidbody2D is assigned
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                Debug.LogError("Rigidbody2D component is missing on this GameObject!");
            }
        }
    }

    void Update()
    {
        // Get player input
        float horizontal = Input.GetAxisRaw("Horizontal"); // A/D or Left/Right
        float vertical = Input.GetAxisRaw("Vertical");     // W/S or Up/Down

        // Prioritize horizontal or vertical movement
        if (horizontal != 0 && vertical != 0)
        {
            // If both are pressed, prioritize horizontal
            vertical = 0;
        }

        movement.x = horizontal;
        movement.y = vertical;

        // Update Animator with movement values
        try
        {
            animator.SetFloat("MoveX", movement.x);
            animator.SetFloat("MoveY", movement.y);
            //animator.SetBool("IsMoving", rb.linearVelocity != Vector2.zero); // Check actual movement

            bool isMoving = rb.linearVelocity != Vector2.zero;

            if (isMoving)
            {
                animator.speed = 1; // Resume animation
            }
            else
            {
                animator.speed = 0; // Freeze animation
            }
        }
        catch
        {
            Debug.LogWarning("Animator not assigned or missing parameters.");
        }
    }

    void FixedUpdate()
    {
        // Move the character using Rigidbody2D
        rb.linearVelocity = movement * moveSpeed;
    }

    public void SetMovement(Vector2 newMovement)
    {
        movement = newMovement;
    }
}
