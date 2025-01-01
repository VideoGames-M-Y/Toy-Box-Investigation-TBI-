using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // Speed of the character

    [SerializeField]
    private Animator animator; // Animator for handling animations

    private Vector2 movement; // Stores the player's input

    void Update()
    {
        // Get player input
        movement.x = Input.GetAxisRaw("Horizontal"); // A/D or Left/Right
        movement.y = Input.GetAxisRaw("Vertical");   // W/S or Up/Down

        // Tell the Animator about the movement
        try
        {
            animator.SetFloat("MoveX", movement.x);
            animator.SetFloat("MoveY", movement.y);
            animator.SetBool("IsMoving", movement != Vector2.zero);
        }
        catch
        {
            Debug.Log("Animator not found");
        }

        // Move the character
        transform.Translate(movement * moveSpeed * Time.deltaTime);
    }
}