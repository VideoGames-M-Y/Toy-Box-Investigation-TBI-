using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PicturGrab : MonoBehaviour
{
    [SerializeField] private int totalPics = 5; // Total number of pictures to collect
    private int picsLeft; // Number of pictures left to place

    private GameObject picFollow = null; // Tracks the picture following the Player
    private bool canInteract = false; // Prevents repeated pressing for pictures
    private Collider2D currentCollider; // The object the player is interacting with
    [SerializeField] private NextLevelManager nextLevelManager; // Reference to the NextLevelManager
    [SerializeField] private Camera mainCamera; // Reference to the main Camera
    [SerializeField] private float zoomDuration = 3f; // Duration of the camera zoom effect 
    [SerializeField] private float zoomSize = 3f; // Camera size when zoomed in
    [SerializeField] private GameObject wrongItemPopup; // UI Popup for wrong item
    [SerializeField] private float waitTime = 3f; // Time to wait before restarting the scene

    void Start()
    {
        if (nextLevelManager == null)
        {
            Debug.LogError("nextLevelManager is not assigned in the Inspector!");
        }

        picsLeft = totalPics;

        // Hide the wrong item popup initially
        if (wrongItemPopup != null)
        {
            wrongItemPopup.SetActive(false);
        }
    }

    void Update()
    {
        // Handle interaction when spacebar is pressed
        if (canInteract && Input.GetKeyDown(KeyCode.Space))
        {
            HandleInteraction();
        }
    }

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
                StartCoroutine(wrongMatch());
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

     private IEnumerator wrongMatch()
{
    // Make the player disappear (hide its renderer or deactivate)
    SpriteRenderer playerRenderer = GetComponent<SpriteRenderer>();
    if (playerRenderer != null)
    {
        playerRenderer.enabled = false; // Hides the player's sprite
    }

    // Target position and size for the camera
    Vector3 targetPosition = new Vector3(
        picFollow.transform.position.x,
        picFollow.transform.position.y,
        mainCamera.transform.position.z // Keep the Z position unchanged
    );

    float initialSize = mainCamera.orthographicSize;
    float elapsedTime = 0f;

    // Show the wrong item popup
    if (wrongItemPopup != null)
    {
        wrongItemPopup.SetActive(true);
    }

    // Gradually zoom and center the camera
    while (elapsedTime < zoomDuration)
    {
        // Smoothly move the camera toward the target position
        mainCamera.transform.position = Vector3.Lerp(
            mainCamera.transform.position,
            targetPosition,
            elapsedTime / zoomDuration
        );

        // Smoothly adjust the orthographic size to zoom in
        mainCamera.orthographicSize = Mathf.Lerp(initialSize, zoomSize, elapsedTime / zoomDuration);

        elapsedTime += Time.deltaTime;
        yield return null;
    }

    // Ensure the camera is exactly at the final position and size
    mainCamera.transform.position = targetPosition;
    mainCamera.orthographicSize = zoomSize;

    // Pause the scene
    Time.timeScale = 0f;

    // Wait for the specified time before restarting
    yield return new WaitForSecondsRealtime(waitTime);

    // Reset the scene
    Time.timeScale = 1f; // Resume time
    if (playerRenderer != null)
    {
        playerRenderer.enabled = true; // Restore the player's sprite before the scene reloads
    }
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
}
}
