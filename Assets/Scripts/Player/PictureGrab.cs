using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PictureGrab : MonoBehaviour
{
    [SerializeField] private int totalPics = 5; // Total number of pictures to collect
    private int picsLeft; // Number of pictures left to place

    private GameObject picFollow = null; // Tracks the picture following the Player
    private bool canInteract = false; // Prevents repeated pressing for pictures
    private Collider2D currentCollider; // The object the player is interacting with
    [SerializeField] private NextLevelManager nextLevelManager; // Reference to the NextLevelManager
    [SerializeField] private Camera mainCamera; // Reference to the main Camera
    [SerializeField] private float zoomDuration = 2f; // Duration of the camera zoom effect
    [SerializeField] private float zoomSize = 3f; // Camera size when zoomed in
    [SerializeField] private GameObject wrongItemPopup; // UI Popup for wrong item
    [SerializeField] private float waitTime = 2f; // Time to wait before restarting the scene
    private Vector3 originalCameraPosition;
    private float originalCameraSize;

    void Start()
    {
        if (nextLevelManager == null)
        {
            Debug.LogError("nextLevelManager is not assigned in the Inspector!");
        }
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        picsLeft = totalPics;

        // Hide the wrong item popup initially
        if (wrongItemPopup != null)
        {
            wrongItemPopup.SetActive(false);
        }
        
        originalCameraPosition = mainCamera.transform.position;
        originalCameraSize = mainCamera.orthographicSize;
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
        if (currentCollider == null || (!currentCollider.CompareTag("Frame") && collision.CompareTag("Frame")))
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
            CollectPic(currentCollider.gameObject);
        }
        else if (picFollow != null && currentCollider != null && currentCollider.CompareTag("Frame"))
        {
            // Attempt to place the picture
            DeliverPic();
        }
        else if (picFollow != null)
        {
            // Drop the picture if not near a valid frame
            DropPic();
        }
    }

    private void DropPic()
    {
        if (picFollow == null)
        {
            Debug.Log("No picture to drop.");
            return;
        }

        if (currentCollider == null || !currentCollider.CompareTag("Frame"))
        {
            Debug.Log($"{picFollow.name} dropped.");
            picFollow = null;
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

    private void CollectPic(GameObject pic)
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

        if (currentCollider != null && currentCollider.CompareTag("Frame"))
        {
            if (IsMatch(picFollow, currentCollider.gameObject))
            {
                Debug.Log($"{picFollow.name} correctly placed in {currentCollider.name}.");
                picFollow.transform.position = currentCollider.transform.position;
                picFollow = null;
                picsLeft--;

                currentCollider.enabled = false;

                if (picsLeft <= 0)
                {
                    Debug.Log("All pictures placed correctly!");
                    LevelComplete();
                }
            }
            else
            {
                Debug.LogError($"{picFollow.name} does not match {currentCollider.name}. Try again.");
                StartCoroutine(WrongMatch());
            }
        }
        else
        {
            Debug.LogError("Not a valid frame.");
        }
    }

    private bool IsMatch(GameObject picture, GameObject frame)
    {
        Debug.Log($"Comparing {picture.name} with {frame.name.Replace("Frame ", "")}");
        return picture.name == frame.name.Replace("Frame ", "");
    }

    private void FixedUpdate()
    {
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
        nextLevelManager.ShowNextLevelButton();
        nextLevelManager.ShowLevelCompleteText();
    }

    private IEnumerator WrongMatch()
    {
        GameObject picZoom = picFollow;
        picFollow = null;
        
        SpriteRenderer playerRenderer = GetComponent<SpriteRenderer>();
        if (playerRenderer != null)
        {
            playerRenderer.enabled = false;
        }

        Vector3 targetPosition = new Vector3(
            picZoom.transform.position.x,
            picZoom.transform.position.y,
            mainCamera.transform.position.z
        );

        float initialSize = mainCamera.orthographicSize;
        float elapsedTime = 0f;

        if (wrongItemPopup != null)
        {
            wrongItemPopup.SetActive(true);
        }

        while (elapsedTime < zoomDuration)
        {
            mainCamera.transform.position = Vector3.Lerp(
                mainCamera.transform.position,
                targetPosition,
                elapsedTime / zoomDuration
            );

            mainCamera.orthographicSize = Mathf.Lerp(initialSize, zoomSize, elapsedTime / zoomDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        mainCamera.transform.position = targetPosition;
        mainCamera.orthographicSize = zoomSize;

        yield return new WaitForSecondsRealtime(waitTime);

        if (playerRenderer != null)
        {
            playerRenderer.enabled = true;
        }
        mainCamera.transform.position = originalCameraPosition;
        mainCamera.orthographicSize = originalCameraSize;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}