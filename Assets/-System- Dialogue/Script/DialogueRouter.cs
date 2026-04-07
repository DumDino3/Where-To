using System;
using System.Security.Cryptography;
using UnityEngine;
using Yarn.Unity;

public class DialogueRouter : MonoBehaviour
{
    private const string DIALOGUE_POOL_DB_PATH = "SO/Asset/DialoguePoolDatabaseSO";
    private DialoguePoolDatabaseSO dialoguePoolDb;

    int spawnID = 0;
    int dropID = 0;

    private void OnEnable()
    {
        EnsureDialoguePoolDatabase();
        CabinStateMachine.OnCabinStateChanged += OnCabinStateChanged;
        LiveQuestInstance.onQuestAccepted += HandleAcceptedQuest;
    }

    private void OnDisable()
    {
        CabinStateMachine.OnCabinStateChanged -= OnCabinStateChanged;
        LiveQuestInstance.onQuestAccepted -= HandleAcceptedQuest;
    }

    private void OnCabinStateChanged(CabinStateMachine.State state)
    {
        if (state == CabinStateMachine.State.Picked){
            HandlePickupDialogue();
        }

        else if (state == CabinStateMachine.State.Dropped){
            HandleDroppedDialogue();
        }
    }

    private void HandleAcceptedQuest(int spawn, int drop)
    {
        spawnID = spawn;
        dropID = drop;
    }

    private void HandlePickupDialogue()
    {
        string spawnPointId = GetStringID(spawnID);
        var matchingRequest = FindRideRequestBySpawnId(spawnPointId);

        var poolEntry = FindDialoguePoolEntryById(matchingRequest.Value.dialoguePoolId);

        DialogueHelper.Instance.RunDialogue(poolEntry.Value.getOn);
    }

    private void HandleDroppedDialogue()
    {
        string destinationPointId = GetStringID(dropID);
        var matchingRequest = FindRideRequestByDestinationId(destinationPointId);

        var poolEntry = FindDialoguePoolEntryById(matchingRequest.Value.dialoguePoolId);

        DialogueHelper.Instance.RunDialogue(poolEntry.Value.end);
    }

    private RideRequestEntry? FindRideRequestBySpawnId(string spawnId)
    {
        foreach (var request in PooledRequest.Instance.PooledRequests)
        {
            if (request.spawnId == spawnId)
            {
                return request;
            }
        }

        return null;
    }

    private RideRequestEntry? FindRideRequestByDestinationId(string destinationId)
    {
        foreach (var request in PooledRequest.Instance.PooledRequests)
        {
            if (request.destinationId == destinationId)
            {
                return request;
            }
        }

        return null;
    }

    private DialoguePoolEntry? FindDialoguePoolEntryById(string poolId)
    {
        EnsureDialoguePoolDatabase();
        if (dialoguePoolDb == null)
            return null;

        return dialoguePoolDb.SearchByID(poolId);
    }

    private string GetStringID(int id)
    {
        // Convert the numeric ID into a 3-digit string.
        // Example: ID 5 becomes "005".
        return id.ToString("D3");
    }

    private void EnsureDialoguePoolDatabase()
    {
        if (dialoguePoolDb != null)
            return;

        dialoguePoolDb = Resources.Load<DialoguePoolDatabaseSO>(DIALOGUE_POOL_DB_PATH);
        if (dialoguePoolDb == null)
        {
            Debug.LogWarning($"DialogueRouter: failed to load DialoguePoolDatabaseSO at '{DIALOGUE_POOL_DB_PATH}'");
        }
    }
}