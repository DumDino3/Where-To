using UnityEngine;

/// <summary>
/// Binds a world GameObject to a UI icon on the map.
/// </summary>
public class GPSMarker : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private string targetTag;
    private Transform worldTarget;          // Player / NPC / object in world
    public RectTransform icon;              // UI icon on map
    public MapProjection mapProjection;     // The projection utility

    void Start()
    {
        GameObject targetObject = GameObject.FindGameObjectWithTag(targetTag);
        worldTarget = targetObject.transform;
    }
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