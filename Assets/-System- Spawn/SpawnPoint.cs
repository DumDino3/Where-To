using UnityEngine;

[ExecuteAlways]
public class SpawnPoint : MonoBehaviour
{
    [Header("Spawn Point Settings")]
    [SerializeField] private string spawnPointID;
    [SerializeField] private float sphereCastRadius = 1f;
    [SerializeField] private bool showGizmos = true;
    
    [Header("Activation")]
    [SerializeField] private bool isActive = true;
    [SerializeField] private bool debugLogging = false;
    
    public string ID => spawnPointID;
    public float SphereRadius => sphereCastRadius;
    public bool IsActive => isActive;
    
    private void Awake()
    {
        Debug.Log($"[SpawnPoint] Awake: {gameObject.name}");
        RegisterWithDirector();
    }
    
    private void OnEnable()
    {
        Debug.Log($"[SpawnPoint] OnEnable: {gameObject.name}");
        RegisterWithDirector();
    }
    
    private void OnDisable()
    {
        Debug.Log($"[SpawnPoint] OnDisable: {gameObject.name}");
        DeregisterFromDirector();
    }
    
    private void RegisterWithDirector()
    {
        SpawnPointsDirector director = FindObjectOfType<SpawnPointsDirector>();
        if (director != null)
        {
            Debug.Log($"[SpawnPoint] Registering {gameObject.name} with director");
            director.RegisterSpawnPoint(this);
        }
        else
        {
            Debug.LogWarning($"[SpawnPoint] No director found for {gameObject.name}");
        }
    }
    
    private void DeregisterFromDirector()
    {
        SpawnPointsDirector director = FindObjectOfType<SpawnPointsDirector>();
        if (director != null)
        {
            Debug.Log($"[SpawnPoint] Deregistering {gameObject.name} from director");
            director.DeregisterSpawnPoint(this);
        }
    }
    
    public void AssignID(string newID)
    {
        spawnPointID = newID;
        gameObject.name = $"SpawnPoint_{newID}";
        Debug.Log($"[SpawnPoint] ID assigned: {newID} to {gameObject.name}");
        
        #if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.SceneView.RepaintAll();
        }
        #endif
    }
    
    public void SetActive(bool active)
    {
        isActive = active;
        
        if (debugLogging)
        {
            Debug.Log($"[SpawnPoint] {gameObject.name} set active: {active}");
        }
        
        #if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.SceneView.RepaintAll();
        }
        #endif
    }
    
    public bool CheckSpherecast()
    {
        return Physics.CheckSphere(transform.position, sphereCastRadius);
    }
    
    public bool CheckSpherecast(LayerMask layerMask)
    {
        return Physics.CheckSphere(transform.position, sphereCastRadius, layerMask);
    }
    
    public Collider[] GetObjectsInSphere()
    {
        return Physics.OverlapSphere(transform.position, sphereCastRadius);
    }
    
    public Collider[] GetObjectsInSphere(LayerMask layerMask)
    {
        return Physics.OverlapSphere(transform.position, sphereCastRadius, layerMask);
    }
    
    private void OnDrawGizmos()
    {
        if (showGizmos)
        {
            // Green if active, red if inactive
            Gizmos.color = isActive ? Color.green : Color.red;
            Gizmos.DrawWireSphere(transform.position, sphereCastRadius);
            
            #if UNITY_EDITOR
            string displayID = string.IsNullOrEmpty(spawnPointID) ? "Unassigned" : spawnPointID;
            string status = isActive ? "ACTIVE" : "INACTIVE";
            UnityEditor.Handles.Label(transform.position + Vector3.up * (sphereCastRadius + 0.5f), 
                $"ID: {displayID}\n{status}");
            #endif
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sphereCastRadius);
        
        #if UNITY_EDITOR
        string displayID = string.IsNullOrEmpty(spawnPointID) ? "Unassigned" : spawnPointID;
        string status = isActive ? "ACTIVE" : "INACTIVE";
        UnityEditor.Handles.Label(transform.position + Vector3.up * (sphereCastRadius + 1f), 
            $"SpawnPoint\nID: {displayID}\nRadius: {sphereCastRadius:F1}\n{status}");
        #endif
    }
}