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
    [SerializeField] private float flowerOffset = 0.5f;
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

        if (collision.CompareTag("Barrier")){
            if (CheckFlowers())
            {
               HandlePitFall(collision.transform.position, "road");
            }
            else
            {
                HandlePitFall(collision.transform.position, "both");
            }
        }
        else if (collision.CompareTag("bike")){
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
        if (HasFlowerSlot() && (currentCollider.CompareTag("OtherFlower") || currentCollider.CompareTag("OrangeFlower")))
        {
            CollectFlower(currentCollider.gameObject);
        }
        else if (HasFlower())
        {
            DropFlower();
        }
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
        StartCoroutine(ZoomInOnPit(pitPosition, failType));
    }

    private IEnumerator ZoomInOnPit(Vector3 pitPosition, string failType)
    {
        this.GetComponent<CharacterMovement>().enabled = false;
        this.GetComponent<Mover>().enabled = false;
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
        this.GetComponent<CharacterMovement>().enabled = false;
        this.GetComponent<Mover>().enabled = false;
        this.GetComponent<Animator>().enabled = false;
    }

    private bool HasFlowerSlot()
    {
        return flowersFollow[0] == null || flowersFollow[1] == null;
    }

    private bool HasFlower(){
        return flowersFollow[0] != null || flowersFollow[1] != null;
    }

    private void CollectFlower(GameObject flower)
    {
        if (flowersFollow[0] == null) flowersFollow[0] = flower;
        else if (flowersFollow[1] == null) flowersFollow[1] = flower;
    }

    private void DropFlower()
    {
        if (flowersFollow[1] != null) flowersFollow[1] = null;
        else if (flowersFollow[0] != null) flowersFollow[0] = null;
    }
}
