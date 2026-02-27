using System;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPaceManager : MonoBehaviour
{
    //hmm if any id being set to 00 or null te system must register random 
    
    public class dataPackage
    {
        private int timeSeg;
        private int duration;
        private int pickUpId;
        private int dropOffId;
    }
    
    [SerializeField]
    int travelIDRaw;

    public int chunkSize = 4;
    [SerializeField] private int duration;
    [SerializeField] private int priority;
    [SerializeField] private int currentID;
    [SerializeField] private int pickUpID;
    [SerializeField] private int dropOffID;
    
    [SerializeField] private CurrentTaxiState currentState;

    public static event Action<int> OnSpawnPointChanged; //this command the director
    
    public static event Action<int> OnChunkChanged;
    
    enum CurrentTaxiState
    {
        PickUp,
        DropOff,
    }



    [ContextMenu("Parse Travel ID")]
    private void ParsingId()
    {
        duration = travelIDRaw / 1000;
        pickUpID = travelIDRaw / 1000000;
        dropOffID = travelIDRaw % 100000;
        priority = dropOffID % 1000;

    }

    [ContextMenu("Test Spawn Event")]
    private void TestSpawnEvent()
    {
        switch (currentState)
        {
            case CurrentTaxiState.PickUp:
                OnSpawnPointChanged?.Invoke(pickUpID);
                Debug.Log($"Invocated PickUp event with ID: {pickUpID}");
                break;

            case CurrentTaxiState.DropOff:
                OnSpawnPointChanged?.Invoke(dropOffID);
                Debug.Log($"Invocated DropOff event with ID: {dropOffID}");
                break;
        }
    }
}