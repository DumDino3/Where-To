using DG.Tweening;
using UnityEngine;
public class PromptZoom : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private RectTransform zoomTarget;
    [SerializeField] private RectTransform reverseZoomTarget;

    [Header("Tween Settings")]
    [SerializeField] private float zoomDuration = 0.2f;
    [SerializeField] private Ease zoomEase = Ease.OutBack;

    private Tween currentTween;
    private Tween reverseTween;

    void Awake()
    {
        //weird
        if (zoomTarget == null)
            zoomTarget = GetComponent<RectTransform>();

        // Start hidden
        zoomTarget.localScale = Vector3.zero;

        // Start reverse target shown (if assigned) so it can shrink when we zoom in
        if (reverseZoomTarget != null)
            reverseZoomTarget.localScale = Vector3.one;
    }

    public void ZoomIn()
    {
        currentTween?.Kill();
        reverseTween?.Kill();

        if (reverseZoomTarget != null)
        {
            reverseTween = reverseZoomTarget.DOScale(Vector3.zero, zoomDuration).SetEase(Ease.InBack);
        }

        currentTween = zoomTarget.DOScale(Vector3.one, zoomDuration).SetEase(zoomEase);
    }

    public void ZoomOut()
    {
        currentTween?.Kill();
        reverseTween?.Kill();

        currentTween = zoomTarget.DOScale(Vector3.zero, zoomDuration).SetEase(Ease.InBack);
    
        if (reverseZoomTarget != null)
        {
            reverseTween = reverseZoomTarget.DOScale(Vector3.one, zoomDuration).SetEase(zoomEase);
        }
    }
}