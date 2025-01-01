using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [SerializeField] Animator animator; // Reference to the Animator component
    private Vector2 lastDirection; // Stores the last movement direction

    void Update()
    {
        // Get movement input from the Rigidbody (or Mover)
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        // Update the Animator parameters
        animator.SetFloat("MoveX", moveX);
        animator.SetFloat("MoveY", moveY);

        // Set IsMoving true/false
        animator.SetBool("IsMoving", moveX != 0 || moveY != 0);

        // Save the last direction if moving
        if (moveX != 0 || moveY != 0)
        {
            lastDirection = new Vector2(moveX, moveY);
        }

        // Pass the last idle direction to the Animator
        animator.SetFloat("LastMoveX", lastDirection.x);
        animator.SetFloat("LastMoveY", lastDirection.y);
    }
}
