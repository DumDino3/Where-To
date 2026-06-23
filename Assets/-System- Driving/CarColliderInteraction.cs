using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Yarn.Unity;

public class CarColliderInteraction : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    [Header("Tracked Collider")]
    [SerializeField] private Collider currentCollider;

    [Header("Dialogue")]
    [SerializeField] private string paulTag = "Paul";
    [SerializeField] private string paulDialogueNode = "InTheCar";
    [SerializeField] private string homeTag = "Home";
    [SerializeField] private string homeDialogueNode = "Bye";
    [SerializeField] private DialogueRunner dialogueRunner;
    [SerializeField] private GameObject paulMapIcon;
    [SerializeField] private GameObject homeMapIcon;

    [Header("After Bye")]
    [SerializeField] private Canvas byeCanvas;

    private readonly List<Collider> overlappingColliders = new List<Collider>();
    private bool waitingForByeDialogue;

    public Collider CurrentCollider => currentCollider;

    private void Awake()
    {
        if (dialogueRunner == null)
        {
            dialogueRunner = FindFirstObjectByType<DialogueRunner>(FindObjectsInactive.Include);
        }

        if (homeMapIcon != null)
        {
            homeMapIcon.SetActive(false);
        }
    }

    private void OnEnable()
    {
        if (dialogueRunner != null)
        {
            dialogueRunner.onDialogueComplete.AddListener(HandleDialogueComplete);
        }
    }

    private void OnDisable()
    {
        if (dialogueRunner != null)
        {
            dialogueRunner.onDialogueComplete.RemoveListener(HandleDialogueComplete);
        }
    }

    private void Update()
    {
        CleanupMissingColliders();

        if (Input.GetKeyDown(interactKey))
        {
            TryInteract();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!overlappingColliders.Contains(other))
        {
            overlappingColliders.Add(other);
        }

        currentCollider = other;
    }

    private void OnTriggerExit(Collider other)
    {
        overlappingColliders.Remove(other);

        if (currentCollider == other)
        {
            currentCollider = overlappingColliders.Count > 0
                ? overlappingColliders[overlappingColliders.Count - 1]
                : null;
        }
    }

    private void TryInteract()
    {
        if (currentCollider == null)
        {
            Debug.Log("CarColliderInteraction: not inside any tracked collider.", this);
            return;
        }

        if (dialogueRunner == null)
        {
            Debug.LogWarning("CarColliderInteraction: no DialogueRunner found.", this);
            return;
        }

        if (dialogueRunner.IsDialogueRunning)
        {
            return;
        }

        if (currentCollider.CompareTag(paulTag))
        {
            if (paulMapIcon != null)
            {
                paulMapIcon.SetActive(false);
            }

            if (homeMapIcon != null)
            {
                homeMapIcon.SetActive(true);
            }

            dialogueRunner.StartDialogue(paulDialogueNode);
            return;
        }

        if (currentCollider.CompareTag(homeTag))
        {
            waitingForByeDialogue = true;
            dialogueRunner.StartDialogue(homeDialogueNode);
        }
    }

    private void HandleDialogueComplete()
    {
        if (!waitingForByeDialogue)
        {
            return;
        }

        waitingForByeDialogue = false;

        if (byeCanvas != null)
        {
            byeCanvas.enabled = true;
        }
    }

    private void CleanupMissingColliders()
    {
        for (int i = overlappingColliders.Count - 1; i >= 0; i--)
        {
            if (overlappingColliders[i] == null)
            {
                overlappingColliders.RemoveAt(i);
            }
        }

        if (currentCollider == null && overlappingColliders.Count > 0)
        {
            currentCollider = overlappingColliders[overlappingColliders.Count - 1];
        }
    }
}
