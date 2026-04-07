using System;
using UnityEngine;
using Yarn.Unity;

public class DialogueRouter : MonoBehaviour
{
    private void OnEnable()
    {
        CabinStateMachine.OnCabinStateChanged += OnCabinStateChanged;
    }

    private void OnDisable()
    {
        CabinStateMachine.OnCabinStateChanged -= OnCabinStateChanged;
    }

    private void OnCabinStateChanged(CabinStateMachine.State state)
    {
        if (state == CabinStateMachine.State.Picked){
            HandlePickupDialogue();
        }

        else if (state == CabinStateMachine.State.Dropped){
            //HandleDroppedDialogue();
        }
    }

    private void HandlePickupDialogue()
    {
        string spawnPointId = GetInteractedSpawnPointID();
        var matchingRequest = FindRideRequestBySpawnId(spawnPointId);
        DialogueHelper.Instance?.RunDialogue(matchingRequest.Value.dialoguePoolId);
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

    private string GetInteractedSpawnPointID()
    {
        // Placeholder: return the spawn point ID from the currently interacted SpawnPoint.
        // Replace this stub with the real interaction lookup when ready.
        return "eu";
    }
}