using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnPaceManager : MonoBehaviour
{
    //hmm if any id being set to 00 or null te system must register random 


    public List<int> requestID = new List<int>();
    public Dictionary<int, List<int>> requestDict = new Dictionary<int, List<int>>();
    
    [Header("Runtime Data")] 
    public int CurrentID;
    
    [Header("Current Time")] public int currentTime;
    
    [Header("Quest Manager")]
    public int currentActiveQuests;
    
    
    
    [Header("Parsed Data")]
    [SerializeField] private int durationID;
    [SerializeField] private int pickupID;
    [SerializeField] private int dropOffID;
    [SerializeField] private int priorityID;
    [SerializeField] private int timeSegID;
    
    private int questLimits = 30; 
    
    [SerializeField] private CurrentTaxiState currentState;


    // ======= Events ========================================================================================================

    public static event Action<int> OnSpawnPointChanged; //this command the director
    public static event Action<int> OnChunkChanged;
    public static event  Action<int,int,int> OnRequestSpawned;
    
    // ======= Events ========================================================================================================



    #region Events Initialization
    private void OnEnable()
    {
        DayCycleManager.onTimeSegsChanged += TimeSegsChanged;
        RandomRequestGen.onQuestGenerated += PushDataIntoQueue;
        DayCycleManager.initializeTimeSeg += InitilizeTimeSegmentDict;
    }
    
    private void OnDisable()
    {
        DayCycleManager.onTimeSegsChanged -= TimeSegsChanged;
        RandomRequestGen.onQuestGenerated -= PushDataIntoQueue;
        DayCycleManager.initializeTimeSeg -= InitilizeTimeSegmentDict;
    }
    

    #endregion
  


    private void Start()
    {
        
    }

    
    #region Progression System

    public void OverrideCurrentPoints()
    {
        //this will override the current active point but what about disable it ?
    }

    public void InitilizeTimeSegmentDict(int timeSeg)
    {
        requestDict.Add(timeSeg,new List<int>());
    }

    
    public void PushDataIntoQueue(string requestID)
    {
        TimeSegParser(requestID);
    }


    public void PushDataIntoLive(int currentTimeSeg)
    {
        
        if (requestDict.TryGetValue(currentTimeSeg, out List<int> requestIDs))
        {
            var sortedPriority = requestIDs.OrderByDescending(singleID =>
            {
                string paddedID = singleID.ToString("D11");
                ReadOnlySpan<char> IDString = paddedID.AsSpan();
                int priority = int.Parse(IDString.Slice(9, 2));
                return priority;
            });
            
            foreach (int requestID in sortedPriority)
            {
                var (duration, pickup, dropoff) = ParsingId(requestID.ToString("D11"));
                OnRequestSpawned(duration, pickup, dropoff);
            }
        }
    }

    #endregion
    

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


    private (int dur, int pick, int drop) ParsingId(string travelIDRaw)
    {
        //convert the string to span so that we can slice it without creating new string instances, improving performance <3
        ReadOnlySpan<char> travelIDChar =  travelIDRaw.AsSpan();
        return
        (
        int.Parse(travelIDChar.Slice(0, 3)),
        int.Parse(travelIDChar.Slice(3, 3)),
        int.Parse(travelIDChar.Slice(6, 3))
        );
        
    }
    
    private void TimeSegParser(string travelIDRaw)
    {
        ReadOnlySpan<char> travelIDChar =  travelIDRaw.AsSpan();
        int timeSegment;
        int requestID;
        timeSegment = int.Parse(travelIDChar.Slice(11,2));
        requestID = int.Parse(travelIDChar.Slice(0,10));
        if (requestDict.ContainsKey(timeSegment))
        {
            requestDict[timeSegment].Add(requestID);
        }
        else if(timeSegment >= requestDict.Keys.Count)
        {
            //random
        }
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