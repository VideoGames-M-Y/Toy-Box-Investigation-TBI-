using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // For Button

public class NextLevelManager : MonoBehaviour
{
    [SerializeField] private Button nextLevelButton; // Reference to the Next Level button
    [SerializeField] private string nextSceneName; // Name of the next scene

    void Start()
    {
        // Ensure the button is initially hidden
        nextLevelButton.gameObject.SetActive(false);

        // Add a listener to the button to load the next level when clicked
        nextLevelButton.onClick.AddListener(LoadNextLevel);
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
}
