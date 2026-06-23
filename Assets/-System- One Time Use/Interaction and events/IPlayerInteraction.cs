using UnityEngine;

public interface IPlayerInteraction
{
    void Highlight(GameObject highlightedObject);
    void Unhighlight(GameObject highlightedObject);
    void OnInteractDown(GameObject highlightedObject);
    void OnInteractUp(GameObject highlightedObject);
}
