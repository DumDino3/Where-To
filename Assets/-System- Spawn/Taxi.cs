using UnityEngine;

public class Taxi : MonoBehaviour
{
    [Header("Detection Settings")]
    [SerializeField] private float detectionRadius = 2f;
    [SerializeField] private LayerMask spawnPointLayer = -1;
    [SerializeField] private bool debugLogging = true;
    
    [Header("Visual Feedback")]
    [SerializeField] private bool showDetectionRadius = true;
    [SerializeField] private Color detectionColor = Color.blue;
    [SerializeField] private Color selectedColor = Color.cyan;
    
    private SpawnPaceManager paceManager;
    private string lastDetectedSpawnPointID;
    private float lastDetectionTime;
    private const float DETECTION_COOLDOWN = 1f; // Prevent rapid re-detection
    
    public string LastDetectedSpawnPointID => lastDetectedSpawnPointID;
    public bool IsNearSpawnPoint => !string.IsNullOrEmpty(lastDetectedSpawnPointID);
    
    private void Start()
    {
        paceManager = FindObjectOfType<SpawnPaceManager>();
        if (paceManager == null)
        {
            Debug.LogError("[Taxi] No SpawnPaceManager found!");
        }
        else if (debugLogging)
        {
            Debug.Log("[Taxi] Connected to SpawnPaceManager");
        }
    }
    
    private void Update()
    {
        CheckForSpawnPoints();
    }
    
    private void CheckForSpawnPoints()
    {
        // Get all colliders in detection radius
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius, spawnPointLayer);
        
        bool foundActiveSpawnPoint = false;
        
        foreach (Collider col in colliders)
        {
            SpawnPoint spawnPoint = col.GetComponent<SpawnPoint>();
            if (spawnPoint != null && spawnPoint.IsActive)
            {
                // Check if taxi is within the spawn point's sphere cast radius
                float distanceToSpawnPoint = Vector3.Distance(transform.position, spawnPoint.transform.position);
                
                if (distanceToSpawnPoint <= spawnPoint.SphereRadius)
                {
                    foundActiveSpawnPoint = true;
                    
                    // Only trigger if it's a new spawn point or enough time has passed
                    bool isNewSpawnPoint = lastDetectedSpawnPointID != spawnPoint.ID;
                    bool cooldownExpired = Time.time - lastDetectionTime > DETECTION_COOLDOWN;
                    
                    if (isNewSpawnPoint || cooldownExpired)
                    {
                        lastDetectedSpawnPointID = spawnPoint.ID;
                        lastDetectionTime = Time.time;
                        NotifyPaceManager(spawnPoint.ID);
                        
                        if (debugLogging)
                        {
                            Debug.Log($"[Taxi] Reached spawn point: {spawnPoint.ID} at distance: {distanceToSpawnPoint:F2}");
                        }
                    }
                    break; // Exit early if we found an active spawn point
                }
            }
        }
        
        // Reset last detected if we're not near any active spawn points
        if (!foundActiveSpawnPoint)
        {
            if (!string.IsNullOrEmpty(lastDetectedSpawnPointID) && debugLogging)
            {
                Debug.Log($"[Taxi] Left spawn point: {lastDetectedSpawnPointID}");
            }
            lastDetectedSpawnPointID = null;
        }
    }
    
    private void NotifyPaceManager(string spawnPointID)
    {
        if (paceManager != null)
        {
            paceManager.OnTaxiReachedSpawnPoint(spawnPointID);
        }
        else
        {
            Debug.LogWarning("[Taxi] Cannot notify pace manager - not found!");
        }
    }
    
    // Public method to get current spawn point state info
    
    // Manual detection trigger for testing
    [ContextMenu("Force Detection Check")]
    public void ForceDetectionCheck()
    {
        CheckForSpawnPoints();
    }
    
    [ContextMenu("Clear Last Detection")]
    public void ClearLastDetection()
    {
        lastDetectedSpawnPointID = null;
        lastDetectionTime = 0f;
        Debug.Log("[Taxi] Cleared last detection");
    }
    
    private void OnDrawGizmos()
    {
        if (showDetectionRadius)
        {
            Gizmos.color = detectionColor;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
            
            #if UNITY_EDITOR
            // Show current state info
            if (paceManager != null)
            {
                string info = $"State: {paceManager.CurrentState}\nAt: {lastDetectedSpawnPointID ?? "None"}";
                UnityEditor.Handles.Label(transform.position + Vector3.up * 2f, info);
            }
            #endif
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        if (showDetectionRadius)
        {
            Gizmos.color = selectedColor;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
            
        }
    }
    
    // Events for external systems
    public System.Action<string> OnSpawnPointEntered;
    public System.Action<string> OnSpawnPointExited;
    
    private void TriggerSpawnPointEntered(string spawnPointID)
    {
        OnSpawnPointEntered?.Invoke(spawnPointID);
    }
    
    private void TriggerSpawnPointExited(string spawnPointID)
    {
        OnSpawnPointExited?.Invoke(spawnPointID);
    }
}