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
            FollowCursor();
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

    private void FollowCursor()
    {
        if (canvas == null)
            canvas = GetComponentInParent<Canvas>();

        UICursorUtility.FollowCursor(rectTransform, canvas, Input.mousePosition);
    }
}
