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

        RectTransform canvasRect = canvas.transform as RectTransform;

        Camera cam =
            canvas.renderMode == RenderMode.ScreenSpaceOverlay
                ? null
                : canvas.worldCamera;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect,
                screenPosition,
                cam,
                out Vector2 localPoint))
        {
            target.anchoredPosition = localPoint;
        }
    }
}