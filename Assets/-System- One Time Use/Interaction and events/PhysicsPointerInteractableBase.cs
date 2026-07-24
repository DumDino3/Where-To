using UnityEngine;

public class PhysicsPointerInteractableBase : MonoBehaviour, IPhysicsPointerInteractable
{
    public virtual void OnPointerHoverEnter(InteractionPointerEvent eventData) { }
    public virtual void OnPointerHoverExit(InteractionPointerEvent eventData) { }
    public virtual void OnPointerDown(InteractionPointerEvent eventData) { }
    public virtual void OnPointerHold(InteractionPointerEvent eventData) { }
    public virtual void OnPointerDrag(InteractionPointerEvent eventData) { }
    public virtual void OnPointerUp(InteractionPointerEvent eventData) { }
    public virtual void OnPointerClick(InteractionPointerEvent eventData) { }
}
