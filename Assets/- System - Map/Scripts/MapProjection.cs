using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Minimal map projection utility.
/// Camera defines map space.
/// No masking, no zoom, no pan.
/// </summary>
public class MapProjection : MonoBehaviour
{
    [Header("Required")]
    public Camera mapCamera;                 // Orthographic, fixed
    public RectTransform mapImage;           // RawImage RectTransform
    public float worldDepth = 0f;             // Distance from camera to map plane

    // -----------------------------
    // WORLD -> UV (player, NPCs)
    // -----------------------------
    public Vector2 WorldToUV(Vector3 worldPos)
    {
        Vector3 vp = mapCamera.WorldToViewportPoint(worldPos);
        return new Vector2(vp.x, vp.y);
    }

    // -----------------------------
    // UV -> WORLD (clicks, markers)
    // -----------------------------
    public Vector3 UVToWorld(Vector2 uv)
    {
        return mapCamera.ViewportToWorldPoint(
            new Vector3(uv.x, uv.y, worldDepth)
        );
    }

    // -----------------------------
    // SCREEN -> UV (mouse click)
    // -----------------------------
    public Vector2 ScreenToUV(Vector2 screenPos)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            mapImage,
            screenPos,
            null,
            out Vector2 local
        );

        Rect r = mapImage.rect;

        float u = Mathf.InverseLerp(r.xMin, r.xMax, local.x);
        float v = Mathf.InverseLerp(r.yMin, r.yMax, local.y);

        return new Vector2(u, v);
    }

    // -----------------------------
    // UV -> UI (icons, overlays)
    // -----------------------------
    public Vector2 UVToUI(Vector2 uv)
    {
        Rect r = mapImage.rect;

        return new Vector2(
            Mathf.Lerp(r.xMin, r.xMax, uv.x),
            Mathf.Lerp(r.yMin, r.yMax, uv.y)
        );
    }
}