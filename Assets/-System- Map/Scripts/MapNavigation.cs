using UnityEngine;

public class MapNavigation : MonoBehaviour
{
    [Header("References")]
    public RectTransform mapRoot;
    public Canvas canvas;

    [Header("Zoom")]
    public float zoomSpeed = 0.12f;
    public float minZoom = 0.5f;
    public float maxZoom = 3f;

    [Header("Pan")]
    public int panMouseButton = 2; // 2 = middle mouse
    public float panSpeed = 1f;

    private Vector2 lastMousePos;

    void Awake()
    {
        if (mapRoot == null)
            mapRoot = GetComponent<RectTransform>();

        if (canvas == null)
            canvas = GetComponentInParent<Canvas>();
    }

    void Update()
    {
        HandleZoom();
        HandlePan();
    }

    // ---------------- ZOOM (CURSOR ANCHORED) ----------------

    private void HandleZoom()
    {
        float scroll = Input.mouseScrollDelta.y;
        if (Mathf.Approximately(scroll, 0)) return;

        float currentScale = mapRoot.localScale.x;
        float targetScale = Mathf.Clamp(
            currentScale * (1f + scroll * zoomSpeed),
            minZoom,
            maxZoom
        );

        if (Mathf.Approximately(currentScale, targetScale))
            return;

        RectTransform canvasRect = canvas.transform as RectTransform;

        // Mouse position in map local space BEFORE scaling
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            mapRoot,
            Input.mousePosition,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay
                ? null
                : canvas.worldCamera,
            out Vector2 localPointBefore
        );

        // Apply scale
        mapRoot.localScale = Vector3.one * targetScale;

        // Mouse position in map local space AFTER scaling
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            mapRoot,
            Input.mousePosition,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay
                ? null
                : canvas.worldCamera,
            out Vector2 localPointAfter
        );

        // Offset map so cursor anchor stays fixed
        Vector2 delta = localPointAfter - localPointBefore;
        mapRoot.localPosition += (Vector3)delta;
    }

    // ---------------- PAN ----------------

    private void HandlePan()
    {
        if (!Input.GetMouseButton(panMouseButton)) return;

        Vector2 mousePos = Input.mousePosition;

        if (Input.GetMouseButtonDown(panMouseButton))
        {
            lastMousePos = mousePos;
            return;
        }

        Vector2 delta = mousePos - lastMousePos;
        lastMousePos = mousePos;

        mapRoot.localPosition += (Vector3)(delta * panSpeed);
    }
}
