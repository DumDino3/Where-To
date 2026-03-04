using System;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPaceManager : MonoBehaviour
{
    //hmm if any id being set to 00 or null te system must register random 

    public List<string> dataPackage = new List<string>();
    
    
    [Header("Runtime Data")] 
    public int CurrentID;
    
    [Header("Current Time")] public int currentTime;
    
    
    [Header("Parsed Data")]
    [SerializeField] private int durationID;
    [SerializeField] private int pickupID;
    [SerializeField] private int dropOffID;
    [SerializeField] private int priorityID;
    
   
 

    
    
    private int questLimits = 30; 
    
    [SerializeField] private CurrentTaxiState currentState;

    public static event Action<int> OnSpawnPointChanged; //this command the director
    
    public static event Action<int> OnChunkChanged;

    private void OnEnable()
    {
        DayCycleManager.onDayStarted += BeginDay;
        DayCycleManager.onTimeSegsChanged += TimeSegsChanged;
        RandomRequestGen.onQuestGenerated += PushDataIntoQueue;
    }
    
    private void OnDisable()
    {
        DayCycleManager.onDayStarted -= BeginDay;
        DayCycleManager.onTimeSegsChanged -= TimeSegsChanged;
        RandomRequestGen.onQuestGenerated -= PushDataIntoQueue;
    }


    private void Start()
    {
    }

    
    #region Progression System

    public void OverrideCurrentPoints()
    {
        //this will override the current active point but what about disable it ?
    }

    
    public void PushDataIntoQueue(string questID)
    {
            dataPackage.Add(questID);
    }


    public void PushDataIntoLive(int currentTimeSeg)
    {
        int randomIndex = UnityEngine.Random.Range(0, dataPackage.Count);
        ParsingId(dataPackage[randomIndex]);

    }

    #endregion

    private void BeginDay()
    {
        
    }

    private void TimeSegsChanged(int currentTimeSeg)
    {
        currentTime = currentTimeSeg;
        PushDataIntoLive(currentTimeSeg);
    }
    

    enum CurrentTaxiState
    {
        PickUp,
        DropOff,
    }



    [ContextMenu("Parse Travel ID")]
    private void ParsingId(string travelIDRaw)
    {
        //convert the string to span so that we can slice it without creating new string instances, improving performance <3
        ReadOnlySpan<char> travelIDChar =  travelIDRaw.AsSpan();

        durationID = int.Parse(travelIDChar.Slice(0,3));
        pickupID = int.Parse(travelIDChar.Slice(3,3));
        dropOffID = int.Parse(travelIDChar.Slice(6,3));
        priorityID = int.Parse(travelIDChar.Slice(9,2));

    }

    

    private void TestSpawnEvent()
    {
        switch (currentState)
        {
            case CurrentTaxiState.PickUp:
                CurrentID = pickupID;
                break;

            case CurrentTaxiState.DropOff:
                CurrentID = dropOffID;
                break;
        }
    }
    
    [ContextMenu("Test Spawn Event")]
    private void TestSpawnEventContext()
    {
        TestSpawnEvent();
        OnSpawnPointChanged?.Invoke(CurrentID);
    }
}