using UnityEngine;
using TMPro; // For TextMeshPro

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI instructionText; // UI text for instructions
    [SerializeField] private NextLevelManager nextLevelManager; // Reference to the NextLevelManager

    private bool hasMovedX = false;
    private bool hasMovedY = false;
    private bool hasGrabbed = false;
    private bool hasDropped = false;

    private SockCollector sockCollector; // Reference to the SockCollector component

    void Start()
    {
        // Set the initial instruction
        instructionText.text = "Move the character left or right using the left or right arrow keys.";

        // Cache the SockCollector component for better performance
        sockCollector = GameObject.FindWithTag("Player")?.GetComponent<SockCollector>();
        if (sockCollector == null)
        {
            Debug.LogError("Player object is missing the SockCollector component!");
        }
    }

    void Update()
    {
        // Step 1: Move horizontally
        if (!hasMovedX && Input.GetAxisRaw("Horizontal") != 0)
        {
            hasMovedX = true;
            instructionText.text = "Move the character up or down using the up or down arrow keys.";
        }

        // Step 2: Move vertically
        if (hasMovedX && !hasMovedY && Input.GetAxisRaw("Vertical") != 0)
        {
            hasMovedY = true;
            instructionText.text = "Now find an a piece of clothing and press Spacebar to grab it.";
        }

        // Step 3: Grab an item
        if (hasMovedY && !hasGrabbed && sockCollector.IsHoldingItem())
        {
            hasGrabbed = true;
            instructionText.text = "Now press Spacebar to drop the item.";
        }

        // Step 4: Deliver the item
        if (hasGrabbed && !hasDropped && !sockCollector.IsHoldingItem())
        {
            hasDropped = true;
            instructionText.text = "Good job! All tasks completed.";
            nextLevelManager.ShowNextLevelButton();
        }
    }

    private void TriggerNextLevel()
    {
        if (nextLevelManager != null)
        {
            // Call NextLevelManager to show the Next Level button
            nextLevelManager.ShowNextLevelButton();
        }
        else
        {
            Debug.LogError("NextLevelManager reference is missing! Ensure it's assigned in the inspector.");
        }
    }
}