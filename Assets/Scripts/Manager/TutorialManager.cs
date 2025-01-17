using UnityEngine;
using TMPro; // For TextMeshPro
using UnityEngine.UI; // For UI elements like Button

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI instructionText; // UI text for instructions
    [SerializeField] private NextLevelManager nextLevelManager; // Reference to the NextLevelManager
    [SerializeField] private Button hintButton; // Reference to the Hint button
    [SerializeField] private Button RestartButton; // Reference to the Restart button
    [SerializeField] private Button HomeButton; // Reference to the Home button


    private bool hasMovedX = false;
    private bool hasMovedY = false;
    private bool hasGrabbed = false;
    private bool hasDropped = false;
    private bool hintClicked = false;
    private bool restartClicked = false;
    private bool homeClicked = false;

    private SockCollector sockCollector; // Reference to the SockCollector component

    void Start()
    {
        // Set the initial instruction
        instructionText.text = "הזז את הילד ימינה ושמאלה בעזרת מקשי החצים במקלדת";

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
            instructionText.text = "מעולה! \n עכשיו הזז את הילד למעלה ולמטה בעזרת מקשי החצים במקלדת";
        }

        // Step 2: Move vertically
        if (hasMovedX && !hasMovedY && Input.GetAxisRaw("Vertical") != 0)
        {
            hasMovedY = true;
            instructionText.text = "יפה מאוד! \n עכשיו אסוף את הגרב בעזרת לחיצה על מקש הרווח כשהילד נמצא ליד הגרב";
        }

        // Step 3: Grab an item
        if (hasMovedY && !hasGrabbed && sockCollector.IsHoldingItem())
        {
            hasGrabbed = true;
            instructionText.text = "נהדר! \n לחץ על מקש הרווח פעם נוספת כדי להפיל את הגרב";
        }

        // Step 4: Deliver the item
        if (hasGrabbed && !hasDropped && !sockCollector.IsHoldingItem())
        {
            hasDropped = true;
            instructionText.text = "עבודה טובה! \n כעת תוכל להמשיך לשלב הבא על ידי לחיצה על הכפתור למטה בעזרת העכבר";
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