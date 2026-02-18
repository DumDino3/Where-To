using UnityEngine;

/// <summary>
/// Binds a world GameObject to a UI icon on the map.
/// </summary>
public class GPSMarker : MonoBehaviour
{
    [Header("References")]
    public Transform worldTarget;          // Player / NPC / object in world
    public RectTransform icon;              // UI icon on map
    public MapProjection mapProjection;     // The projection utility

    private void LateUpdate()
    {
        if (worldTarget == null || icon == null || mapProjection == null)
            return;

        // World -> UV
        Vector2 uv = mapProjection.WorldToUV(worldTarget.position);

        // UV -> UI
        icon.anchoredPosition = mapProjection.UVToUI(uv);
    }
}