using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnPaceManager : MonoBehaviour
{
    //hmm if any id being set to 00 or null te system must register random 
    

    // ======= Variables ========================================================================================================

    
    private readonly RequestFlowController _flow = new RequestFlowController();
    private readonly RequestScheduler _schedule = new RequestScheduler();
    
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
        _schedule.EnsureSegment(timeSeg);
    }
    
    
    public void PushDataIntoQueue(string requestID)
    {
        _schedule.TryAddRawQuestId(requestID);
    }
    

    

    public void PushDataIntoLive(int currentTimeSeg)
    {
        foreach (int coreId in _schedule.GetSortedCoreIDForSegment(currentTimeSeg))
        {
            var (duration, pickup, dropoff) = RequestIDParser.ParseCoreID(coreId);
            OnRequestSpawned?.Invoke(duration, pickup, dropoff);
            currentActiveQuests += 1;
        }
        
    }

    public void RequestActive(int pickup, int dropOff)
    {
        if (!_flow.TryActivate(pickup, dropOff))
            return;
        OnSpawnPointToggle?.Invoke(pickup);
    }


    private void OnCabinStateUpdated(CabinStateMachine.CabinStates state)
    {
        var result = _flow.HandleCabinState(state);

        if (result.ToggleSpawnPoint)
            OnSpawnPointToggle?.Invoke(result.ToggleId);

        if (result.QuestCompleted)
            OnRequestDone?.Invoke();
    }

    #endregion
    
    
    private void TimeSegsChanged(int currentTimeSeg)
    {
        currentTime = currentTimeSeg;
        PushDataIntoLive(currentTimeSeg);
    }
    
    
}