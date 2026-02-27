using UnityEditor;
using UnityEngine;

public class WaypointManagerWindow : EditorWindow
{
    [MenuItem ("Tools/Waypoint Editor")]
    public static void Open() => GetWindow<WaypointManagerWindow>();

    public Transform waypointRoot;

    private void OnGUI()
    {
        SerializedObject obj = new SerializedObject(this);
        EditorGUILayout.PropertyField(obj.FindProperty("waypointRoot"));

        if (waypointRoot != null)
        {
            EditorGUILayout.BeginVertical("box");
            if (GUILayout.Button("Create Waypoint")) CreateWaypoint();
            if (GUILayout.Button("Connect Waypoints (Selection)")) ConnectManual();
            EditorGUILayout.EndVertical();
        }
        obj.ApplyModifiedProperties();
    }

    private void CreateWaypoint()
    {
        GameObject go = new GameObject("Waypoint" + waypointRoot.childCount, typeof(Waypoint));
        go.transform.SetParent(waypointRoot, false);
        Waypoint wp = go.GetComponent<Waypoint>();

        if (Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<Waypoint>())
        {
            Waypoint cur = Selection.activeGameObject.GetComponent<Waypoint>();
            cur.connectedWaypoints.Add(wp);
            wp.connectedWaypoints.Add(cur);
            wp.transform.position = cur.transform.position;
            wp.transform.forward = cur.transform.forward;
        }
        Selection.activeObject = go;
    }

    
    static void ConnectManual()
    {
        Waypoint[] selected = Selection.GetFiltered<Waypoint>(SelectionMode.Deep);
        if (selected.Length < 2) return;

        for (int i = 0; i < selected.Length; i++)
        {
            for (int j = 0; j < selected.Length; j++)
            {
                if (i != j && !selected[i].connectedWaypoints.Contains(selected[j]))
                    selected[i].connectedWaypoints.Add(selected[j]);
            }
        }
    }
    static void RemoveConnction()
    {
        Waypoint[] selected = Selection.GetFiltered<Waypoint>(SelectionMode.Deep);
        if (selected.Length < 2) return;

        for (int i = 0; i < selected.Length; i++)
        {
            for (int j = 0; j < selected.Length; j++)
            {
                if (i != j && !selected[i].connectedWaypoints.Remove(selected[j]))
                    selected[i].connectedWaypoints.Remove(selected[j]);
            }
        }
    }

    [MenuItem("CONTEXT/Waypoint/Connect waypoints")]
    static void CreateWaypointMenu()
    {
        ConnectManual();
    }
    [MenuItem("CONTEXT/Waypoint/Destroy the bond between waypoints")]
    static void DestroyWaypointMenu()
    {
        RemoveConnction();
    }
    
}