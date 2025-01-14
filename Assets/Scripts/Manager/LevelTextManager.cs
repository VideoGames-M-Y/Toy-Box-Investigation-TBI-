using UnityEngine;
using TMPro;
using UnityEngine.UI; // For TextMeshPro

public class LevelTextManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelText; // Reference to the Level 1 text
    private bool textHidden = false; // Tracks whether the text is already hidden
    [SerializeField] private Button hintButton; // Reference to the Hint button
    [SerializeField] private GameObject Panel; // Reference to the Hint panel

    void Start()
    {
        levelText.gameObject.SetActive(true); // Ensure the text is visible at the start
        Panel.SetActive(true); // Ensure the panel is hidden at the start
        hintButton.onClick.AddListener(ShowLevelText); // Add a listener to the button to hide the text
    }

    void Update()
    {
        // Check for player movement
        if (!textHidden && (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0))
        {
            HideLevelText(); // Hide the text when the player starts moving
        }
    }

    private void HideLevelText()
    {
        levelText.gameObject.SetActive(false); // Hide the text
        Panel.SetActive(false); // Show the panel
        textHidden = true; // Prevent the text from being hidden multiple times
    }

    private void ShowLevelText()
    {
        levelText.gameObject.SetActive(true); // Show the text
        Panel.SetActive(true); // Hide the panel
        textHidden = false; // Allow the text to be hidden again
    }
}
