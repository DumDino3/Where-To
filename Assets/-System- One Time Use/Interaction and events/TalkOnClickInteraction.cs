using UnityEngine;

public class TalkOnClickInteraction : PhysicsPointerInteractableBase
{
    [SerializeField] private DialogueTrigger dialogueTrigger;

    private void Awake()
    {
        if (dialogueTrigger == null)
            TryGetComponent(out dialogueTrigger);
    }

    public override void OnPointerClick(InteractionPointerEvent eventData)
    {
        dialogueTrigger?.TryRunDialogue();
    }
}
