using UnityEngine;
using UnityEngine.UI;

public class ArrowPointerController : MonoBehaviour
{
    public Transform player;  // Player in world space
    public Transform bike;    // Target in world space
    public RectTransform arrowUI; // UI arrow RectTransform
    public Camera mainCamera; // Assign main camera in Inspector

    void Update()
    {
        if (bike == null || player == null || arrowUI == null || mainCamera == null)
            return;

        // Convert world position to screen space
        Vector3 bikeScreenPos = mainCamera.WorldToScreenPoint(bike.position);
        Vector3 playerScreenPos = mainCamera.WorldToScreenPoint(player.position);

        // Direction in screen space
        Vector3 direction = (bikeScreenPos - playerScreenPos).normalized;

        // Calculate angle (fixing offset so UP means forward)
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

        // Apply rotation to the arrow
        arrowUI.rotation = Quaternion.Euler(0, 0, angle);
    }
}
