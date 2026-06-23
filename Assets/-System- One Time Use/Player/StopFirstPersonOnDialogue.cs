using UnityEngine;
using Yarn.Unity;

public class StopFirstPersonOnDialogue : MonoBehaviour
{
    private DialogueRunner dialogueRunner;
    private Player player;
    private PlayerMovement playerMovement;
    private PlayerInteraction playerInteraction;

    void Awake()
    {
        dialogueRunner = FindFirstObjectByType<DialogueRunner>(FindObjectsInactive.Include);

        var playerGO = GameObject.FindGameObjectWithTag("Player");
        if (playerGO != null)
        {
            playerGO.TryGetComponent(out player);
            playerGO.TryGetComponent(out playerMovement);
            playerGO.TryGetComponent(out playerInteraction);
        }
        else
        {
            Debug.LogWarning("StopFirstPersonOnDialogue: Player tag not found in scene.");
        }
    }

    void OnEnable()
    {
        if (dialogueRunner == null)
        {
            //Debug.LogError("StopFirstPersonOnDialogue: no DialogueRunner in scene");
            enabled = false;
            return;
        }

        dialogueRunner.onDialogueStart.AddListener(HandleDialogueStart);
        dialogueRunner.onDialogueComplete.AddListener(HandleDialogueEnd);
    }

    void OnDisable()
    {
        if (dialogueRunner == null) return;
        dialogueRunner.onDialogueStart.RemoveListener(HandleDialogueStart);
        dialogueRunner.onDialogueComplete.RemoveListener(HandleDialogueEnd);
    }

    private void HandleDialogueStart()
    {
        SetTalking(true);
    }

    private void HandleDialogueEnd()
    {
        SetTalking(false);
    }

    private void SetTalking(bool talking)
    {
        if (talking)
        {
            if (playerMovement != null) playerMovement.isTalking = true;
            if (playerInteraction != null) playerInteraction.isTalking = true;
            if (player != null)
            {
                player.FreezeMovementOnly();
                player.DisablePlayer();
            }
        }
        else
        {
            if (playerMovement != null) playerMovement.isTalking = false;
            if (playerInteraction != null) playerInteraction.isTalking = false;
            if (player != null)
            {
                player.UnFreezeMovementOnly();
                player.EnablePlayer();
            }
        }
    }
}
