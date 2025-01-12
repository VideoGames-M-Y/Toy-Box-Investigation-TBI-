using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // For Button
using TMPro; // For TextMeshPro

public class NextLevelManager : MonoBehaviour
{
    [SerializeField] private Button nextLevelButton; // Reference to the Next Level button
    [SerializeField] private string nextSceneName; // Name of the next scene
    [SerializeField] private TextMeshProUGUI LevelCompleteText; // Reference to the Level 2 text
    [SerializeField] private Button homeButton; // Reference to the Home button


    void Start()
    {
        // Ensure the button is initially hidden
        nextLevelButton.gameObject.SetActive(false);

        LevelCompleteText.gameObject.SetActive(false); // Ensure the Level 2 text is hidden

        // Add a listener to the button to load the next level when clicked
        nextLevelButton.onClick.AddListener(LoadNextLevel);
        homeButton.onClick.AddListener(LoadHomeScene);
    }

    public void ShowNextLevelButton()
    {
        // Make the Next Level button visible
        nextLevelButton.gameObject.SetActive(true);
    }

    private void LoadNextLevel()
    {
        // Load the specified next scene
        SceneManager.LoadScene(nextSceneName);
    }

    public void ShowLevelCompleteText()
    {
        LevelCompleteText.gameObject.SetActive(true); // Show the Level 2 text
    }

    private void LoadHomeScene()
    {
        // Load the Home scene
        SceneManager.LoadScene("OpeningScene");
    }
}
