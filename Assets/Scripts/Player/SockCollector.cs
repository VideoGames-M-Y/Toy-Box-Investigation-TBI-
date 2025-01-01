using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class SockCollector : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI socksCounterText; // UI text for the socks counter

    [SerializeField]
    private int totalSocks = 6; // Total number of socks to collect

    private int socksLeft; // Number of socks collected
    private GameObject blueSockFollowingPlayer = null; // Tracks the BlueSock following the Player
    private bool canInteract = false; // Prevents repeated pressing for socks
    private Collider2D currentCollider; // The object the player is interacting with

    [SerializeField]
    private NextLevelManager nextLevelManager; // Reference to the NextLevelManager

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

        socksLeft = totalSocks;

        // Safely update the text only if the reference is valid
        if (socksCounterText != null)
        {
            socksCounterText.text = $"Socks Left: {socksLeft}";
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
        currentCollider = collision;

        // Handle automatic delivery when touching Laundry
        if (collision.CompareTag("Laundry") && blueSockFollowingPlayer != null)
        {
            DeliverBlueSock();
        }
        else
        {
            // Allow interaction for other objects
            canInteract = true;
        }

        Debug.Log($"Entered trigger with {collision.tag}");
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Prevent interaction when exiting the trigger
        if (collision == currentCollider)
        {
            canInteract = false;
            currentCollider = null;
        }

        Debug.Log($"Exited trigger with {collision.tag}");
    }

    private void HandleInteraction()
    {
        // Check the tag of the current collider and handle accordingly
        if (currentCollider.CompareTag("OtherSock"))
        {
            RestartScene();
        }
        else if (currentCollider.CompareTag("BlueSock") && blueSockFollowingPlayer == null)
        {
            CollectBlueSock(currentCollider.gameObject);
        }
    }

    private void RestartScene()
    {
        Debug.Log("Scene restarted.");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void CollectBlueSock(GameObject blueSock)
    {
        blueSockFollowingPlayer = blueSock;
        Debug.Log($"{blueSock.name} is now following the Player.");
    }

    private void DeliverBlueSock()
    {
        Debug.Log($"{blueSockFollowingPlayer.name} delivered to Laundry.");
        Destroy(blueSockFollowingPlayer);
        blueSockFollowingPlayer = null; // Clear the reference

        socksLeft--;
        UpdateSocksCounter();

        // Check if all socks are delivered
        if (socksLeft <= 0)
        {
            Debug.Log("All socks delivered!");
            LevelComplete();
        }
    }

    private void UpdateSocksCounter()
    {
        // Update the socks counter UI text
        socksCounterText.text = $"Socks Left: {socksLeft}";
    }

    private void FixedUpdate()
    {
        // Make the BlueSock follow the Player if it's assigned
        if (blueSockFollowingPlayer != null)
        {
            blueSockFollowingPlayer.transform.position = Vector2.Lerp(
                blueSockFollowingPlayer.transform.position,
                transform.position,
                Time.deltaTime * 5f
            );
        }
    }

    public bool HasBlueSock()
    {
        return blueSockFollowingPlayer != null;
    }

    public bool HasSocksLeft()
    {
        return socksLeft > 0;
    }

    private void LevelComplete()
    {
        Debug.Log("Level completed!");

        // Show the Next Level button
        nextLevelManager.ShowNextLevelButton();
    }
}