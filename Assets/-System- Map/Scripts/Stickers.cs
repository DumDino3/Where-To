using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class Stickers : MonoBehaviour, IPointerClickHandler
{
    [Header("References")]
    public Canvas canvas;

    [Header("Scale Settings")]
    public float pickUpScale = 1.2f;
    public float tweenDuration = 0.15f;

    private RectTransform rectTransform;
    private bool isHeld;
    private bool clickLocked;
    private Vector3 originalScale;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalScale = rectTransform.localScale;

        if (canvas == null)
            canvas = GetComponentInParent<Canvas>();
    }

    void Update()
    {
        if (isHeld)
            FollowCursorWorld();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (clickLocked) return;

        ToggleHold();
    }

    public void ForcePickUp(float graceTime)
    {
        isHeld = true;
        clickLocked = true;

        rectTransform.SetAsLastSibling();

        rectTransform.DOKill();
        rectTransform
            .DOScale(originalScale * pickUpScale, tweenDuration)
            .SetEase(Ease.OutBack);

        Invoke(nameof(UnlockClick), graceTime);
    }

    private void ToggleHold()
    {
        isHeld = !isHeld;

        rectTransform.DOKill();

        rectTransform
            .DOScale(isHeld ? originalScale * pickUpScale : originalScale, tweenDuration)
            .SetEase(Ease.OutBack);

        if (isHeld)
            rectTransform.SetAsLastSibling();
    }

    private void UnlockClick()
    {
        clickLocked = false;
    }

    private void FollowCursorWorld()
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
}
