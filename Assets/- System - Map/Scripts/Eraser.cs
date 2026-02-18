using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using System.Collections.Generic;

public class StickerEraser : MonoBehaviour
{
    [Header("References")]
    public Canvas canvas;
    public GraphicRaycaster raycaster;

    [Header("Input")]
    public KeyCode toggleKey = KeyCode.E;

    [Header("Scale Settings")]
    public float showHideDuration = 0.15f;
    public float pressScale = 0.8f;

    private RectTransform rectTransform;
    private Vector3 baseScale = Vector3.one;
    private bool isEnabled;
    private bool isPointerDown;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        if (canvas == null)
            canvas = GetComponentInParent<Canvas>();

        if (raycaster == null)
            raycaster = canvas.GetComponent<GraphicRaycaster>();

        // Start hidden
        rectTransform.localScale = Vector3.zero;
    }

    void Update()
    {
        HandleToggle();

        if (!isEnabled) return;

        FollowCursor();
        HandlePointerScale();
        Scrub();
    }

    // ---------------- TOGGLE ----------------

    private void HandleToggle()
    {
        if (!Input.GetKeyDown(toggleKey)) return;

        isEnabled = !isEnabled;

        rectTransform.DOKill();

        if (isEnabled)
        {
            rectTransform
                .DOScale(baseScale, showHideDuration)
                .SetEase(Ease.OutBack);
        }
        else
        {
            isPointerDown = false;

            rectTransform
                .DOScale(Vector3.zero, showHideDuration)
                .SetEase(Ease.InBack);
        }
    }

    // ---------------- POINTER SCALE ----------------

    private void HandlePointerScale()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isPointerDown = true;

            rectTransform.DOKill();
            rectTransform
                .DOScale(baseScale * pressScale, 0.08f)
                .SetEase(Ease.OutQuad);
        }

        if (Input.GetMouseButtonUp(0))
        {
            isPointerDown = false;

            rectTransform.DOKill();
            rectTransform
                .DOScale(baseScale, 0.08f)
                .SetEase(Ease.OutQuad);
        }
    }

    // ---------------- CURSOR FOLLOW ----------------

    private void FollowCursor()
    {
        RectTransform canvasRect = canvas.transform as RectTransform;

        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            canvasRect,
            Input.mousePosition,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay
                ? null
                : canvas.worldCamera,
            out Vector3 worldPos
        );

        rectTransform.position = worldPos;
    }

    // ---------------- SCRUB ----------------

    private void Scrub()
    {
        if (!isPointerDown) return;

        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerData, results);

        foreach (var result in results)
        {
            Stickers sticker = result.gameObject.GetComponentInParent<Stickers>();
            if (sticker != null)
            {
                Destroy(sticker.gameObject);
                break; // erase one per frame
            }
        }
    }
}
