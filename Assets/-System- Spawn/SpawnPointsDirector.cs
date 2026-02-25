using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[ExecuteAlways]
public class SpawnPointsDirector : MonoBehaviour
{
    [Header("Director Settings")]
    [SerializeField] private bool debugLogging = true;
    [SerializeField] private string idPrefix = "SP_";
    
    [Header("Parent Settings")]
    [SerializeField] private Transform spawnPointsParent;
    
    [Header("Spawn Points Registry")]
    [SerializeField] private List<SpawnPoint> registeredSpawnPoints = new List<SpawnPoint>();
    
    private static SpawnPointsDirector _instance;
    public static SpawnPointsDirector Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<SpawnPointsDirector>();
            }
            return _instance;
        }
    }
    
    public List<SpawnPoint> RegisteredSpawnPoints => registeredSpawnPoints.ToList();
    public int SpawnPointCount => registeredSpawnPoints.Count;
    public Transform SpawnPointsParent => spawnPointsParent;
    
    private void Awake()
    {
        _instance = this;
        Debug.Log("[SpawnPointsDirector] Awake called");
        
        if (spawnPointsParent == null)
        {
            CreateDefaultParent();
        }
        
        RefreshRegistry();
    }
    
    private void OnEnable()
    {
        Debug.Log("[SpawnPointsDirector] OnEnable called");
        
        if (!Application.isPlaying)
        {
            if (_instance == null)
            {
                _instance = this;
            }
            
            if (spawnPointsParent == null)
            {
                CreateDefaultParent();
            }
            
            RefreshRegistry();
        }
    }
    
    private void CreateDefaultParent()
    {
        GameObject parentGO = new GameObject("SpawnPoints");
        parentGO.transform.SetParent(this.transform);
        spawnPointsParent = parentGO.transform;
        
        if (debugLogging)
        {
            Debug.Log("[SpawnPointsDirector] Created default spawn points parent");
        }
        
        #if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            UnityEditor.EditorUtility.SetDirty(this);
        }
        #endif
    }
    
    public void SetSpawnPointsParent(Transform newParent)
    {
        spawnPointsParent = newParent;
        
        foreach (SpawnPoint sp in registeredSpawnPoints)
        {
            if (sp != null)
            {
                sp.transform.SetParent(spawnPointsParent);
            }
        }
        
        if (debugLogging)
        {
            Debug.Log($"[SpawnPointsDirector] Changed spawn points parent to: {newParent?.name ?? "null"}");
        }
        
        #if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            UnityEditor.EditorUtility.SetDirty(this);
        }
        #endif
    }
    
    public void RegisterSpawnPoint(SpawnPoint spawnPoint)
    {
        if (spawnPoint == null) return;
        
        registeredSpawnPoints.RemoveAll(sp => sp == null);
        
        if (!registeredSpawnPoints.Contains(spawnPoint))
        {
            Debug.Log($"[SpawnPointsDirector] Registering: {spawnPoint.gameObject.name}");
            
            registeredSpawnPoints.Add(spawnPoint);
            
            if (spawnPointsParent != null)
            {
                spawnPoint.transform.SetParent(spawnPointsParent);
            }
            
            AssignIDsToAllSpawnPoints();
            
            #if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                UnityEditor.EditorUtility.SetDirty(this);
            }
            #endif
        }
    }
    
    public void DeregisterSpawnPoint(SpawnPoint spawnPoint)
    {
        if (spawnPoint == null) return;
        
        if (registeredSpawnPoints.Contains(spawnPoint))
        {
            Debug.Log($"[SpawnPointsDirector] Deregistering: {spawnPoint.gameObject.name}");
            registeredSpawnPoints.Remove(spawnPoint);
            AssignIDsToAllSpawnPoints();
            
            #if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                UnityEditor.EditorUtility.SetDirty(this);
            }
            #endif
        }
    }
    
    private void AssignIDsToAllSpawnPoints()
    {
        registeredSpawnPoints.RemoveAll(sp => sp == null);
        
        for (int i = 0; i < registeredSpawnPoints.Count; i++)
        {
            SpawnPoint spawnPoint = registeredSpawnPoints[i];
            if (spawnPoint != null)
            {
                string newID = $"{idPrefix}{i:D3}";
                spawnPoint.AssignID(newID);
            }
        }
        
        #if UNITY_EDITOR
        UnityEditor.SceneView.RepaintAll();
        #endif
    }
    
    public SpawnPoint GetSpawnPointByID(string id)
    {
        return registeredSpawnPoints.FirstOrDefault(sp => sp != null && sp.ID == id);
    }
    
    public SpawnPoint GetSpawnPointByIndex(int index)
    {
        if (index >= 0 && index < registeredSpawnPoints.Count)
        {
            return registeredSpawnPoints[index];
        }
        return null;
    }
    
    public List<SpawnPoint> GetSpawnPointsInRadius(Vector3 position, float radius)
    {
        return registeredSpawnPoints.Where(sp => sp != null && 
            Vector3.Distance(sp.transform.position, position) <= radius).ToList();
    }
    
    public SpawnPoint GetNearestSpawnPoint(Vector3 position)
    {
        if (registeredSpawnPoints.Count == 0) return null;
        
        return registeredSpawnPoints.Where(sp => sp != null).OrderBy(sp => 
            Vector3.Distance(sp.transform.position, position)).FirstOrDefault();
    }
    
    public void SetSpawnPointActive(string spawnPointID, bool active)
    {
        SpawnPoint spawnPoint = GetSpawnPointByID(spawnPointID);
        if (spawnPoint != null)
        {
            spawnPoint.SetActive(active);
            
            if (debugLogging)
            {
                Debug.Log($"[SpawnPointsDirector] Set spawn point {spawnPointID} active: {active}");
            }
        }
        else
        {
            Debug.LogWarning($"[SpawnPointsDirector] Spawn point with ID {spawnPointID} not found!");
        }
    }
    
    public void SetAllSpawnPointsActive(bool active)
    {
        foreach (SpawnPoint spawnPoint in registeredSpawnPoints)
        {
            if (spawnPoint != null)
            {
                spawnPoint.SetActive(active);
            }
        }
        
        if (debugLogging)
        {
            Debug.Log($"[SpawnPointsDirector] Set all spawn points active: {active}");
        }
    }
    
    public void ClearAllSpawnPoints()
    {
        registeredSpawnPoints.Clear();
        
        if (debugLogging)
        {
            Debug.Log("[SpawnPointsDirector] Cleared all spawn points from registry");
        }
        
        #if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            UnityEditor.EditorUtility.SetDirty(this);
        }
        #endif
    }
    
    [ContextMenu("Refresh Registry")]
    public void RefreshRegistry()
    {
        registeredSpawnPoints.Clear();
        SpawnPoint[] existingSpawnPoints = FindObjectsOfType<SpawnPoint>();
        
        foreach (SpawnPoint spawnPoint in existingSpawnPoints)
        {
            if (spawnPoint != null)
            {
                registeredSpawnPoints.Add(spawnPoint);
                if (spawnPointsParent != null)
                {
                    spawnPoint.transform.SetParent(spawnPointsParent);
                }
            }
        }
        
        AssignIDsToAllSpawnPoints();
        Debug.Log($"[SpawnPointsDirector] Refreshed registry with {registeredSpawnPoints.Count} spawn points");
    }
    
    [ContextMenu("Log All Spawn Points")]
    public void LogAllSpawnPoints()
    {
        Debug.Log($"[SpawnPointsDirector] Total spawn points: {registeredSpawnPoints.Count}");
        
        for (int i = 0; i < registeredSpawnPoints.Count; i++)
        {
            SpawnPoint sp = registeredSpawnPoints[i];
            if (sp != null)
            {
                Debug.Log($"  {i}. ID: {sp.ID}, Name: {sp.gameObject.name}, Active: {sp.IsActive}");
            }
        }
    }
    
    [ContextMenu("Force Reassign IDs")]
    public void ForceReassignIDs()
    {
        AssignIDsToAllSpawnPoints();
    }
}