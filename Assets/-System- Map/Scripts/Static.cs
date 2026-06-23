using UnityEngine;

public static class UICursorUtility
{
    /// <summary>
    /// Moves a RectTransform to follow the cursor correctly
    /// in Overlay, Camera, and World Space canvases.
    /// </summary>
    public static void FollowCursor(
        RectTransform target,
        Canvas canvas,
        Vector2 screenPosition
    )
    {
        if (canvas == null || target == null)
            return;

        RectTransform targetSpace = target.parent as RectTransform;
        if (targetSpace == null)
            targetSpace = canvas.transform as RectTransform;

        Camera cam = GetEventCamera(canvas);

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                targetSpace,
                screenPosition,
                cam,
                out Vector2 localPoint))
        {
            target.anchoredPosition = localPoint;
        }
    }

    public static Camera GetEventCamera(Canvas canvas)
    {
        if (canvas == null || canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            return null;

        return canvas.worldCamera != null ? canvas.worldCamera : Camera.main;
    }
}
