using UnityEngine;
using UnityEngine.Events;

public abstract class PlayerInteractableBase : MonoBehaviour, IPlayerInteraction
{
    [System.Serializable]
    public class GameObjectEvent : UnityEvent<GameObject> { }

    [Header("Hover Feedback")]
    [SerializeField] private PromptZoom highlightZoom;
    [SerializeField] private HighlightOnHover hoverEvents;

    [Header("Interaction Events")]
    [SerializeField] private GameObjectEvent onInteractDown;
    [SerializeField] private GameObjectEvent onInteractUp;

    public virtual void Highlight(GameObject highlightedObject)
    {
        highlightZoom?.ZoomIn();
        hoverEvents?.TriggerHoverEnter();
    }

    public virtual void Unhighlight(GameObject highlightedObject)
    {
        highlightZoom?.ZoomOut();
        hoverEvents?.TriggerHoverExit();
    }

    public virtual void OnInteractDown(GameObject highlightedObject)
    {
        onInteractDown?.Invoke(highlightedObject);
    }

    public virtual void OnInteractUp(GameObject highlightedObject)
    {
        onInteractUp?.Invoke(highlightedObject);
    }
}