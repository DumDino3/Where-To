using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPaceManager : MonoBehaviour
{
    [Header("Pace Settings")]
    [Tooltip("Total session duration in seconds (0 or less = infinite).")]
    [SerializeField] private float sessionDuration = 0f;

    [Tooltip("Dropoff time limit in seconds (0 or less = no timeout).")]
    [SerializeField] private float dropoffTimeLimit = 30f;

    [Header("Randomization Settings")]
    [Tooltip("Minimum number of simultaneous pickup points.")]
    [SerializeField] private int minPickupCount = 1;

    [Tooltip("Maximum number of simultaneous pickup points.")]
    [SerializeField] private int maxPickupCount = 3;

    [Tooltip("Allow the current dropoff spawn point to also be a pickup at the same time.")]
    [SerializeField] private bool allowDropoffAlsoPickup = false;

    [Header("Auto Start")]
    [SerializeField] private bool autoStartQuest = true;
    [SerializeField] private float autoStartDelay = 2f;

    [Header("Debug")]
    [SerializeField] private bool debugLogging = true;

    // State
    public enum SpawnPointState
    {
        Pickup,
        Dropoff,
        Inactive
    }

    private SpawnPointState currentState = SpawnPointState.Inactive;

    private SpawnPointsDirector director;
    private Coroutine sessionCoroutine;
    private Coroutine dropoffTimerCoroutine;
    private bool isSessionActive = false;

    // IDs
    private readonly List<string> allSpawnPointIDs = new List<string>();
    private readonly HashSet<string> activePickupIDs = new HashSet<string>();
    private string activeDropoffID;

    // Timers
    private float sessionStartTime;
    private float pickupStartTime;
    private float dropoffStartTime;

    // Events
    public System.Action<SpawnPointState> OnStateChanged;
    public System.Action OnSessionCompleted;
    public System.Action OnSessionStarted;
    public System.Action OnQuestStarted;
    public System.Action OnQuestCompleted;
    public System.Action OnPickupReached;
    public System.Action OnDropoffReached;
    public System.Action OnDropoffTimeout;

    // Public read-only API
    public SpawnPointState CurrentState => currentState;
    public bool IsSessionActive => isSessionActive;

    public IReadOnlyCollection<string> ActivePickupIDs => activePickupIDs;
    public string ActiveDropoffID => activeDropoffID;

    public float DropoffTimeLimit => dropoffTimeLimit;
    public float SessionElapsedTime => isSessionActive ? Time.time - sessionStartTime : 0f;
    public float StateElapsedTime => currentState switch
    {
        SpawnPointState.Pickup  => Time.time - pickupStartTime,
        SpawnPointState.Dropoff => Time.time - dropoffStartTime,
        _                       => 0f
    };
    public float DropoffRemainingTime => currentState == SpawnPointState.Dropoff && dropoffTimeLimit > 0f
        ? Mathf.Max(0f, dropoffTimeLimit - (Time.time - dropoffStartTime))
        : 0f;

    private void Start()
    {
        director = SpawnPointsDirector.Instance;
        if (director == null)
        {
            Debug.LogError("[SpawnPaceManager] No SpawnPointsDirector found!");
            enabled = false;
            return;
        }

        InitializeSystem();

        if (autoStartQuest)
        {
            StartCoroutine(AutoStartQuestCoroutine());
        }
    }

    #region Initialization

    private void InitializeSystem()
    {
        ReadAllSpawnPointIDs();
        SetAllSpawnPointsInactiveInternal();

        // Clamp pickup counts
        minPickupCount = Mathf.Max(1, minPickupCount);
        maxPickupCount = Mathf.Max(minPickupCount, maxPickupCount);

        if (debugLogging)
        {
            Debug.Log($"[SpawnPaceManager] Initialized with {allSpawnPointIDs.Count} spawn points. " +
                      $"PickupCount: {minPickupCount}-{maxPickupCount}");
        }
    }

    private IEnumerator AutoStartQuestCoroutine()
    {
        yield return new WaitForSeconds(autoStartDelay);
        StartQuest();
    }

    private void ReadAllSpawnPointIDs()
    {
        allSpawnPointIDs.Clear();

        var spawnPoints = director.RegisteredSpawnPoints;
        foreach (SpawnPoint sp in spawnPoints)
        {
            if (sp != null && !string.IsNullOrEmpty(sp.ID))
            {
                allSpawnPointIDs.Add(sp.ID);
            }
        }

        if (debugLogging)
        {
            Debug.Log($"[SpawnPaceManager] Read {allSpawnPointIDs.Count} spawn point IDs: " +
                      $"[{string.Join(", ", allSpawnPointIDs)}]");
        }
    }

    #endregion

    #region Quest / Session Control

    public void StartQuest()
    {
        if (allSpawnPointIDs.Count < 2)
        {
            ReadAllSpawnPointIDs();
            if (allSpawnPointIDs.Count < 2)
            {
                Debug.LogError("[SpawnPaceManager] Need at least 2 spawn points to start quest!");
                return;
            }
        }

        // Reset state
        SetAllSpawnPointsInactiveInternal();
        StartSession();

        OnQuestStarted?.Invoke();

        if (debugLogging)
        {
            Debug.Log("[SpawnPaceManager] Quest started");
        }
    }

    public void EndQuest()
    {
        StopSession();
        SetAllSpawnPointsInactiveInternal();

        OnQuestCompleted?.Invoke();

        if (debugLogging)
        {
            Debug.Log("[SpawnPaceManager] Quest ended - all spawn points inactive");
        }
    }

    public void StartSession()
    {
        StopSession(); // ensure clean

        isSessionActive = true;
        sessionStartTime = Time.time;

        sessionCoroutine = StartCoroutine(SessionLoopCoroutine());
        OnSessionStarted?.Invoke();

        if (debugLogging)
        {
            Debug.Log($"[SpawnPaceManager] Session started (sessionDuration: {sessionDuration}s, dropoffLimit: {dropoffTimeLimit}s)");
        }
    }

    public void StopSession()
    {
        if (sessionCoroutine != null)
        {
            StopCoroutine(sessionCoroutine);
            sessionCoroutine = null;
        }

        if (dropoffTimerCoroutine != null)
        {
            StopCoroutine(dropoffTimerCoroutine);
            dropoffTimerCoroutine = null;
        }

        isSessionActive = false;
        currentState = SpawnPointState.Inactive;
        SetAllSpawnPointsInactiveInternal();
        OnStateChanged?.Invoke(currentState);

        if (debugLogging)
        {
            Debug.Log("[SpawnPaceManager] Session stopped");
        }
    }

    private IEnumerator SessionLoopCoroutine()
    {
        // 1) Start in PICKUP (no time limit)
        SetStateToPickupRandom();

        // Wait until we leave pickup (taxi reaches one of the pickup points)
        while (isSessionActive && currentState == SpawnPointState.Pickup)
        {
            yield return null;
        }

        if (!isSessionActive)
            yield break;

        // 2) Then cycle between DROP-OFF and new PICKUPs until session duration (if any) is over
        float startTime = Time.time;

        while (isSessionActive && (sessionDuration <= 0f || Time.time - startTime < sessionDuration))
        {
            // In dropoff: wait until taxi reaches dropoff or timeout causes state change
            while (isSessionActive && currentState == SpawnPointState.Dropoff)
            {
                yield return null;
            }

            if (!isSessionActive)
                break;

            // After dropoff, choose new random pickup(s)
            SetStateToPickupRandom();

            while (isSessionActive && currentState == SpawnPointState.Pickup)
            {
                yield return null;
            }

            if (!isSessionActive)
                break;
        }

        // End of session
        isSessionActive = false;
        currentState = SpawnPointState.Inactive;
        SetAllSpawnPointsInactiveInternal();
        OnStateChanged?.Invoke(currentState);
        OnSessionCompleted?.Invoke();

        if (debugLogging)
        {
            Debug.Log("[SpawnPaceManager] Session completed");
        }
    }

    #endregion

    #region State Logic

    /// <summary>
    /// Choose random pickup points (1..N) and activate them. No time limit in this state.
    /// </summary>
    private void SetStateToPickupRandom()
    {
        // Stop dropoff timer if running
        if (dropoffTimerCoroutine != null)
        {
            StopCoroutine(dropoffTimerCoroutine);
            dropoffTimerCoroutine = null;
        }

        // Deactivate everything
        SetAllSpawnPointsInactiveInternal();

        // Choose random count
        int availableCount = allSpawnPointIDs.Count;
        if (!allowDropoffAlsoPickup && !string.IsNullOrEmpty(activeDropoffID))
        {
            availableCount = Mathf.Max(0, availableCount - 1);
        }

        int targetPickupCount = Mathf.Clamp(Random.Range(minPickupCount, maxPickupCount + 1), 1, availableCount);

        // Build a candidate list
        List<string> candidates = new List<string>(allSpawnPointIDs);
        if (!allowDropoffAlsoPickup && !string.IsNullOrEmpty(activeDropoffID))
        {
            candidates.Remove(activeDropoffID);
        }

        Shuffle(candidates);

        activePickupIDs.Clear();

        for (int i = 0; i < targetPickupCount && i < candidates.Count; i++)
        {
            string id = candidates[i];
            activePickupIDs.Add(id);
            ActivateSpawnPoint(id);
        }

        // Clear dropoff while in pickup state
        activeDropoffID = null;

        currentState = SpawnPointState.Pickup;
        pickupStartTime = Time.time;
        OnStateChanged?.Invoke(currentState);

        if (debugLogging)
        {
            Debug.Log($"[SpawnPaceManager] State: PICKUP - Active pickups: [{string.Join(", ", activePickupIDs)}]");
        }
    }

    /// <summary>
    /// Choose a random dropoff point and activate only that.
    /// </summary>
    private void SetStateToDropoffRandom()
    {
        // Deactivate everything
        SetAllSpawnPointsInactiveInternal();

        if (allSpawnPointIDs.Count == 0)
        {
            ReadAllSpawnPointIDs();
            if (allSpawnPointIDs.Count == 0)
            {
                Debug.LogError("[SpawnPaceManager] No spawn points available for dropoff!");
                return;
            }
        }

        // Choose random dropoff
        string newDropoffID = allSpawnPointIDs[Random.Range(0, allSpawnPointIDs.Count)];
        activeDropoffID = newDropoffID;

        ActivateSpawnPoint(activeDropoffID);

        // Clear pickups while in dropoff state
        activePickupIDs.Clear();

        currentState = SpawnPointState.Dropoff;
        dropoffStartTime = Time.time;
        OnStateChanged?.Invoke(currentState);

        if (dropoffTimeLimit > 0f)
        {
            dropoffTimerCoroutine = StartCoroutine(DropoffTimerCoroutine());
        }

        if (debugLogging)
        {
            Debug.Log($"[SpawnPaceManager] State: DROPOFF - Active dropoff: {activeDropoffID}");
        }
    }

    private IEnumerator DropoffTimerCoroutine()
    {
        yield return new WaitForSeconds(dropoffTimeLimit);

        if (isSessionActive && currentState == SpawnPointState.Dropoff)
        {
            OnDropoffTimeout?.Invoke();

            if (debugLogging)
            {
                Debug.Log("[SpawnPaceManager] Dropoff timeout - switching to new pickups");
            }

            SetStateToPickupRandom();
        }
    }

    private void SetAllSpawnPointsInactiveInternal()
    {
        foreach (string id in allSpawnPointIDs)
        {
            director.SetSpawnPointActive(id, false);
        }

        activePickupIDs.Clear();
        activeDropoffID = null;
    }

    private void ActivateSpawnPoint(string id)
    {
        director.SetSpawnPointActive(id, true);
    }

    #endregion

    #region Taxi Callbacks

    /// <summary>
    /// Called by Taxi when it reaches a spawn point.
    /// </summary>
    public void OnTaxiReachedSpawnPoint(string spawnPointID)
    {
        if (!isSessionActive)
            return;

        if (debugLogging)
        {
            Debug.Log($"[SpawnPaceManager] Taxi reached spawn point: {spawnPointID}");
        }

        // Reached any active pickup?
        if (currentState == SpawnPointState.Pickup && activePickupIDs.Contains(spawnPointID))
        {
            OnPickupReached?.Invoke();

            if (debugLogging)
            {
                Debug.Log("[SpawnPaceManager] Taxi reached pickup - switching to random dropoff");
            }

            // Move to DROP-OFF with a random dropoff location
            SetStateToDropoffRandom();
            return;
        }

        // Reached current dropoff?
        if (currentState == SpawnPointState.Dropoff && spawnPointID == activeDropoffID)
        {
            OnDropoffReached?.Invoke();

            if (debugLogging)
            {
                Debug.Log("[SpawnPaceManager] Taxi reached dropoff - switching to new random pickups");
            }

            // Move back to PICKUP with new random pickups
            SetStateToPickupRandom();
        }
    }

    #endregion

    #region Utilities

    // Fisher–Yates shuffle
    private static void Shuffle<T>(IList<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int j = Random.Range(i, list.Count);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    public int GetTotalSpawnPointCount() => allSpawnPointIDs.Count;
    public string[] GetAvailableSpawnPointIDs() => allSpawnPointIDs.ToArray();
    public bool IsSpawnPointRegistered(string id) => allSpawnPointIDs.Contains(id);

    #endregion
}