using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI instructionsText;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] GameObject[] shapes;

    private int score = 0;
    private int shapesClicked = 0;
    [SerializeField] float timeRemaining = 30f;
    private bool isGameOver = false;

    [SerializeField] string sceneName;

    void Start()
    {
        instructionsText.text = "Click the red circles!";
        UpdateScore();
    }

    void Update()
    {
        if (!isGameOver)
        {
            timeRemaining -= Time.deltaTime;

            if (timeRemaining <= 0f)
            {
                timeRemaining = 0f;
                isGameOver = true;
                instructionsText.text = "Good job! Final Score: " + score;
                Invoke("GoToNextLevel", 2f); // Go to next level after a delay
            }

            UpdateTimer();
        }
    }

    public void OnShapeClicked(bool isCorrect)
    {
        if (isGameOver) return; // If the game is over, don't process clicks

        if (isCorrect)
        {
            score += 1; // Correct shape clicked -> +1 point
        }
        else
        {
            score -= 1; // Incorrect shape clicked -> -1 point
            instructionsText.text = "Incorrect! Restarting level..."; // Provide feedback
            Invoke("RestartLevel", 1f); // Restart level after a 1-second delay
            return;
        }

        shapesClicked++; // Increment the number of shapes clicked

        UpdateScore(); // Update the score display
    }

    void UpdateScore()
    {
        scoreText.text = "Score: " + score; // Update the score UI
    }

    void UpdateTimer()
    {
        timerText.text = "Time: " + Mathf.Ceil(timeRemaining).ToString(); // Update timer UI
    }

    void GoToNextLevel()
    {
        SceneManager.LoadScene(sceneName); // Load next level
    }

    void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reload the current scene
    }
}
