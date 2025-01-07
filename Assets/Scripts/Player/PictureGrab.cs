using UnityEngine;
using UnityEngine.SceneManagement;

public class PicturGrab : MonoBehaviour
{
    [SerializeField] private int totalPics = 5; // Total number of pictures to collect
    private int picsLeft; // Number of pictures left to place

    private GameObject picFollow = null; // Tracks the picture following the Player
    private bool canInteract = false; // Prevents repeated pressing for pictures
    private Collider2D currentCollider; // The object the player is interacting with
    [SerializeField] private NextLevelManager nextLevelManager; // Reference to the NextLevelManager

    void Start()
    {
        if (nextLevelManager == null)
        {
            Debug.LogError("nextLevelManager is not assigned in the Inspector!");
        }

        picsLeft = totalPics;
    }

    void Update()
    {
        // Handle interaction when spacebar is pressed
        if (canInteract && Input.GetKeyDown(KeyCode.Space))
        {
            HandleInteraction();
        }
    }

    // private void OnTriggerEnter2D(Collider2D collision)
    // {
    //     currentCollider = collision;
    //     canInteract = true;

    //     Debug.Log($"Entered trigger with {collision.name}");
    // }

    private void OnTriggerEnter2D(Collider2D collision)
{
    // Update the currentCollider only if it's null or prioritize frames over other colliders
    if (currentCollider == null || collision.CompareTag("Frame"))
    {
        currentCollider = collision;
        canInteract = true;
    }

    Debug.Log($"Entered trigger with {collision.name}");
}


    // private void OnTriggerExit2D(Collider2D collision)
    // {
    //     // Prevent interaction when exiting the trigger
    //     if (collision == currentCollider)
    //     {
    //         canInteract = false;
    //         currentCollider = null;
    //     }

    //     Debug.Log($"Exited trigger with {collision.tag}");
    // }

    private void OnTriggerExit2D(Collider2D collision)
{
    // Only clear currentCollider if the exiting collider is the active one
    if (collision == currentCollider)
    {
        currentCollider = null;
        canInteract = false;
    }

    Debug.Log($"Exited trigger with {collision.name}");
}


    private void HandleInteraction()
    {
        if (picFollow == null && currentCollider != null && currentCollider.CompareTag("Picture"))
        {
            // Collect the picture
            collectPic(currentCollider.gameObject);
        }
        else if (picFollow != null && currentCollider != null && currentCollider.CompareTag("Frame"))
        {
            // Attempt to place the picture
            DeliverPic();
        }
        else if (picFollow != null)
        {
            // Drop the picture if not near a valid frame
            dropPic();
        }
    }

   private void dropPic()
    {
        if (picFollow == null)
        {
            Debug.Log("No picture to drop.");
            return;
        }

        if (currentCollider == null || !currentCollider.CompareTag("Frame"))
        {
            // Drop the picture if not near a valid frame
            Debug.Log($"{picFollow.name} dropped.");
            picFollow = null; // Clear the reference
        }
        else
        {
            Debug.Log("Cannot drop picture while near a valid frame.");
        }
    }

    private void RestartScene()
    {
        Debug.Log("Scene restarted.");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void collectPic(GameObject pic)
    {
        picFollow = pic;
        Debug.Log($"{pic.name} is now following the Player.");
    }

    private void DeliverPic()
    {
        if (picFollow == null)
        {
            Debug.Log("No picture to deliver.");
            return;
        }

        // Check if the currentCollider is a valid frame
        if (currentCollider != null && currentCollider.CompareTag("Frame"))
        {
            if (IsMatch(picFollow, currentCollider.gameObject))
            {
                Debug.Log($"{picFollow.name} correctly placed in {currentCollider.name}.");

                // Place the picture at the frame's position
                picFollow.transform.position = currentCollider.transform.position;

                // Clear the reference
                picFollow = null;
                picsLeft--;

                // Check if all pictures are placed
                if (picsLeft <= 0)
                {
                    Debug.Log("All pictures placed correctly!");
                    LevelComplete();
                }
            }
            else
            {
                Debug.LogError($"{picFollow.name} does not match {currentCollider.name}. Try again.");
                RestartScene();
            }
        }
        else
        {
            Debug.LogError("Not a valid frame.");
        }
    }

    private bool IsMatch(GameObject picture, GameObject frame)
    {
        // Compare names (e.g., "P1" and "P1 Frame") or use a custom logic
        Debug.Log($"Comparing {picture.name} with {frame.name.Replace("Frame ", "")}");
        return picture.name == frame.name.Replace("Frame ", "");
    }

    private void FixedUpdate()
    {
        // Make the picture follow the Player if it's assigned
        if (picFollow != null)
        {
            picFollow.transform.position = Vector2.Lerp(
                picFollow.transform.position,
                transform.position,
                Time.deltaTime * 5f
            );
        }
    }

    private void LevelComplete()
    {
        Debug.Log("Level completed!");

        // Show the Next Level button
        nextLevelManager.ShowNextLevelButton();
        nextLevelManager.ShowLevelCompleteText();
    }
}
