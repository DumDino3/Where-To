using UnityEngine;

public readonly struct InteractionPointerEvent
{
    public readonly Vector2 ScreenPosition;
    public readonly Vector2 ScreenDelta;
    public readonly Ray Ray;
    public readonly bool HasCurrentHit;
    public readonly RaycastHit CurrentHit;
    public readonly bool HasPressStartHit;
    public readonly RaycastHit PressStartHit;
    public readonly float HeldDuration;
    public readonly bool IsDragging;

    public InteractionPointerEvent(
        Vector2 screenPosition,
        Vector2 screenDelta,
        Ray ray,
        bool hasCurrentHit,
        RaycastHit currentHit,
        bool hasPressStartHit,
        RaycastHit pressStartHit,
        float heldDuration,
        bool isDragging)
    {
        ScreenPosition = screenPosition;
        ScreenDelta = screenDelta;
        Ray = ray;
        HasCurrentHit = hasCurrentHit;
        CurrentHit = currentHit;
        HasPressStartHit = hasPressStartHit;
        PressStartHit = pressStartHit;
        HeldDuration = heldDuration;
        IsDragging = isDragging;
    }
}
