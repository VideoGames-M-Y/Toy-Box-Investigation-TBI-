using UnityEngine;
using TMPro; // For TextMeshPro

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI instructionText; // UI text for instructions
    [SerializeField] private NextLevelManager nextLevelManager; // Reference to the NextLevelManager

    private bool hasMovedX = false;
    private bool hasMovedY = false;
    private bool hasGrabbed = false;
    private bool touchingSock = false;

    void Start()
    {
        instructionText.text = "Move the character left or right using the left or right arrow keys."; // Set the initial instruction
    }

    void Update()
    {
        // Check for movement
        if (!hasMovedX && (Input.GetAxisRaw("Horizontal") != 0))
        {
            hasMovedX = true;
        }
        if (hasMovedX && !hasMovedY)
        {
            instructionText.text = "Move the character up or down using the up or down arrow keys.";
            if (Input.GetAxisRaw("Vertical") != 0)
            {
                hasMovedY = true;
                instructionText.text = "Good! Now press Spacebar to grab objects.";
            }
        }

        // Check if the player is touching a BlueSock
        touchingSock = GameObject.FindWithTag("Player").GetComponent<SockCollector>().HasBlueSock();

        // Check for grabbing
        if (hasMovedX && hasMovedY && !hasGrabbed && Input.GetKeyDown(KeyCode.Space) && touchingSock)
        {
            hasGrabbed = true;
            instructionText.text = "Great! Tutorial complete!";
            TriggerNextLevel();
        }
    }

    private void TriggerNextLevel()
    {
        // Call NextLevelManager to show the Next Level button
        nextLevelManager.ShowNextLevelButton();
    }
}
