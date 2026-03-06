using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnPaceManager : MonoBehaviour
{
    //hmm if any id being set to 00 or null te system must register random 
    

    // ======= Variables ========================================================================================================


    public List<int> requestID = new List<int>();
    public Dictionary<int, List<int>> requestDict = new Dictionary<int, List<int>>();
    
    [Header("Runtime Data")] 
    public int CurrentID;
    
    [Header("Current Time")] public int currentTime;
    
    [Header("Quest Manager")]
    public int currentActiveQuests = 0;

    private bool hasActiveQuest = false;
    
    
    private CabinStateMachine.CabinStates previousState;
    
    
    [Header("Parsed Data")]
    [SerializeField] private int durationID;
    [SerializeField] private int pickupID;
    [SerializeField] private int dropOffID;
    [SerializeField] private int priorityID;
    [SerializeField] private int timeSegID;
    
    private int questLimits = 30; 
    
    [SerializeField] private CabinStateMachine.CabinStates currentState;


    // ======= Events ========================================================================================================

    public static event Action<int> OnSpawnPointToggle; //this command the director
    public static event Action OnCabinStateChanged;
    public static event  Action<int,int,int> OnRequestSpawned;
    public static event Action OnRequestDone;
    
   
    
    
    
    // ======= Events ========================================================================================================



    #region Events Initialization
    private void OnEnable()
    {
        DayCycleManager.onTimeSegsChanged += TimeSegsChanged;
        RandomRequestGen.onQuestGenerated += PushDataIntoQueue;
        DayCycleManager.initializeTimeSeg += InitilizeTimeSegmentDict;
        
        //Quest Flagger
        LiveQuestInstance.onQuestAccepted += RequestActive;
        //cabin state
        CabinStateMachine.OnCabinStateChanged += OnCabinStateUpdated;

    }
    
    private void OnDisable()
    {
        DayCycleManager.onTimeSegsChanged -= TimeSegsChanged;
        RandomRequestGen.onQuestGenerated -= PushDataIntoQueue;
        DayCycleManager.initializeTimeSeg -= InitilizeTimeSegmentDict;
        
        //Quest Flagger
        LiveQuestInstance.onQuestAccepted -= RequestActive;
        //cabin state
        CabinStateMachine.OnCabinStateChanged -= OnCabinStateUpdated;
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
                OnRequestSpawned?.Invoke(duration, pickup, dropoff);
                currentActiveQuests += 1;
            }
        }
    }

    public void RequestActive(int pickup, int dropOff)
    {
        if (hasActiveQuest) return;

        pickupID = pickup;
        dropOffID = dropOff;
        hasActiveQuest = true;

        OnSpawnPointToggle?.Invoke(pickupID); 
    }


    private void OnCabinStateUpdated(CabinStateMachine.CabinStates state)
    {
        currentState = state;

        if (!hasActiveQuest) return;

        if (currentState == CabinStateMachine.CabinStates.Idling)
        {
            OnSpawnPointToggle?.Invoke(pickupID);
        }
        else if (currentState == CabinStateMachine.CabinStates.Picked)
        {
            OnSpawnPointToggle?.Invoke(dropOffID);
        }
        else if (currentState == CabinStateMachine.CabinStates.Dropped)
        {
            hasActiveQuest = false;
            OnRequestDone?.Invoke();
        }
    }

    #endregion
    

    
    
    private void TimeSegsChanged(int currentTimeSeg)
    {
        currentTime = currentTimeSeg;
        PushDataIntoLive(currentTimeSeg);
    }
    
    

    private (int dur, int pick, int drop) ParsingId(string travelIDRaw)
    {
        ReadOnlySpan<char> travelIDChar = travelIDRaw.AsSpan();
        return
        (
            int.Parse(travelIDChar.Slice(0, 3)),  // duration
            int.Parse(travelIDChar.Slice(3, 3)),  // pickup
            int.Parse(travelIDChar.Slice(6, 3))   // dropoff
        );
    }
    
    
    //This part is to parse the data from the str
    private void TimeSegParser(string travelIDRaw)
    {
        ReadOnlySpan<char> travelIDChar = travelIDRaw.AsSpan();
        int timeSegment = int.Parse(travelIDChar.Slice(11, 2));
        int requestID   = int.Parse(travelIDChar.Slice(0, 11)); 
    
        if (requestDict.ContainsKey(timeSegment))
            requestDict[timeSegment].Add(requestID);
    }
}