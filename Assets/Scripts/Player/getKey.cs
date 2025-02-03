using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class getKey : MonoBehaviour
{
    [SerializeField] private int totalKeys = 2; // Total number of socks to collect
    [SerializeField] private GameObject wrongItemPopup; // UI Popup for wrong item
    [SerializeField] private GameObject RoadNotSafePopUp;
    [SerializeField] private Camera mainCamera; // Reference to the main camera
    [SerializeField] private float zoomDuration = 3f; // Duration of the zoom effect
    [SerializeField] private float zoomSize = 3f; // Camera size when zoomed in
    private int KeysLeft;
    // private GameObject currentItemFollowingPlayer = null; // Tracks the item following the Player
    private List<GameObject> KeysFollow = new List<GameObject>(); 
    private Collider2D currentCollider;
    [SerializeField] private NextLevelManager nextLevelManager; // Reference to the NextLevelManager
    [SerializeField] private float waitTime = 2f; // Time to wait before restarting the scene
    private Vector3 originalCameraPosition;
    private float originalCameraSize;
    [SerializeField] private float keyOffset = 0.5f;
    private bool canInteract = false;

    void Start()
    {
        KeysLeft = totalKeys;
    
        if (nextLevelManager == null)
        {
            Debug.LogError("nextLevelManager is not assigned in the Inspector!");
        }
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        // Hide the wrong item popup initially
        if (wrongItemPopup != null)
        {
            wrongItemPopup.SetActive(false);
        }

        if (RoadNotSafePopUp != null)
        {
            RoadNotSafePopUp.SetActive(false);
        }

        // Save the original camera state
        originalCameraPosition = mainCamera.transform.position;
        originalCameraSize = mainCamera.orthographicSize;
    }

    void Update()
    {
        if (canInteract && Input.GetKeyDown(KeyCode.Space))
        {
            if (currentCollider != null && !currentCollider.gameObject.CompareTag("Lock"))
            {
                HandleInteraction();
            }
            else if (KeysFollow.Count > 0) // Allow dropping even with one key
            {
                DropItem();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        currentCollider = collision;
        canInteract = true;

        if (collision.CompareTag("Lock") && KeysFollow.Count > 0)
        {
            DeliverItem();
        }

        if (collision.CompareTag("road"))
        {
            ZoomInOnRoad();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision == currentCollider)
        {
            currentCollider = null;
            
        }

        Debug.Log($"Exited trigger with {collision.tag}");
    }

    private void HandleInteraction()
    {
        if (currentCollider != null && !currentCollider.gameObject.CompareTag("Lock"))
        {
            GrabItem(currentCollider.gameObject);
        }
    }

    private void GrabItem(GameObject item)
    {
        if (!KeysFollow.Contains(item) && KeysFollow.Count < totalKeys) 
        {
            KeysFollow.Add(item);
            Debug.Log($"{item.name} is now following the Player.");
        }
    }

    public void DropItem()
    {
        if (KeysFollow.Count > 0)
        {
            GameObject keyToDrop = KeysFollow[KeysFollow.Count - 1]; // Remove the last collected key
            KeysFollow.RemoveAt(KeysFollow.Count - 1);

            keyToDrop.transform.position = transform.position + Vector3.down; // Drop slightly below the player
            keyToDrop.SetActive(true); // Ensure it's active in the scene

            Debug.Log($"Dropped {keyToDrop.name}. Remaining keys: {KeysFollow.Count}");
        }
    }

    private void DeliverItem()
    {
        if(KeysFollow.Count == totalKeys){
            Debug.Log($"Delivering {KeysFollow.Count} keys.");

            List<GameObject> keysToRemove = new List<GameObject>();
            GameObject badKey = null;

            foreach (GameObject key in KeysFollow)
            {
                if (key.CompareTag("correctKey"))
                {
                    Destroy(key);
                    keysToRemove.Add(key);
                    KeysLeft--;
                }
                else
                {
                badKey = key; 
                }
            }

            foreach (GameObject key in keysToRemove)
            {
                KeysFollow.Remove(key);
            }

            if (KeysLeft <= 0)
            {
                Debug.Log("All keys delivered!");
                GameObject[] lockObjects = GameObject.FindGameObjectsWithTag("Lock");
                foreach (GameObject lockObject in lockObjects)
                {
                    Destroy(lockObject);
                }
                LevelComplete();
                return;
            }

            ZoomInOnKey(badKey);
        }
    }

    private void ZoomInOnKey(GameObject currentItemZoom)
    {
        StopPlayerMovement();
        StartCoroutine(HandleWrongItem(currentItemZoom));
    }

    private IEnumerator HandleWrongItem(GameObject currentItemZoom)
    {

        KeysFollow.Clear();
        // Make the player disappear (hide its renderer or deactivate)
        SpriteRenderer playerRenderer = GetComponent<SpriteRenderer>();
        if (playerRenderer != null)
        {
            playerRenderer.enabled = false; // Hides the player's sprite
        }

        // Target position and size for the camera
        Vector3 targetPosition = new Vector3(
            currentItemZoom.transform.position.x,
            currentItemZoom.transform.position.y,
            mainCamera.transform.position.z // Keep the Z position unchanged
        );

        float initialSize = mainCamera.orthographicSize;
        float elapsedTime = 0f;
        float newZoomDuration = zoomDuration * 0.7f; // Zoom duration is 70% of the original

        // Show the wrong item popup
        if (wrongItemPopup != null)
        {
            wrongItemPopup.SetActive(true);
        }

        // Gradually zoom and center the camera
        while (elapsedTime < newZoomDuration)
        {
            mainCamera.transform.position = Vector3.Lerp(
                mainCamera.transform.position,
                targetPosition,
                elapsedTime / newZoomDuration
            );

            mainCamera.orthographicSize = Mathf.Lerp(initialSize, zoomSize, elapsedTime / newZoomDuration);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the camera is exactly at the final position and size
        mainCamera.transform.position = targetPosition;
        mainCamera.orthographicSize = zoomSize;

        // Briefly pause before restarting (reduce wait time)
        yield return new WaitForSecondsRealtime(waitTime); 

        // Reset the scene
        if (playerRenderer != null)
        {
            playerRenderer.enabled = true; // Restore the player's sprite before the scene reloads
        }
        mainCamera.transform.position = originalCameraPosition;
        mainCamera.orthographicSize = originalCameraSize;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void FixedUpdate()
    {
        PositionKeys();
    }

    private void PositionKeys()
    {
        Vector2 basePosition = transform.position;

        for (int i = 0; i < KeysFollow.Count; i++)
        {
            if (KeysFollow[i] != null)
            {
                Vector2 offset = new Vector2((i % 2 == 0 ? -1 : 1) * (i / 2 + 1) * keyOffset, 0);
                KeysFollow[i].transform.position = Vector2.Lerp(KeysFollow[i].transform.position, basePosition + offset, Time.deltaTime * 5f);
            }
        }
    }

    private void LevelComplete()
    {
        Debug.Log("Level completed!");
        nextLevelManager.ShowLevelCompleteText();
        nextLevelManager.ShowNextLevelButton();
        StopPlayerMovement();
    }

    public bool HasKeysLeft()
    {
        return KeysLeft > 0;
    }

    public bool HasCorrectKey()
    {
        return KeysFollow.Exists(key => key.CompareTag("correctKey"));
    }

    public bool IsHoldingItem()
    {
        return KeysFollow != null;
    }

    private void ZoomInOnRoad()
    {
        StopPlayerMovement();
        StartCoroutine(ZoomInOnRoadCoroutine());
    }

    private IEnumerator ZoomInOnRoadCoroutine()
    {
        KeysFollow = null;

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
