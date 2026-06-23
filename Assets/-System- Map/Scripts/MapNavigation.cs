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
    private Vector2 centerPosition;

    void Awake()
    {
        if (mapRoot == null)
            mapRoot = GetComponent<RectTransform>();

        if (canvas == null)
            canvas = GetComponentInParent<Canvas>();

        centerPosition = mapRoot.anchoredPosition;
    }

    void OnEnable()
    {
        if (mapRoot != null)
            ClampMapPosition();
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

        Camera eventCamera = GetEventCamera();

        // Mouse position in map local space BEFORE scaling
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
            mapRoot,
            Input.mousePosition,
            eventCamera,
            out Vector2 localPointBefore
        ))
            return;

        // Apply scale
        mapRoot.localScale = Vector3.one * targetScale;

        // Mouse position in map local space AFTER scaling
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
            mapRoot,
            Input.mousePosition,
            eventCamera,
            out Vector2 localPointAfter
        ))
        {
            ClampMapPosition();
            return;
        }

        // Offset map so cursor anchor stays fixed
        Vector2 delta = localPointAfter - localPointBefore;
        mapRoot.anchoredPosition += delta;
        ClampMapPosition();
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

        mapRoot.anchoredPosition += delta * panSpeed;
        ClampMapPosition();
    }

    private Camera GetEventCamera()
    {
        if (canvas == null || canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            return null;

        return canvas.worldCamera != null ? canvas.worldCamera : Camera.main;
    }

    private void ClampMapPosition()
    {
        RectTransform viewport = mapRoot.parent as RectTransform;
        if (viewport == null)
            return;

        Vector2 mapSize = mapRoot.rect.size * mapRoot.localScale.x;
        Vector2 viewportSize = viewport.rect.size;

        float maxOffsetX = Mathf.Max(0f, (mapSize.x - viewportSize.x) * 0.5f);
        float maxOffsetY = Mathf.Max(0f, (mapSize.y - viewportSize.y) * 0.5f);

        Vector2 position = mapRoot.anchoredPosition;
        position.x = Mathf.Clamp(position.x, centerPosition.x - maxOffsetX, centerPosition.x + maxOffsetX);
        position.y = Mathf.Clamp(position.y, centerPosition.y - maxOffsetY, centerPosition.y + maxOffsetY);

        mapRoot.anchoredPosition = position;
    }
}
