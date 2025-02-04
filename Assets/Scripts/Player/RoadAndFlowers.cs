using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class RoadAndFlowers : MonoBehaviour
{
    [SerializeField] private int totalFlowers = 2;
    private int FlowersLeft;
    private GameObject[] flowersFollow = new GameObject[2]; // Two slots
    private bool canInteract = false;
    private Collider2D currentCollider;
    [SerializeField] private NextLevelManager nextLevelManager;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float zoomDuration = 2f;
    [SerializeField] private float zoomSize = 3f;
    [SerializeField] private GameObject wrongFlowersPopup;
    [SerializeField] private GameObject wrongRoadPopUp;
    [SerializeField] private GameObject wrongBothPopUp;
    [SerializeField] private float waitTime = 2f;
    [SerializeField] private float flowerOffset = 1.5f;
    private Vector3 originalCameraPosition;
    private float originalCameraSize;

    void Start()
    {
        if (mainCamera == null) mainCamera = Camera.main;
        FlowersLeft = totalFlowers;
        if (wrongFlowersPopup) wrongFlowersPopup.SetActive(false);
        if (wrongRoadPopUp) wrongRoadPopUp.SetActive(false);
        if (wrongBothPopUp) wrongBothPopUp.SetActive(false);

        originalCameraPosition = mainCamera.transform.position;
        originalCameraSize = mainCamera.orthographicSize;
    }

    void Update()
    {
        if (canInteract && Input.GetKeyDown(KeyCode.Space))
        {
            HandleInteraction();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        currentCollider = collision;
        canInteract = true;
        Debug.Log($"Entered trigger with {collision.name}");

        if (collision.CompareTag("Barrier"))
        {
            if (CheckFlowers())
            {
                HandlePitFall(collision.transform.position, "road");
            }
            else
            {
                HandlePitFall(collision.transform.position, "both");
            }
        }
        else if (collision.CompareTag("bike"))
        {
            if (CheckFlowers())
            {
                LevelComplete();
            }
            else
            {
                HandlePitFall(collision.transform.position, "flowers");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision == currentCollider)
        {
            currentCollider = null;
            canInteract = false;
        }
    }

    private void HandleInteraction()
    {
        if (HasFlowerSlot() && currentCollider != null && (currentCollider.CompareTag("OtherFlower") || currentCollider.CompareTag("OrangeFlower")) && currentCollider.gameObject != flowersFollow[0] && currentCollider.gameObject != flowersFollow[1])
        {
            CollectFlower(currentCollider.gameObject);
            Debug.Log("Collecting flower: " + currentCollider.name);
        }
        else if (HasFlower())
        {
            Debug.Log("Attempting to drop a flower.");
            DropFlower();
        }

        Debug.Log("HandleInteraction called. Current flower state: [0] " + (flowersFollow[0] ? flowersFollow[0].name : "null") + ", [1] " + (flowersFollow[1] ? flowersFollow[1].name : "null"));
    }



    private void FixedUpdate()
    {
        PositionFlowers();
    }

    private void PositionFlowers()
    {
        Vector2 basePosition = transform.position;
        if (flowersFollow[0] != null)
        {
            flowersFollow[0].transform.position = Vector2.Lerp(flowersFollow[0].transform.position, basePosition + Vector2.left * flowerOffset, Time.deltaTime * 5f);
        }
        if (flowersFollow[1] != null)
        {
            flowersFollow[1].transform.position = Vector2.Lerp(flowersFollow[1].transform.position, basePosition + Vector2.right * flowerOffset, Time.deltaTime * 5f);
        }
    }

    public void HandlePitFall(Vector3 pitPosition, string failType)
    {
        StopPlayerMovement();
        StartCoroutine(ZoomInOnPit(pitPosition, failType));
    }

    private IEnumerator ZoomInOnPit(Vector3 pitPosition, string failType)
    {
        DropFlower();
        DropFlower();

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
        if (wrongFlowersPopup != null && failType == "flowers")
        {
            wrongFlowersPopup.SetActive(true);
        }
        else if (wrongRoadPopUp != null && failType == "road")
        {
            wrongRoadPopUp.SetActive(true);
        }
        else if (wrongFlowersPopup != null && wrongRoadPopUp != null && failType == "both")
        {
            wrongBothPopUp.SetActive(true);
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

    private bool CheckFlowers()
    {
        if (flowersFollow[0] && flowersFollow[1] && flowersFollow[0].CompareTag("OrangeFlower") && flowersFollow[1].CompareTag("OrangeFlower"))
        {
            return true;
        }
        return false;
    }

    private void LevelComplete()
    {
        Debug.Log("Level completed!");
        nextLevelManager.ShowNextLevelButton();
        nextLevelManager.ShowLevelCompleteText();
        StopPlayerMovement();
    }

    private bool HasFlowerSlot()
    {
        return flowersFollow[0] == null || flowersFollow[1] == null;
    }

    private bool HasFlower()
    {
        return flowersFollow[0] != null || flowersFollow[1] != null;
    }

    private void CollectFlower(GameObject flower)
    {
        if (flowersFollow[0] == null) flowersFollow[0] = flower;
        else if (flowersFollow[1] == null && flowersFollow[0] != flower) flowersFollow[1] = flower;
    }

   private void DropFlower()
    {
        if (flowersFollow[1] != null)
        {
            Debug.Log("Dropping flower from slot 1: " + flowersFollow[1].name);
            flowersFollow[1].transform.parent = null; // Unparent the flower
            flowersFollow[1].SetActive(true); // Ensure it's visible after dropping
            flowersFollow[1] = null;
        }
        else if (flowersFollow[0] != null)
        {
            Debug.Log("Dropping flower from slot 0: " + flowersFollow[0].name);
            flowersFollow[0].transform.parent = null; // Unparent the flower
            flowersFollow[0].SetActive(true); // Ensure it's visible after dropping
            flowersFollow[0] = null;
        }
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
