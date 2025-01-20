using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using UnityEngine.UIElements;

public class BoxToDoorManager : MonoBehaviour
{
    private bool isCompleted = false;
    private GameObject currentItemFollowingPlayer = null; // Tracks the item following the Player
    private Collider2D currentCollider;
    [SerializeField] private NextLevelManager nextLevelManager; // Reference to the NextLevelManager
    [SerializeField] private GameObject Doorway; // Reference to the Doorway

    void Start()
    {
        if (nextLevelManager == null)
        {
            Debug.LogError("nextLevelManager is not assigned in the Inspector!");
        }
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

        if (collision.CompareTag("Doorway") && currentItemFollowingPlayer.CompareTag("CorrectItem"))
        {
            OpenDoor();
        }

        if (collision.CompareTag("Finish"))
        {
            LevelComplete();
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
        if (currentCollider != null && !currentCollider.gameObject.CompareTag("Doorway"))
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
        Debug.Log($"{currentItemFollowingPlayer.name} delivered to DoorWay.");

        if (currentItemFollowingPlayer.CompareTag("CorrectItem"))
        {
            OpenDoor();
        }
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
        nextLevelManager.ShowLevelCompleteText();
        nextLevelManager.ShowNextLevelButton();
        //make the character disappear
        gameObject.SetActive(false);
        //make the following item disappear
        currentItemFollowingPlayer.SetActive(false);
    }

    public bool IsHoldingItem()
    {
        return currentItemFollowingPlayer != null;
    }

    private void OpenDoor()
    {
        if (!isCompleted)
        {
            isCompleted = true;
            Doorway.GetComponent<BoxCollider2D>().enabled = false;
        }
    }
}