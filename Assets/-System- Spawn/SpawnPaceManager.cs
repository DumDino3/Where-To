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
    private readonly SegmentQueue _perSegmentsQueue = new SegmentQueue();
    
    
    [Header("Request Manager")]
    public int currentActiveQuests = 0;
    public int perPriorityActiveLimit = 1; //this is the active limit for each priority level, it will be used to control the spawn of quests based on their priority.
    private bool hasActiveQuest = false;
    


    // ======= Events ========================================================================================================
    
    public static event Action<int> OnSpawnPointToggle; //this command the director
    public static event Action OnCabinStateChanged;
    public static event  Action<int,int,int> OnRequestSpawned;
    public static event Action OnRequestDone;
    
    
    // ======= Subscribing to ========================================================================================================



    #region Events Initialization
    private void OnEnable()
    {
        DayCycleManager.onTimeSegsChanged += TimeSegsChanged;
        RandomRequestGen.onQuestGenerated += PushDataIntoQueue;
        DayCycleManager.initializeTimeSeg += InitilizeTimeSegmentDict;
        
        //Quest Flagger
        LiveQuestInstance.onQuestAccepted += RequestActive;
        LiveQuestInstance.onQuestExpired += RequestExpired;
        //cabin state
        CabinStateMachine.OnCabinStateChanged += OnCabinStateUpdated;
        

        //subscribe to override mediator
        SpawnPointOverrideMediator.OnSpawnPointOverride += OverrideCurrentPoints;


    }
    
    private void OnDisable()
    {
        DayCycleManager.onTimeSegsChanged -= TimeSegsChanged;
        RandomRequestGen.onQuestGenerated -= PushDataIntoQueue;
        DayCycleManager.initializeTimeSeg -= InitilizeTimeSegmentDict;
        
        //Quest Flagger
        LiveQuestInstance.onQuestAccepted -= RequestActive;
        LiveQuestInstance.onQuestExpired -= RequestExpired;
        //cabin state
        CabinStateMachine.OnCabinStateChanged -= OnCabinStateUpdated;
        
        //unsubscribe to override mediator
        SpawnPointOverrideMediator.OnSpawnPointOverride -= OverrideCurrentPoints;
        
    }
    

    #endregion
  


    private void Start()
    {
        SetRule();
    }

    private void SetRule()
    {
    }


    
    #region Progression System

    public void OverrideCurrentPoints(int spawnPointID)
    {
        OnSpawnPointToggle?.Invoke(spawnPointID);
    }

    public void InitilizeTimeSegmentDict(int timeSeg)
    {
        _schedule.EstablishSegments(timeSeg);
    }
    
    
    public void PushDataIntoQueue(string requestID)
    {
        Debug.Log($"pushed in: {requestID}");
        _schedule.TryAddRawQuestId(requestID);
    }
    

    

    public void PushDataIntoLive(int currentTimeSeg)
    {
        var sortedIds = _schedule.GetSortedCoreIDForSegment(currentTimeSeg).ToList();
    
        //this to ensure that when a segmenet is empty it will not trigger the pull live request function and cause error
        if (sortedIds.Count == 0) {
            return;
        }

        foreach (int coreId in sortedIds)
        {
            _perSegmentsQueue.PushCoreIDintoQueue(coreId);
        }
        PullLiveRequestFromQueue();
    }
    public void PullLiveRequestFromQueue()
    {
        if (currentActiveQuests <= perPriorityActiveLimit && hasActiveQuest == false)
        {
            //to-do: hey the ochestor shouldn't be responsible for this, fix later.
            
            
            for (int i = 0; i < perPriorityActiveLimit - currentActiveQuests; i++)
            {
                int coreId = _perSegmentsQueue.PopCoreIDFromQueue();
                if (coreId == 0) return; // this means the queue is empty, so we should stop trying to pull from it.
                var (duration, pickup, dropoff) = RequestIDParser.ParseCoreID(coreId);
                OnRequestSpawned?.Invoke(duration, pickup, dropoff);
                currentActiveQuests += 1;
            }
        }
        return;
    }
    
    public void RequestExpired()
    {
        currentActiveQuests -= 1;
        PullLiveRequestFromQueue();
    }
    
    public void RequestCompleted()
    {
        currentActiveQuests = 0;
        PullLiveRequestFromQueue();
    }

    public void RequestActive(int pickup, int dropOff)
    {
        if (!_flow.TryActivate(pickup, dropOff))
            return;
        OnSpawnPointToggle?.Invoke(pickup);
        hasActiveQuest = true;
    }


    private void OnCabinStateUpdated(CabinStateMachine.CabinStates state)
    {
        var result = _flow.HandleCabinState(state);

        if (result.ToggleSpawnPoint)
            OnSpawnPointToggle?.Invoke(result.ToggleId);

        if (result.QuestCompleted)
            OnRequestDone?.Invoke();
            RequestCompleted();
            hasActiveQuest = false;
    }

    #endregion
    
    
    private void TimeSegsChanged(int currentTimeSeg)
    {
        PushDataIntoLive(currentTimeSeg);
    }
    
    
}