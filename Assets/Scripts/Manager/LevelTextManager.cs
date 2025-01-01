using UnityEngine;
using TMPro; // For TextMeshPro

public class LevelTextManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelText; // Reference to the Level 1 text
    [SerializeField] private TextMeshProUGUI LevelCompleteText; // Reference to the Level 2 text
    private bool textHidden = false; // Tracks whether the text is already hidden

    void Start()
    {
        levelText.gameObject.SetActive(true); // Ensure the text is visible at the start
        LevelCompleteText.gameObject.SetActive(false); // Ensure the Level 2 text is hidden
    }

    void Update()
    {
        // Check for player movement
        if (!textHidden && (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0))
        {
            HideLevelText(); // Hide the text when the player starts moving
        }
        if (!GameObject.FindWithTag("Player").GetComponent<SockCollector>().HasSocksLeft()){
            LevelCompleteText.gameObject.SetActive(true); // Show the Level 2 text
        }
    }

    private void HideLevelText()
    {
        levelText.gameObject.SetActive(false); // Hide the text
        textHidden = true; // Prevent the text from being hidden multiple times
    }
}
