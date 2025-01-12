// /*
//  * ////////// OLD CODE - RESTART THE SCENE ID GRABS THE WRONG ITEM //////////
//  *
//  * using UnityEngine;
//  * using UnityEngine.SceneManagement;
//  * using TMPro;
//  *
//  * public class SockCollector : MonoBehaviour
//  * {
//  *     [SerializeField] private TextMeshProUGUI socksCounterText; // UI text for the socks counter
//  *     [SerializeField] private int totalSocks = 6; // Total number of socks to collect
//  *     private int socksLeft; // Number of socks collected
//  *
//  *     private GameObject blueSockFollowingPlayer = null; // Tracks the BlueSock following the Player
//  *     private bool canInteract = false; // Prevents repeated pressing for socks
//  *     private Collider2D currentCollider; // The object the player is interacting with
//  *     [SerializeField] private NextLevelManager nextLevelManager; // Reference to the NextLevelManager
//  *
//  *     void Start()
//  *     {
//  *         if (socksCounterText == null)
//  *         {
//  *             Debug.LogError("socksCounterText is not assigned in the Inspector!");
//  *         }
//  *         if (nextLevelManager == null)
//  *         {
//  *             Debug.LogError("nextLevelManager is not assigned in the Inspector!");
//  *         }
//  *
//  *         socksLeft = totalSocks;
//  *
//  *         // Safely update the text only if the reference is valid
//  *         if (socksCounterText != null)
//  *         {
//  *             socksCounterText.text = $"Socks Left: {socksLeft}";
//  *         }
//  *     }
//  *
//  *     void Update()
//  *     {
//  *         // Handle interaction when spacebar is pressed
//  *         if (canInteract && Input.GetKeyDown(KeyCode.Space))
//  *         {
//  *             HandleInteraction();
//  *         }
//  *     }
//  *
//  *     private void OnTriggerEnter2D(Collider2D collision)
//  *     {
//  *         currentCollider = collision;
//  *
//  *         // Handle automatic delivery when touching Laundry
//  *         if (collision.CompareTag("Laundry") && blueSockFollowingPlayer != null)
//  *         {
//  *             DeliverBlueSock();
//  *         }
//  *         else
//  *         {
//  *             // Allow interaction for other objects
//  *             canInteract = true;
//  *         }
//  *
//  *         Debug.Log($"Entered trigger with {collision.tag}");
//  *     }
//  *
//  *     private void OnTriggerExit2D(Collider2D collision)
//  *     {
//  *         // Prevent interaction when exiting the trigger
//  *         if (collision == currentCollider)
//  *         {
//  *             canInteract = false;
//  *             currentCollider = null;
//  *         }
//  *
//  *         Debug.Log($"Exited trigger with {collision.tag}");
//  *     }
//  *
//  *     private void HandleInteraction()
//  *     {
//  *         // Check the tag of the current collider and handle accordingly
//  *         if (currentCollider.CompareTag("OtherSock"))
//  *         {
//  *             RestartScene();
//  *         }
//  *         else if (currentCollider.CompareTag("BlueShirts"))
//  *         {
//  *             RestartScene();
//  *         }
//  *         else if (currentCollider.CompareTag("PurplePants"))
//  *         {
//  *             RestartScene();
//  *         }
//  *         else if (currentCollider.CompareTag("BlueSock") && blueSockFollowingPlayer == null)
//  *         {
//  *             CollectBlueSock(currentCollider.gameObject);
//  *         }
//  *     }
//  *
//  *     private void RestartScene()
//  *     {
//  *         Debug.Log("Scene restarted.");
//  *         SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
//  *     }
//  *
//  *     private void CollectBlueSock(GameObject blueSock)
//  *     {
//  *         blueSockFollowingPlayer = blueSock;
//  *         Debug.Log($"{blueSock.name} is now following the Player.");
//  *     }
//  *
//  *     private void DeliverBlueSock()
//  *     {
//  *         Debug.Log($"{blueSockFollowingPlayer.name} delivered to Laundry.");
//  *         Destroy(blueSockFollowingPlayer);
//  *         blueSockFollowingPlayer = null; // Clear the reference
//  *
//  *         socksLeft--;
//  *         UpdateSocksCounter();
//  *
//  *         // Check if all socks are delivered
//  *         if (socksLeft <= 0)
//  *         {
//  *             Debug.Log("All socks delivered!");
//  *             LevelComplete();
//  *         }
//  *     }
//  *
//  *     private void UpdateSocksCounter()
//  *     {
//  *         // Update the socks counter UI text
//  *         socksCounterText.text = $"Socks Left: {socksLeft}";
//  *     }
//  *
//  *     private void FixedUpdate()
//  *     {
//  *         // Make the BlueSock follow the Player if it's assigned
//  *         if (blueSockFollowingPlayer != null)
//  *         {
//  *             blueSockFollowingPlayer.transform.position = Vector2.Lerp(
//  *                 blueSockFollowingPlayer.transform.position,
//  *                 transform.position,
//  *                 Time.deltaTime * 5f
//  *             );
//  *         }
//  *     }
//  *
//  *     public bool HasBlueSock()
//  *     {
//  *         return blueSockFollowingPlayer != null;
//  *     }
//  *
//  *     public bool HasSocksLeft()
//  *     {
//  *         return socksLeft > 0;
//  *     }
//  *
//  *     private void LevelComplete()
//  *     {
//  *         Debug.Log("Level completed!");
//  *
//  *         // Show the Next Level button
//  *         nextLevelManager.ShowNextLevelButton();
//  *     }
//  * }
//  */

// using UnityEngine;
// using UnityEngine.SceneManagement;
// using TMPro;

// public class SockCollector : MonoBehaviour
// {
//     [SerializeField] private TextMeshProUGUI socksCounterText; // UI text for the socks counter
//     [SerializeField] private int totalSocks = 6; // Total number of socks to collect
//     private int socksLeft; // Number of socks left to deliver

//     private GameObject currentItemFollowingPlayer = null; // Tracks the item following the Player
//     private Collider2D currentCollider; // The object the player is interacting with
//     [SerializeField] private NextLevelManager nextLevelManager; // Reference to the NextLevelManager

//     void Start()
//     {
//         if (socksCounterText == null)
//         {
//             Debug.LogError("socksCounterText is not assigned in the Inspector!");
//         }
//         if (nextLevelManager == null)
//         {
//             Debug.LogError("nextLevelManager is not assigned in the Inspector!");
//         }

//         socksLeft = totalSocks;

//         if (socksCounterText != null)
//         {
//             socksCounterText.text = $"גרביים שנשארו: {socksLeft}";
//         }
//     }

//     void Update()
//     {
//         if (currentItemFollowingPlayer == null && Input.GetKeyDown(KeyCode.Space))
//         {
//             HandleInteraction();
//         }
//         else if (currentItemFollowingPlayer != null && Input.GetKeyDown(KeyCode.Space))
//         {
//             DropItem();
//         }
//     }

//     private void OnTriggerEnter2D(Collider2D collision)
//     {
//         currentCollider = collision;

//         if (collision.CompareTag("Laundry") && currentItemFollowingPlayer != null)
//         {
//             DeliverItem();
//         }

//         Debug.Log($"Entered trigger with {collision.tag}");
//     }

//     private void OnTriggerExit2D(Collider2D collision)
//     {
//         if (collision == currentCollider)
//         {
//             currentCollider = null;
//         }

//         Debug.Log($"Exited trigger with {collision.tag}");
//     }

//     private void HandleInteraction()
//     {
//         if (currentCollider != null && !currentCollider.gameObject.CompareTag("Laundry"))
//         {
//             GrabItem(currentCollider.gameObject);
//         }
//     }

//     private void GrabItem(GameObject item)
//     {
//         currentItemFollowingPlayer = item;
//         Debug.Log($"{item.name} is now following the Player.");
//     }

//     public void DropItem()
//     {
//         Debug.Log($"{currentItemFollowingPlayer.name} dropped.");
//         currentItemFollowingPlayer = null;
//     }

//     private void DeliverItem()
//     {
//         Debug.Log($"{currentItemFollowingPlayer.name} delivered to Laundry.");

//         if (currentItemFollowingPlayer.CompareTag("BlueSock"))
//         {
//             Destroy(currentItemFollowingPlayer);
//             currentItemFollowingPlayer = null;

//             socksLeft--;
//             UpdateSocksCounter();

//             if (socksLeft <= 0)
//             {
//                 Debug.Log("All socks delivered!");
//                 LevelComplete();
//             }
//         }
//         else
//         {
//             RestartScene();
//         }
//     }

//     private void RestartScene()
//     {
//         Debug.Log("Scene restarted.");
//         SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
//     }

//     private void UpdateSocksCounter()
//     {
//         socksCounterText.text = $"גרביים שנשארו: {socksLeft}";
//     }

//     private void FixedUpdate()
//     {
//         if (currentItemFollowingPlayer != null)
//         {
//             currentItemFollowingPlayer.transform.position = Vector2.Lerp(
//                 currentItemFollowingPlayer.transform.position,
//                 transform.position,
//                 Time.deltaTime * 5f
//             );
//         }
//     }

//     private void LevelComplete()
//     {
//         Debug.Log("Level completed!");
//         nextLevelManager.ShowNextLevelButton();
//         nextLevelManager.ShowLevelCompleteText();
//     }

//     public bool HasSocksLeft()
//     {
//         return socksLeft > 0;
//     }

//     public bool HasBlueSock()
//     {
//         return currentItemFollowingPlayer != null && currentItemFollowingPlayer.CompareTag("BlueSock");
//     }

//     public bool IsHoldingItem()
//     {
//         return currentItemFollowingPlayer != null;
//     }
// }

using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class SockCollector : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI socksCounterText; // UI text for the socks counter
    [SerializeField] private int totalSocks = 6; // Total number of socks to collect
    [SerializeField] private GameObject wrongItemPopup; // UI Popup for wrong item
    [SerializeField] private Camera mainCamera; // Reference to the main camera
    [SerializeField] private float zoomDuration = 3f; // Duration of the zoom effect
    [SerializeField] private float zoomSize = 3f; // Camera size when zoomed in
    private int socksLeft;
    private GameObject currentItemFollowingPlayer = null; // Tracks the item following the Player
    private Collider2D currentCollider;
    [SerializeField] private NextLevelManager nextLevelManager; // Reference to the NextLevelManager
    [SerializeField] private float waitTime = 5f; // Time to wait before restarting the scene
    private Vector3 originalCameraPosition;
    private float originalCameraSize;

    void Start()
    {
        if (socksCounterText == null)
        {
            Debug.LogError("socksCounterText is not assigned in the Inspector!");
        }
        if (nextLevelManager == null)
        {
            Debug.LogError("nextLevelManager is not assigned in the Inspector!");
        }
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        socksLeft = totalSocks;
        if (socksCounterText != null)
        {
            socksCounterText.text = $"גרביים שנשארו: {socksLeft}";
        }

        // Hide the wrong item popup initially
        if (wrongItemPopup != null)
        {
            wrongItemPopup.SetActive(false);
        }

        // Save the original camera state
        originalCameraPosition = mainCamera.transform.position;
        originalCameraSize = mainCamera.orthographicSize;
    }

    void Update()
    {
        if (currentItemFollowingPlayer == null && Input.GetKeyDown(KeyCode.Space))
        {
            HandleInteraction();
        }
        else if (currentItemFollowingPlayer != null && Input.GetKeyDown(KeyCode.Space))
        {
            DropItem();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        currentCollider = collision;

        if (collision.CompareTag("Laundry") && currentItemFollowingPlayer != null)
        {
            DeliverItem();
        }

        Debug.Log($"Entered trigger with {collision.tag}");
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
        if (currentCollider != null && !currentCollider.gameObject.CompareTag("Laundry"))
        {
            GrabItem(currentCollider.gameObject);
        }
    }

    private void GrabItem(GameObject item)
    {
        currentItemFollowingPlayer = item;
        Debug.Log($"{item.name} is now following the Player.");
    }

    public void DropItem()
    {
        Debug.Log($"{currentItemFollowingPlayer.name} dropped.");
        currentItemFollowingPlayer = null;
    }

    private void DeliverItem()
    {
        Debug.Log($"{currentItemFollowingPlayer.name} delivered to Laundry.");

        if (currentItemFollowingPlayer.CompareTag("BlueSock"))
        {
            Destroy(currentItemFollowingPlayer);
            currentItemFollowingPlayer = null;

            socksLeft--;
            UpdateSocksCounter();

            if (socksLeft <= 0)
            {
                Debug.Log("All socks delivered!");
                LevelComplete();
            }
        }
        else
        {
            StartCoroutine(HandleWrongItem());
        }
    }

    private IEnumerator HandleWrongItem()
{
    // Make the player disappear (hide its renderer or deactivate)
    SpriteRenderer playerRenderer = GetComponent<SpriteRenderer>();
    if (playerRenderer != null)
    {
        playerRenderer.enabled = false; // Hides the player's sprite
    }

    // Target position and size for the camera
    Vector3 targetPosition = new Vector3(
        currentItemFollowingPlayer.transform.position.x,
        currentItemFollowingPlayer.transform.position.y,
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



    private void UpdateSocksCounter()
    {
        socksCounterText.text = $"גרביים שנשארו: {socksLeft}";
    }

    private void FixedUpdate()
    {
        if (currentItemFollowingPlayer != null)
        {
            currentItemFollowingPlayer.transform.position = Vector2.Lerp(
                currentItemFollowingPlayer.transform.position,
                transform.position,
                Time.deltaTime * 5f
            );
        }
    }

    private void LevelComplete()
    {
        Debug.Log("Level completed!");
        nextLevelManager.ShowNextLevelButton();
    }

    public bool HasSocksLeft()
    {
        return socksLeft > 0;
    }

    public bool HasBlueSock()
    {
        return currentItemFollowingPlayer != null && currentItemFollowingPlayer.CompareTag("BlueSock");
    }

    public bool IsHoldingItem()
    {
        return currentItemFollowingPlayer != null;
    }
}
