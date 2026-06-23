using UnityEngine;
using UnityEngine.InputSystem;

public class MousePhysicsInteraction : MonoBehaviour
{
    [SerializeField] private Camera rayCamera;
    [SerializeField] private float maxDistance = 100f;
    [SerializeField] private LayerMask interactionMask = ~0;
    [SerializeField] private QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Collide;
    [SerializeField] private float clickMaxDuration = 0.4f;
    [SerializeField] private float dragThresholdPixels = 6f;
    [SerializeField] private bool drawDebugRay;

    private IPhysicsPointerInteractable hoveredInteractable;
    private IPhysicsPointerInteractable pressedInteractable;
    private RaycastHit pressStartHit;
    private Vector2 previousScreenPosition;
    private Vector2 pressStartScreenPosition;
    private float pressStartTime;
    private bool hasPreviousScreenPosition;
    private bool hasPressStartHit;
    private bool isDragging;

    private void Awake()
    {
        if (rayCamera == null)
            rayCamera = Camera.main;
    }

    private void Update()
    {
        var mouse = Mouse.current;
        if (rayCamera == null)
            rayCamera = Camera.main;

        if (mouse == null || rayCamera == null)
            return;

        Vector2 screenPosition = mouse.position.ReadValue();
        Vector2 screenDelta = hasPreviousScreenPosition ? screenPosition - previousScreenPosition : Vector2.zero;
        previousScreenPosition = screenPosition;
        hasPreviousScreenPosition = true;

        Ray ray = rayCamera.ScreenPointToRay(screenPosition);
        bool hasCurrentHit = Physics.Raycast(ray, out RaycastHit currentHit, maxDistance, interactionMask, triggerInteraction);

        if (drawDebugRay)
            Debug.DrawRay(ray.origin, ray.direction * maxDistance, hasCurrentHit ? Color.green : Color.red);

        IPhysicsPointerInteractable currentInteractable = GetInteractable(hasCurrentHit ? currentHit.collider : null);
        float heldDuration = pressedInteractable != null ? Time.unscaledTime - pressStartTime : 0f;

        DispatchHover(currentInteractable, screenPosition, screenDelta, ray, hasCurrentHit, currentHit, heldDuration);

        if (mouse.leftButton.wasPressedThisFrame)
        {
            HandlePointerDown(currentInteractable, screenPosition, screenDelta, ray, hasCurrentHit, currentHit);
        }

        if (mouse.leftButton.isPressed && pressedInteractable != null)
        {
            HandlePointerHeld(screenPosition, screenDelta, ray, hasCurrentHit, currentHit);
        }

        if (mouse.leftButton.wasReleasedThisFrame && pressedInteractable != null)
        {
            HandlePointerUp(screenPosition, screenDelta, ray, hasCurrentHit, currentHit);
        }
    }

    private void DispatchHover(
        IPhysicsPointerInteractable currentInteractable,
        Vector2 screenPosition,
        Vector2 screenDelta,
        Ray ray,
        bool hasCurrentHit,
        RaycastHit currentHit,
        float heldDuration)
    {
        if (currentInteractable == hoveredInteractable)
            return;

        InteractionPointerEvent eventData = CreateEvent(screenPosition, screenDelta, ray, hasCurrentHit, currentHit, heldDuration, isDragging);

        if (IsAlive(hoveredInteractable))
            hoveredInteractable.OnPointerHoverExit(eventData);

        hoveredInteractable = currentInteractable;

        if (IsAlive(hoveredInteractable))
            hoveredInteractable.OnPointerHoverEnter(eventData);
    }

    private void HandlePointerDown(
        IPhysicsPointerInteractable currentInteractable,
        Vector2 screenPosition,
        Vector2 screenDelta,
        Ray ray,
        bool hasCurrentHit,
        RaycastHit currentHit)
    {
        if (!IsAlive(currentInteractable))
            return;

        pressedInteractable = currentInteractable;
        pressStartScreenPosition = screenPosition;
        pressStartTime = Time.unscaledTime;
        pressStartHit = currentHit;
        hasPressStartHit = hasCurrentHit;
        isDragging = false;

        InteractionPointerEvent eventData = CreateEvent(screenPosition, screenDelta, ray, hasCurrentHit, currentHit, 0f, false);
        pressedInteractable.OnPointerDown(eventData);
    }

    private void HandlePointerHeld(
        Vector2 screenPosition,
        Vector2 screenDelta,
        Ray ray,
        bool hasCurrentHit,
        RaycastHit currentHit)
    {
        if (!IsAlive(pressedInteractable))
        {
            ClearPress();
            return;
        }

        float heldDuration = Time.unscaledTime - pressStartTime;
        isDragging |= IsPastDragThreshold(screenPosition);

        InteractionPointerEvent eventData = CreateEvent(screenPosition, screenDelta, ray, hasCurrentHit, currentHit, heldDuration, isDragging);
        pressedInteractable.OnPointerHold(eventData);

        if (isDragging && screenDelta.sqrMagnitude > 0.001f)
            pressedInteractable.OnPointerDrag(eventData);
    }

    private void HandlePointerUp(
        Vector2 screenPosition,
        Vector2 screenDelta,
        Ray ray,
        bool hasCurrentHit,
        RaycastHit currentHit)
    {
        if (!IsAlive(pressedInteractable))
        {
            ClearPress();
            return;
        }

        float heldDuration = Time.unscaledTime - pressStartTime;
        isDragging |= IsPastDragThreshold(screenPosition);
        bool isClick = !isDragging && (clickMaxDuration <= 0f || heldDuration <= clickMaxDuration);

        InteractionPointerEvent eventData = CreateEvent(screenPosition, screenDelta, ray, hasCurrentHit, currentHit, heldDuration, isDragging);
        pressedInteractable.OnPointerUp(eventData);

        if (isClick)
            pressedInteractable.OnPointerClick(eventData);

        ClearPress();
    }

    private InteractionPointerEvent CreateEvent(
        Vector2 screenPosition,
        Vector2 screenDelta,
        Ray ray,
        bool hasCurrentHit,
        RaycastHit currentHit,
        float heldDuration,
        bool dragging)
    {
        return new InteractionPointerEvent(
            screenPosition,
            screenDelta,
            ray,
            hasCurrentHit,
            currentHit,
            hasPressStartHit,
            pressStartHit,
            heldDuration,
            dragging);
    }

    private bool IsPastDragThreshold(Vector2 screenPosition)
    {
        return (screenPosition - pressStartScreenPosition).sqrMagnitude >= dragThresholdPixels * dragThresholdPixels;
    }

    private void ClearPress()
    {
        pressedInteractable = null;
        hasPressStartHit = false;
        isDragging = false;
    }

    private static IPhysicsPointerInteractable GetInteractable(Collider hitCollider)
    {
        if (hitCollider == null)
            return null;

        return hitCollider.GetComponentInParent<IPhysicsPointerInteractable>();
    }

    private static bool IsAlive(IPhysicsPointerInteractable interactable)
    {
        if (interactable == null)
            return false;

        return interactable is not Object unityObject || unityObject != null;
    }
}
