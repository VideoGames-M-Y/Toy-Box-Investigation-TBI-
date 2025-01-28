using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class AvoidPit : MonoBehaviour
{
    [SerializeField] private int totalWood = 5; // Total wood pieces required to cover pits
    private int woodLeft; // Number of wood pieces left to place
    private GameObject woodFollow = null; // The wood piece following the Player
    private bool canInteract = false; // Prevent repeated interactions
    private Collider2D currentCollider; // The object the player is interacting with
    [SerializeField] private NextLevelManager nextLevelManager; // Reference to the NextLevelManager
    [SerializeField] private Camera mainCamera; // Reference to the main Camera
    [SerializeField] private float zoomDuration = 2f; // Duration of the camera zoom effect
    [SerializeField] private float zoomSize = 3f; // Camera size when zoomed in
    [SerializeField] private GameObject wrongItemPopup; // UI Popup for wrong item
    [SerializeField] private GameObject RoadNotSafePopUp; 
    [SerializeField] private float waitTime = 2f; // Time to wait before restarting the scene
    private Vector3 originalCameraPosition;
    private float originalCameraSize;
    private bool isPitCovered = false;

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

        woodLeft = totalWood;

        // Hide the wrong item popup initially
        if (wrongItemPopup != null)
        {
            wrongItemPopup.SetActive(false);
        }
        
        if (RoadNotSafePopUp != null)
        {
            RoadNotSafePopUp.SetActive(false);
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
        // Update the currentCollider only if it's null or prioritize pits/wood
        if (currentCollider == null || collision.CompareTag("pit") || collision.CompareTag("wood"))
        {
            currentCollider = collision;
            canInteract = true;
        }

        Debug.Log($"Entered trigger with {collision.name}");

        if(collision.CompareTag("Frame") && !isPitCovered){
            HandlePitFall(collision.transform.position);
        }
        
        if(collision.CompareTag("bike"))
        {
            LevelComplete();
        }

        if(collision.CompareTag("road")){
            ZoomInOnRoad();
        }
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
        if (woodFollow == null && currentCollider != null && currentCollider.CompareTag("wood"))
        {
            // Collect the wood piece
            CollectWood(currentCollider.gameObject);
        }
        else if (woodFollow != null && currentCollider != null && currentCollider.CompareTag("pit"))
        {
            // Attempt to place the wood
            FillPit();
        }
        else if (woodFollow != null)
        {
            // Drop the wood if not near a valid pit
            DropWood();
        }
    }

    private void DropWood()
    {
        if (woodFollow == null)
        {
            Debug.Log("No wood to drop.");
            return;
        }

        if (currentCollider == null || !currentCollider.CompareTag("pit"))
        {
            Debug.Log($"{woodFollow.name} dropped.");
            woodFollow = null;
        }
        else
        {
            Debug.Log("Cannot drop wood while near a valid pit.");
        }
    }

    private void CollectWood(GameObject wood)
    {
        woodFollow = wood;
        Debug.Log($"{wood.name} is now following the Player.");
    }

    private void FillPit()
    {
        if (woodFollow == null)
        {
            Debug.Log("No wood to fill the pit.");
            return;
        }

        if (currentCollider != null && currentCollider.CompareTag("pit"))
        {
            Debug.Log($"{woodFollow.name} placed in {currentCollider.name}.");
            woodFollow.transform.position = currentCollider.transform.position;
            woodFollow = null;
            woodLeft--;

            currentCollider.enabled = false;
            isPitCovered = true;

        }
        else
        {
            Debug.LogError("Not a valid pit.");
        }
    }

    private void FixedUpdate()
    {
        if (woodFollow != null)
        {
            woodFollow.transform.position = Vector2.Lerp(
                woodFollow.transform.position,
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

    public void HandlePitFall(Vector3 pitPosition)
    {
        StartCoroutine(ZoomInOnPit(pitPosition));
    }

    private IEnumerator ZoomInOnPit(Vector3 pitPosition)
    {
        woodFollow = null;

        SpriteRenderer playerRenderer = GetComponent<SpriteRenderer>();
        if (playerRenderer != null)
        {
            playerRenderer.enabled = false;
        }
        // Adjust pit position for camera
        Vector3 targetPosition = new Vector3(
            pitPosition.x,
            pitPosition.y,
            mainCamera.transform.position.z
        );

        float elapsedTime = 0f;

        // Show wrong action popup, if assigned
        if (wrongItemPopup != null)
        {
            wrongItemPopup.SetActive(true);
        }

        // Zoom in on the pit
        while (elapsedTime < zoomDuration)
        {
            mainCamera.transform.position = Vector3.Lerp(
                mainCamera.transform.position,
                targetPosition,
                elapsedTime / zoomDuration
            );

            mainCamera.orthographicSize = Mathf.Lerp(originalCameraSize, zoomSize, elapsedTime / zoomDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure camera is fully zoomed in
        mainCamera.transform.position = targetPosition;
        mainCamera.orthographicSize = zoomSize;

        // Pause briefly before restarting
        Time.timeScale = 0f; // Pause the game
        yield return new WaitForSecondsRealtime(waitTime);
        Time.timeScale = 1f; // Resume the game

        if (playerRenderer != null)
        {
            playerRenderer.enabled = true;
        }

        // Restore camera settings
        mainCamera.transform.position = originalCameraPosition;
        mainCamera.orthographicSize = originalCameraSize;

        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void ZoomInOnRoad()
    {
        StartCoroutine(ZoomInOnRoadCoroutine());
    }

    private IEnumerator ZoomInOnRoadCoroutine()
    {
        woodFollow = null;

        SpriteRenderer playerRenderer = GetComponent<SpriteRenderer>();
        if (playerRenderer != null)
        {
            playerRenderer.enabled = false;
        }
        
        // Get the player's position to zoom in on
        Vector3 playerPosition = transform.position; // Get the player's current position
        Vector3 targetPosition = new Vector3(playerPosition.x, playerPosition.y, mainCamera.transform.position.z);

        float elapsedTime = 0f;

        if (RoadNotSafePopUp!= null)
        {
            RoadNotSafePopUp.SetActive(true);
        }

        // Zoom in on the player's position
        while (elapsedTime < zoomDuration)
        {
            mainCamera.transform.position = Vector3.Lerp(
                mainCamera.transform.position,
                targetPosition,
                elapsedTime / zoomDuration
            );

            mainCamera.orthographicSize = Mathf.Lerp(originalCameraSize, zoomSize, elapsedTime / zoomDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure camera is fully zoomed in
        mainCamera.transform.position = targetPosition;
        mainCamera.orthographicSize = zoomSize;

        // Optionally, wait a brief moment before zooming back out (optional)
        yield return new WaitForSeconds(waitTime);

        if (playerRenderer != null)
        {
            playerRenderer.enabled = true;
        }

        // Restore camera settings (optional, or you could zoom out at a different time)
        mainCamera.transform.position = originalCameraPosition;
        mainCamera.orthographicSize = originalCameraSize;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}