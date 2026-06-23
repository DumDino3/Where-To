using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private float raycastDistance = 5f;

    private Camera mainCamera;
    private IPlayerInteraction currentInteractable;
    private GameObject currentInteractableObject;
    private IPlayerInteraction pressedInteractable;
    private GameObject pressedInteractableObject;

    [HideInInspector] public bool isTalking = false;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (!isTalking)
        {
            HandleInteraction();
        }
    }

    private void HandleInteraction()
    {
        // Get the ray from the center of the screen
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        Debug.DrawRay(ray.origin, ray.direction * raycastDistance, Color.red);

        IPlayerInteraction hitInteractable = null;
        GameObject hitInteractableObject = null;

        if (Physics.Raycast(ray, out RaycastHit hit, raycastDistance))
        {
            hitInteractable = hit.collider.GetComponentInParent<IPlayerInteraction>();
            if (hitInteractable is Component interactableComponent)
            {
                hitInteractableObject = interactableComponent.gameObject;
            }
            else
            {
                hitInteractableObject = hit.collider.gameObject;
            }
        }

        if (hitInteractable != currentInteractable)
        {
            if (currentInteractable != null)
            {
                currentInteractable.Unhighlight(currentInteractableObject);
            }

            currentInteractable = hitInteractable;
            currentInteractableObject = hitInteractableObject;

            if (currentInteractable != null)
            {
                currentInteractable.Highlight(currentInteractableObject);
            }
        }

        if (currentInteractable != null)
        {
            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            {
                pressedInteractable = currentInteractable;
                pressedInteractableObject = currentInteractableObject;
                currentInteractable.OnInteractDown(currentInteractableObject);
            }
        }

        if (pressedInteractable != null)
        {
            if (Mouse.current != null && Mouse.current.leftButton.wasReleasedThisFrame)
            {
                pressedInteractable.OnInteractUp(pressedInteractableObject);
                pressedInteractable = null;
                pressedInteractableObject = null;
            }
        }
    }
}
