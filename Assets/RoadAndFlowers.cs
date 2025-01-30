using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class RoadAndFlowers : MonoBehaviour
{
    [SerializeField] private int totalFlowers = 2; // Total flowers required to pass
    private int FlowersLeft; // Number of wood pieces left to place
    private GameObject[] flowersFollow = {null,null}; // The wood piece following the Player
    private bool canInteract = false; // Prevent repeated interactions
    private Collider2D currentCollider; // The object the player is interacting with
    [SerializeField] private NextLevelManager nextLevelManager; // Reference to the NextLevelManager
    [SerializeField] private Camera mainCamera; // Reference to the main Camera
    [SerializeField] private float zoomDuration = 2f; // Duration of the camera zoom effect
    [SerializeField] private float zoomSize = 3f; // Camera size when zoomed in
    [SerializeField] private GameObject wrongFlowersPopup; // UI Popup for wrong item
    [SerializeField] private GameObject wrongRoadPopUp; 
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

        FlowersLeft = totalFlowers;

        // Hide the wrong item popup initially
        if (wrongFlowersPopup != null)
        {
            wrongFlowersPopup.SetActive(false);
        }
        
        if (wrongRoadPopUp != null)
        {
            wrongRoadPopUp.SetActive(false);
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
        if (currentCollider == null || collision.CompareTag("Barrier") || collision.CompareTag("OtherFlower")|| collision.CompareTag("OrangeFlower"))
        {
            currentCollider = collision;
            canInteract = true;
        }

        Debug.Log($"Entered trigger with {collision.name}");

        if(collision.CompareTag("Barrier")){
            HandlePitFall(collision.transform.position);
        }
        
        if(collision.CompareTag("bike"))
        {
            CheckFlowers();
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
        if (!isFlowerFollow() && currentCollider != null && (currentCollider.CompareTag("OtherFlower") || currentCollider.CompareTag("OrangeFlower")))
        {
            // Collect the wood piece
            CollectFlower(currentCollider.gameObject);
        }
        else if (isFlowerFollow())
        {
            DropFlower();
        }
    }


    // private void FixedUpdate()
    // {
    //     if (isFlowerFollow())
    //     {
    //         getFlowersFollow().transform.position = Vector2.Lerp(
    //             getFlowersFollow().transform.position,
    //             transform.position,
    //             Time.deltaTime * 5f
    //         );
    //     }
    // }

    private void FixedUpdate()
    {
        PositionFlowers();
    }

    private void PositionFlowers()
    {
        Vector2 basePosition = transform.position;
        if (flowersFollow[0] != null)
        {
            flowersFollow[0].transform.position = Vector2.Lerp(flowersFollow[0].transform.position, basePosition + Vector2.left * 0.5f, Time.deltaTime * 5f);
        }
        if (flowersFollow[1] != null)
        {
            flowersFollow[1].transform.position = Vector2.Lerp(flowersFollow[1].transform.position, basePosition + Vector2.right * 0.5f, Time.deltaTime * 5f);
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
        if (wrongFlowersPopup != null)
        {
            wrongFlowersPopup.SetActive(true);
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

    private void ZoomInOnRoad()
    {
        StartCoroutine(ZoomInOnRoadCoroutine());
    }

    private IEnumerator ZoomInOnRoadCoroutine()
    {
        DropFlower();

        SpriteRenderer playerRenderer = GetComponent<SpriteRenderer>();
        if (playerRenderer != null)
        {
            playerRenderer.enabled = false;
        }
        
        // Get the player's position to zoom in on
        Vector3 playerPosition = transform.position; // Get the player's current position
        Vector3 targetPosition = new Vector3(playerPosition.x, playerPosition.y, mainCamera.transform.position.z);

        float elapsedTime = 0f;

        if (wrongRoadPopUp!= null)
        {
            wrongRoadPopUp.SetActive(true);
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

    private bool HasFlowerSlot(){
        return flowersFollow[0] == null || flowersFollow[1] == null;
    }

    private bool isFlowerFollow(){
        return flowersFollow[0] != null || flowersFollow[1] != null;
    }

    private void CollectFlower(GameObject flower){
        if(flowersFollow[0] == null){
            flowersFollow[0] = flower;
            Debug.Log($"{flower.name} is now following the Player.");
        }else if(flowersFollow[1] == null){
            flowersFollow[1] = flower;
            Debug.Log($"{flower.name} is now following the Player.");
        }
    }

    private void DropFlower(){
        if(flowersFollow[0] != null){
            flowersFollow[0] = null;
        }else if(flowersFollow[1] != null){
            flowersFollow[1] = null;
        }
    }

    private void CheckFlowers(){
        if(!HasFlowerSlot()){
            if (flowersFollow[0].CompareTag("OrangeFlower") && flowersFollow[1].CompareTag("OrangeFlower"))
            {
                LevelComplete();
            }
            else
            {
                if (wrongFlowersPopup != null)
                {
                    wrongFlowersPopup.SetActive(true);
                }
            }
        }
    }

    private GameObject getFlowersFollow(){
        if (flowersFollow[0] != null)
        {
            return flowersFollow[0];
        }
        else if (flowersFollow[1] != null)
        {
            return flowersFollow[1];
        }
        return null;
    }

}
