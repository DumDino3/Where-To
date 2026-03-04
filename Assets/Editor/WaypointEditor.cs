using UnityEngine;
using UnityEditor;
using System.Collections.ObjectModel;

[InitializeOnLoad()]

static public class WaypointEditor
{
    [DrawGizmo(GizmoType.NonSelected | GizmoType.Selected | GizmoType.Pickable | GizmoType.Active)]
    public static void OnDrawSceneGizmo(Waypoint waypoint, GizmoType gizmoType)
    {

        Gizmos.color = (gizmoType & GizmoType.Selected) != 0 ? Color.yellow : Color.whiteSmoke;
        Gizmos.DrawSphere(waypoint.transform.position, 0.5f);

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
