using UnityEngine;
using UnityEditor;
using System.Collections.ObjectModel;

[InitializeOnLoad()]

static public class WaypointEditor
{
    [DrawGizmo(GizmoType.NonSelected | GizmoType.Selected | GizmoType.Pickable | GizmoType.Active)]
    public static void OnDrawSceneGizmo(Waypoint waypoint, GizmoType gizmoType)
    {
        Gizmos.DrawSphere(waypoint.transform.position, 0.5f);

        Gizmos.color = Color.yellow;

        foreach (Waypoint connectedWaypoint in waypoint.connectedWaypoints)
        {
            if (connectedWaypoint != null)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(waypoint.transform.position, connectedWaypoint.transform.position);
            }
        }
        
        
    }
}
