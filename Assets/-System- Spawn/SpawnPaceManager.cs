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

    [Header("Current Time")] public int currentTime;
    
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

    private void OnEnable()
    {
        DayCycleManager.onDayStarted += BeginDay;
        DayCycleManager.onTimeSegsChanged += TimeSegsChanged;
    }
    
    private void OnDisable()
    {
        DayCycleManager.onDayStarted -= BeginDay;
        DayCycleManager.onTimeSegsChanged -= TimeSegsChanged;
    }


    private void Start()
    {
        InitializingQuest();
    }


    public void OverrideCurrentPoints()
    {
        //this will override the current active point but what about disable it ?
    }

    public void PushDataIntoQueue(int currentTimeSeg)
    {
        //This will be the worker who will fget the data from the quest for each time segment
        //and also check if the data require specific segment it would push that into queue instead of random from pool
        //but will this performance taxing ?
        
        //but this script should only push but will not display them all at once right ?
        //Like it should put all of them into that interval queue all call it one by one
        //we can use some priority system to always push a quest higher to display sooner than any quests
        
        // note:it should wait till the state of the taxi reach idle state to push the quest (for now)
    }

    private void InitializingQuest()
    {
        //This will intialize the pool of that day quest
    }
    

    private void BeginDay()
    {
        
    }

    private void TimeSegsChanged(int currentTimeSeg)
    {
        currentTime = currentTimeSeg;
    }
    

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