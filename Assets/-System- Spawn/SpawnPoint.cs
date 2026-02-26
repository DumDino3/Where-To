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


    private void OnValidate()
    {
        string targetName = $"Spawn Point {spawnPointID}";
        if (gameObject.name != targetName)
        {
            gameObject.name = targetName;
        }

        onSpawnPointChanged?.Invoke();
    }

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
    }


    #region Gizmos
    private void OnDrawGizmos()
    {
        DrawSphereColliderGizmo();
    }
    private void DrawSphereColliderGizmo()
    {
        SphereCollider sphereCollider = GetComponent<SphereCollider>();
        if (sphereCollider == null)
            return;
        Vector3 worldCenter = transform.TransformPoint(sphereCollider.center);
        
        float maxScale = Mathf.Max(
            Mathf.Abs(transform.lossyScale.x),
            Mathf.Abs(transform.lossyScale.y),
            Mathf.Abs(transform.lossyScale.z)
        );
        float worldRadius = sphereCollider.radius * maxScale;
        Gizmos.color = gizmoColor;
        Gizmos.DrawSphere(worldCenter, worldRadius);
        Gizmos.color = gizmoWireColor;
        Gizmos.DrawWireSphere(worldCenter, worldRadius);
    }
    

    #endregion

}