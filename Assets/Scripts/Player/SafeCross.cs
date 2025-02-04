using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SafeCross : MonoBehaviour
{
    private Collider2D currentCollider; // The object the player is interacting with
    [SerializeField] private NextLevelManager nextLevelManager; // Reference to the NextLevelManager
    [SerializeField] private Camera mainCamera; // Reference to the main Camera
    [SerializeField] private float zoomDuration = 2f; // Duration of the camera zoom effect
    [SerializeField] private float zoomSize = 3f; // Camera size when zoomed in
    [SerializeField] private GameObject CrossedOnRedPopUp; // UI Popup for wrong item
    [SerializeField] private GameObject RoadNotSafePopUp;
    [SerializeField] private float waitTime = 2f; // Time to wait before restarting the scene
    [SerializeField] private GameObject Arrow;
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

        // Hide the wrong item popup initially
        if (CrossedOnRedPopUp != null)
        {
            CrossedOnRedPopUp.SetActive(false);
        }
        if (RoadNotSafePopUp != null)
        {
            RoadNotSafePopUp.SetActive(false);
        }

        originalCameraPosition = mainCamera.transform.position;
        originalCameraSize = mainCamera.orthographicSize;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        currentCollider = collision;

        Debug.Log($"Entered trigger with {collision.name}");

        if (collision.CompareTag("bike"))
        {
            LevelComplete();
        }

        if (collision.CompareTag("road"))
        {
            HandleFail(collision.transform.position, "road");
        }

        if (collision.CompareTag("Redlight"))
        {
            HandleFail(collision.transform.position, "Redlight");
        }
    }

    private void LevelComplete()
    {
        Debug.Log("Level completed!");
        nextLevelManager.ShowNextLevelButton();
        nextLevelManager.ShowLevelCompleteText();
        StopPlayerMovement();
    }

    public void HandleFail(Vector3 pitPosition, string failType)
    {
        StopPlayerMovement();
        StartCoroutine(ZoomInOnPit(pitPosition, failType));
    }

    private IEnumerator ZoomInOnPit(Vector3 pitPosition, string failType)
    {
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
        if (failType == "Redlight")
        {
            if (CrossedOnRedPopUp != null)
            {
                CrossedOnRedPopUp.SetActive(true);
            }
        }

        else if (failType == "road")
        {
            if (RoadNotSafePopUp != null)
            {
                RoadNotSafePopUp.SetActive(true);
            }
        }

        Arrow.SetActive(false);

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
        yield return new WaitForSecondsRealtime(waitTime);

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
        StopPlayerMovement();
        StartCoroutine(ZoomInOnRoadCoroutine());
    }

    private IEnumerator ZoomInOnRoadCoroutine()
    {
        SpriteRenderer playerRenderer = GetComponent<SpriteRenderer>();
        if (playerRenderer != null)
        {
            playerRenderer.enabled = false;
        }
        // Get the player's position to zoom in on
        Vector3 playerPosition = transform.position;
        Vector3 targetPosition = new Vector3(playerPosition.x, playerPosition.y, mainCamera.transform.position.z);

        float elapsedTime = 0f;

        if (RoadNotSafePopUp != null)
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

    private void StopPlayerMovement()
    {
        // Disable movement scripts
        CharacterMovement characterMovement = GetComponent<CharacterMovement>();
        if (characterMovement != null)
        {
            characterMovement.enabled = false;
        }

        Mover mover = GetComponent<Mover>();
        if (mover != null)
        {
            mover.enabled = false;
        }

        // Stop Rigidbody movement
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.bodyType = RigidbodyType2D.Kinematic; // Fix for deprecated isKinematic
        }

        // Stop animations
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.speed = 0; // Freeze animation
            animator.SetBool("IsMoving", false); // Ensure any movement state is reset
        }
    }
}