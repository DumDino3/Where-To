public interface IPhysicsPointerInteractable
{
    void OnPointerHoverEnter(InteractionPointerEvent eventData);
    void OnPointerHoverExit(InteractionPointerEvent eventData);
    void OnPointerDown(InteractionPointerEvent eventData);
    void OnPointerHold(InteractionPointerEvent eventData);
    void OnPointerDrag(InteractionPointerEvent eventData);
    void OnPointerUp(InteractionPointerEvent eventData);
    void OnPointerClick(InteractionPointerEvent eventData);
}
