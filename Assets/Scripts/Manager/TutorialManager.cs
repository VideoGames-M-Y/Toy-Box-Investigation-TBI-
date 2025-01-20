using UnityEngine;
using TMPro; // For TextMeshPro
using UnityEngine.UI; // For UI elements like Button
using System.Collections; // For IEnumerator
using UnityEngine.SceneManagement; // For SceneManager

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI instructionText; // UI text for instructions
    [SerializeField] private NextLevelManager nextLevelManager; // Reference to the NextLevelManager
    [SerializeField] private Button hintButton; // Reference to the Hint button
    [SerializeField] private Button restartButton; // Reference to the Restart button
    [SerializeField] private Button homeButton; // Reference to the Home button
    [SerializeField] private Button nextLevelButton; // Reference to the Next Level button
    [SerializeField] private GameObject arrow; // Reference to the Hint panel
    [SerializeField] private Vector3 arrowOffset = new Vector3(0, -50, 0); // Offset for the Arrow position


    private bool hasMovedX = false;
    private bool hasMovedY = false;
    private bool hasGrabbed = false;
    private bool hasDropped = false;
    private bool isHintTutorial = false;
    private bool isRestartTutorial = false;
    private bool isHomeTutorial = false;


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

        // Set up the buttons
        restartButton.onClick.AddListener(OnRestartButtonClicked);
        homeButton.onClick.AddListener(OnHomeButtonClicked);
        hintButton.onClick.AddListener(OnHintButtonClicked);
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
            // Disable the Player movement
            sockCollector.GetComponent<CharacterMovement>().enabled = false;
            sockCollector.GetComponent<Mover>().SetSpeed(0);
            sockCollector.GetComponent<Animator>().enabled = false;
            StartButtonTutorial();
        }
    }

    private void StartButtonTutorial()
    {
        StartCoroutine(ButtonTutorialCoroutine());
    }

    private IEnumerator ButtonTutorialCoroutine()
    {
        // Step 1: Hint button tutorial
        isHintTutorial = true;
        instructionText.text = "כפתור הרמז יציג את הוראות השלב לאחר שנעלמו. \n לחץ עליו כדי להמשיך";
        SetArrowPosition(hintButton);

        yield return new WaitUntil(() => isHintTutorial == false);

        // Step 2: Restart button tutorial
        isRestartTutorial = true;
        instructionText.text = "כפתור ההתחלה מחדש  יתחיל את השלב ממהתחלה. \n לחץ עליו כדי להמשיך.";
        SetArrowPosition(restartButton);

        yield return new WaitUntil(() => isRestartTutorial == false);

        // Step 3: Home button tutorial
        isHomeTutorial = true;
        instructionText.text = "כפתור הבית יחזיר אותך לתפריט הראשי. \n לחץ עליו כדי להמשיך";
        SetArrowPosition(homeButton);

        yield return new WaitUntil(() => isHomeTutorial == false);

        // Tutorial complete
        arrow.SetActive(false);
        instructionText.text = "מעולה! \n המדריך בוצע בהצלחה! \n לחץ על הכפתור למטה כדי להמשיך לשלב הראשון";
        nextLevelManager.ShowNextLevelButton();
        SetArrowPosition(nextLevelButton);
    }


    public void OnHintButtonClicked()
    {
        if (isHintTutorial)
        {
            isHintTutorial = false;
        }
        else
        {
            Debug.Log("Hint button clicked outside tutorial.");
        }
    }

    public void OnRestartButtonClicked()
    {
        if (isRestartTutorial)
        {
            isRestartTutorial = false;
        }
        else
        {
            RestartScene();
        }
    }

    public void OnHomeButtonClicked()
    {
        if (isHomeTutorial)
        {
            isHomeTutorial = false;
        }
        else
        {
            ReturnToHome();
        }
    }

    private void RestartScene()
    {
         SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void ReturnToHome()
    {
        SceneManager.LoadScene("OpeningScene");
    }

    private void SetArrowPosition(Button targetButton)
    {
        arrow.SetActive(true);
        arrow.transform.position = targetButton.transform.position + arrowOffset;
    }

}