using UnityEngine;

public class SliderInteraction : PhysicsPointerInteractableBase
{
    [SerializeField] private Transform handle;
    [SerializeField] private Transform leftPoint;
    [SerializeField] private Transform rightPoint;
    [SerializeField] private DialogueTrigger dialogueTrigger;
    [SerializeField, Range(0f, 1f)] private float triggerValue = 0f;
    [SerializeField, Range(0f, 0.25f)] private float triggerTolerance = 0.01f;
    [SerializeField] private bool triggerOnce = true;

    private Plane dragPlane;
    private float normalizedValue;
    private float grabOffset;
    private bool hasTriggered;
    private bool wasInTriggerZone;

    public float NormalizedValue => normalizedValue;

    private void Awake()
    {
        if (handle == null)
            handle = transform;

        if (dialogueTrigger == null)
            TryGetComponent(out dialogueTrigger);

        CacheValueFromHandle();
    }

    public override void OnPointerDown(InteractionPointerEvent eventData)
    {
        if (!TryBuildDragPlane(eventData.Ray))
            return;

        if (TryGetValueFromRay(eventData.Ray, out float pointerValue))
            grabOffset = normalizedValue - pointerValue;
    }

    public override void OnPointerDrag(InteractionPointerEvent eventData)
    {
        if (!TryGetValueFromRay(eventData.Ray, out float pointerValue))
            return;

        SetNormalizedValue(pointerValue + grabOffset);
    }

    public void SetNormalizedValue(float value)
    {
        normalizedValue = Mathf.Clamp01(value);

        if (handle != null && leftPoint != null && rightPoint != null)
            handle.position = Vector3.Lerp(leftPoint.position, rightPoint.position, normalizedValue);

        CheckDialogueTrigger();
    }

    private void CacheValueFromHandle()
    {
        if (handle == null || leftPoint == null || rightPoint == null)
            return;

        Vector3 left = leftPoint.position;
        Vector3 right = rightPoint.position;
        Vector3 axis = right - left;
        float lengthSquared = axis.sqrMagnitude;

        if (lengthSquared <= Mathf.Epsilon)
            return;

        normalizedValue = Mathf.Clamp01(Vector3.Dot(handle.position - left, axis) / lengthSquared);
        wasInTriggerZone = IsInTriggerZone();
    }

    private bool TryBuildDragPlane(Ray pointerRay)
    {
        if (leftPoint == null || rightPoint == null)
            return false;

        Vector3 axis = rightPoint.position - leftPoint.position;
        if (axis.sqrMagnitude <= Mathf.Epsilon)
            return false;

        axis.Normalize();
        Vector3 normal = Vector3.Cross(axis, Vector3.Cross(pointerRay.direction, axis));

        if (normal.sqrMagnitude <= Mathf.Epsilon)
            normal = -pointerRay.direction;

        dragPlane = new Plane(normal.normalized, leftPoint.position);
        return true;
    }

    private bool TryGetValueFromRay(Ray pointerRay, out float value)
    {
        value = normalizedValue;

        if (leftPoint == null || rightPoint == null)
            return false;

        if (!dragPlane.Raycast(pointerRay, out float distance))
            return false;

        Vector3 left = leftPoint.position;
        Vector3 right = rightPoint.position;
        Vector3 axis = right - left;
        float lengthSquared = axis.sqrMagnitude;

        if (lengthSquared <= Mathf.Epsilon)
            return false;

        Vector3 point = pointerRay.GetPoint(distance);
        value = Mathf.Clamp01(Vector3.Dot(point - left, axis) / lengthSquared);
        return true;
    }

    private void CheckDialogueTrigger()
    {
        bool isInTriggerZone = IsInTriggerZone();

        if (isInTriggerZone && !wasInTriggerZone && (!hasTriggered || !triggerOnce))
        {
            dialogueTrigger?.TryRunDialogue();
            hasTriggered = true;
        }

        wasInTriggerZone = isInTriggerZone;
    }

    private bool IsInTriggerZone()
    {
        return normalizedValue <= triggerValue + triggerTolerance;
    }
}
