using System;
using UnityEngine;

[ExecuteAlways]
public class SpawnPoint : MonoBehaviour
{
    public static event Action onSpawnPointChanged;
    [Header("Spawn Point Settings")]
    [SerializeField] public int spawnPointID;
    [SerializeField] private float sphereCastRadius = 1f;

    [Header("Gizmo Settings")]
    [SerializeField] private Color gizmoColor = new Color(0f, 1f, 0f, 0.25f);
    [SerializeField] private Color gizmoWireColor = Color.green;
    [SerializeField] private Color selectedGizmoColor = new Color(1f, 0.92f, 0.016f, 0.4f); // Yellow

    #region Hieu Adjustment for true name display
    //-----------------------------------------------------------
    // private void OnValidate()
    // {
    //     string targetName = $"Spawn Point {spawnPointID}";
    //     if (gameObject.name != targetName)
    //     {
    //         gameObject.name = targetName;
    //     }

    //     onSpawnPointChanged?.Invoke();
    // }
    //-----------------------------------------------------------
    private const string LOCATION_DB_PATH = "SO/Asset/LocationDatabaseSO";
    private LocationDatabaseSO locationDatabase;

    private void EnsureLocationDatabase()
    {
        if (locationDatabase == null)
            locationDatabase = Resources.Load<LocationDatabaseSO>(LOCATION_DB_PATH);
    }

    private void OnValidate()
    {
        //Load database and set a trueName variable
        EnsureLocationDatabase();
        string trueName = null;

        //Search by id from location database and extract entry's name into trueName
        if (locationDatabase != null)
        {
            var entry = locationDatabase.SearchByID(spawnPointID);
            if (entry.HasValue)
                trueName = entry.Value.name;
        }

        //Overide current name
        if (gameObject.name != trueName)
            gameObject.name = trueName;

        onSpawnPointChanged?.Invoke();
    }
    #endregion

    private void Update()
    {
        if (!Application.isPlaying)
        {
            SphereManipulate();
        }
    }

    private void SphereManipulate()
    {
        SphereCollider sphereCollider = GetComponent<SphereCollider>();
        if (sphereCollider == null)
        {
            sphereCollider = gameObject.AddComponent<SphereCollider>();
        }
        sphereCollider.radius = sphereCastRadius;
        sphereCollider.isTrigger = true; // Prevents physics bumps in editor
    }

    #region Gizmos
    private void OnDrawGizmos()
    {
        DrawSphereColliderGizmo(false);
    }

    private void OnDrawGizmosSelected()
    {
        DrawSphereColliderGizmo(true);
    }

    private void DrawSphereColliderGizmo(bool isSelected)
    {
        SphereCollider sphereCollider = GetComponent<SphereCollider>();
        if (sphereCollider == null) return;

        Vector3 worldCenter = transform.TransformPoint(sphereCollider.center);
        
        float maxScale = Mathf.Max(
            Mathf.Abs(transform.lossyScale.x),
            Mathf.Abs(transform.lossyScale.y),
            Mathf.Abs(transform.lossyScale.z)
        );
        float worldRadius = sphereCollider.radius * maxScale;

        // Swap colors if selected
        Gizmos.color = isSelected ? selectedGizmoColor : gizmoColor;
        Gizmos.DrawSphere(worldCenter, worldRadius);
        
        Gizmos.color = isSelected ? Color.yellow : gizmoWireColor;
        Gizmos.DrawWireSphere(worldCenter, worldRadius);
    }
    #endregion
}