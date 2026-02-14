using System.Collections.Generic;
using UnityEngine;

public class UIArtBoardSketch : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private RectTransform artBoardRect;   // Panel/board area
    [SerializeField] private GameObject artBoardRoot;      // Optional: the whole artboard UI root to toggle (can be same object as artBoardRect)
    [SerializeField] private Camera uiCamera;              // Null for Screen Space Overlay; assign for Screen Space Camera/World Space

    [Header("Drawing")]
    [SerializeField] private LineRenderer strokePrefab;
    [SerializeField] private float minPointDistance = 4f;  // in local UI units (pixels in overlay)
    [SerializeField] private float strokeWidth = 8f;       // local UI units
    [SerializeField] private int maxPositionsPerStroke = 2000;

    [Header("Eraser")]
    [SerializeField] private KeyCode eraserHoldKey = KeyCode.E;
    [SerializeField] private float eraserRadius = 18f;     // local UI units
    [SerializeField] private bool eraseOnDrag = true;

    private readonly List<Stroke> strokes = new List<Stroke>();
    private Stroke currentStroke;

    private bool boardOpen = true;

    private class Stroke
    {
        public LineRenderer lr;
        public readonly List<Vector2> points = new List<Vector2>(); // local points in artBoardRect space
    }

    private void Awake()
    {
        if (artBoardRoot == null && artBoardRect != null)
            artBoardRoot = artBoardRect.gameObject;

        SetBoardOpen(boardOpen);
    }

    private void Update()
    {
        // Toggle artboard
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            SetBoardOpen(!boardOpen);
        }

        if (!boardOpen || artBoardRect == null)
            return;

        // Read pointer
        Vector2 screenPos = Input.mousePosition;

        // Only interact if pointer is inside board rect
        if (!RectTransformUtility.RectangleContainsScreenPoint(artBoardRect, screenPos, uiCamera))
        {
            // If we were drawing and moved off-board, end stroke cleanly
            if (Input.GetMouseButtonUp(0))
                currentStroke = null;
            return;
        }

        bool erasing = Input.GetKey(eraserHoldKey);

        if (erasing)
        {
            HandleEraser(screenPos);
        }
        else
        {
            HandleDraw(screenPos);
        }
    }

    private void SetBoardOpen(bool open)
    {
        boardOpen = open;
        if (artBoardRoot != null)
            artBoardRoot.SetActive(open);

        // stop current stroke if closing
        if (!open)
            currentStroke = null;
    }

    private void HandleDraw(Vector2 screenPos)
    {
        // start stroke
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 local;
            if (!ScreenToBoardLocal(screenPos, out local))
                return;

            currentStroke = CreateStroke();
            AddPoint(currentStroke, local, force: true);
        }

        // continue stroke
        if (Input.GetMouseButton(0) && currentStroke != null)
        {
            Vector2 local;
            if (!ScreenToBoardLocal(screenPos, out local))
                return;

            AddPoint(currentStroke, local, force: false);
        }

        // end stroke
        if (Input.GetMouseButtonUp(0))
        {
            currentStroke = null;
        }
    }

    private void HandleEraser(Vector2 screenPos)
    {
        if (!Input.GetMouseButton(0))
            return;

        if (!eraseOnDrag && !Input.GetMouseButtonDown(0))
            return;

        Vector2 local;
        if (!ScreenToBoardLocal(screenPos, out local))
            return;

        int hitIndex = FindStrokeNearPoint(local, eraserRadius);
        if (hitIndex >= 0)
        {
            Destroy(strokes[hitIndex].lr.gameObject);
            strokes.RemoveAt(hitIndex);
        }
    }

    private Stroke CreateStroke()
    {
        LineRenderer lr = Instantiate(strokePrefab, artBoardRect);
        lr.useWorldSpace = false;

        lr.positionCount = 0;
        lr.startWidth = strokeWidth;
        lr.endWidth = strokeWidth;

        // Optional smoothing settings (you can also set these on the prefab)
        // lr.numCapVertices = 6;
        // lr.numCornerVertices = 6;

        var s = new Stroke { lr = lr };
        strokes.Add(s);
        return s;
    }

    private void AddPoint(Stroke s, Vector2 localPoint, bool force)
    {
        if (s.points.Count >= maxPositionsPerStroke)
            return;

        if (!force && s.points.Count > 0)
        {
            float d = Vector2.Distance(s.points[s.points.Count - 1], localPoint);
            if (d < minPointDistance)
                return;
        }

        s.points.Add(localPoint);

        s.lr.positionCount = s.points.Count;
        s.lr.SetPosition(s.points.Count - 1, (Vector3)localPoint);
    }

    private bool ScreenToBoardLocal(Vector2 screenPos, out Vector2 local)
    {
        return RectTransformUtility.ScreenPointToLocalPointInRectangle(
            artBoardRect, screenPos, uiCamera, out local
        );
    }

    private int FindStrokeNearPoint(Vector2 p, float radius)
    {
        float r2 = radius * radius;

        // Scan from newest to oldest so the “topmost” recent stroke gets erased first.
        for (int i = strokes.Count - 1; i >= 0; i--)
        {
            var pts = strokes[i].points;
            if (pts.Count == 0) continue;

            // Quick check: if near any segment
            if (pts.Count == 1)
            {
                if ((pts[0] - p).sqrMagnitude <= r2) return i;
            }
            else
            {
                for (int k = 0; k < pts.Count - 1; k++)
                {
                    float d2 = DistancePointToSegmentSqr(p, pts[k], pts[k + 1]);
                    if (d2 <= r2) return i;
                }
            }
        }

        return -1;
    }

    // Squared distance from point p to segment ab
    private static float DistancePointToSegmentSqr(Vector2 p, Vector2 a, Vector2 b)
    {
        Vector2 ab = b - a;
        float abLen2 = ab.sqrMagnitude;
        if (abLen2 < 1e-6f) return (p - a).sqrMagnitude;

        float t = Vector2.Dot(p - a, ab) / abLen2;
        t = Mathf.Clamp01(t);
        Vector2 proj = a + t * ab;
        return (p - proj).sqrMagnitude;
    }

    // Optional convenience
    public void ClearAll()
    {
        for (int i = strokes.Count - 1; i >= 0; i--)
        {
            if (strokes[i].lr != null)
                Destroy(strokes[i].lr.gameObject);
        }
        strokes.Clear();
        currentStroke = null;
    }
}
