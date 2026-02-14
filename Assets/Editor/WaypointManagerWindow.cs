using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class WaypointManagerWindow : EditorWindow
{
    [MenuItem ("Tools/Waypoint Editor")]

    public static void Open()
    {
        GetWindow<WaypointManagerWindow>();
    }

    public Transform waypointRoot;

    private void OnGUI()
    {
        SerializedObject obj = new SerializedObject(this);

        EditorGUILayout.PropertyField(obj.FindProperty("waypointRoot"));

        if (waypointRoot == null)
        {
            EditorGUILayout.HelpBox("Root transform must be assigned", MessageType.Warning);
        }
        else
        {
            EditorGUILayout.BeginVertical("box");
            DrawButtons();
            EditorGUILayout.EndVertical();

        }

        obj.ApplyModifiedProperties();
    }

    private void DrawButtons()
    {
        if(GUILayout.Button("Create Waypoint"))
        {
            CreateWaypoint();
        }

        if (GUILayout.Button("Create Left Turn"))
        {
            CreateLeftTurn();
        }

        if (GUILayout.Button("Create Right Turn"))
        {
            CreateRightTurn();
        }

        if (GUILayout.Button("Connect Manual"))
        {
            ConnectManual();
        }

        if (GUILayout.Button("Connect Right Turn"))
        {
            ConnectRightTurn();
        }

        if (GUILayout.Button("Connect Left Turn"))
        {
            ConnectLeftTurn();
        }
    }


    private void CreateWaypoint()
    {
        GameObject waypointObject = new GameObject("Waypoint" + waypointRoot.childCount, typeof(Waypoint));
        waypointObject.transform.SetParent(waypointRoot, false);

        Waypoint waypoint = waypointObject.GetComponent<Waypoint>();

        if (waypointRoot.childCount > 1)
        {
            Waypoint currentWaypoint = Selection.activeGameObject.GetComponent<Waypoint>();

            currentWaypoint.connectedWaypoints.Add(waypoint);
            waypoint.connectedWaypoints.Add(currentWaypoint);

            waypoint.transform.position = currentWaypoint.transform.position;
            waypoint.transform.forward = currentWaypoint.transform.forward;
        }

        Selection.activeObject = waypoint.gameObject;
    }

    private void CreateLeftTurn()
    {
        GameObject waypointObject = new GameObject("Waypoint" + waypointRoot.childCount, typeof(Waypoint));
        waypointObject.transform.SetParent(waypointRoot, false);

        Waypoint waypoint = waypointObject.GetComponent<Waypoint>();

        if (waypointRoot.childCount > 1)
        {
            Waypoint currentWaypoint = Selection.activeGameObject.GetComponent<Waypoint>();
            waypoint.isIntersection = true;
            currentWaypoint.isIntersection = true;

            currentWaypoint.leftTurn = waypoint.transform;
            waypoint.rightTurn = currentWaypoint.transform;

            waypoint.transform.position = currentWaypoint.transform.position;   
            waypoint.transform.forward = currentWaypoint.transform.forward;
        }

        Selection.activeObject = waypoint.gameObject;
    }

    private void CreateRightTurn()
    {
        GameObject waypointObject = new GameObject("Waypoint" + waypointRoot.childCount, typeof(Waypoint));
        waypointObject.transform.SetParent(waypointRoot, false);

        Waypoint waypoint = waypointObject.GetComponent<Waypoint>();

        if (waypointRoot.childCount > 1)
        {
            Waypoint currentWaypoint = Selection.activeGameObject.GetComponent<Waypoint>();
            waypoint.isIntersection = true;
            currentWaypoint.isIntersection = true;

            currentWaypoint.rightTurn = waypoint.transform;
            waypoint.leftTurn = currentWaypoint.transform;

            waypoint.transform.position = currentWaypoint.transform.position;
            waypoint.transform.forward = currentWaypoint.transform.forward;
        }

        Selection.activeObject = waypoint.gameObject;
    }

    private void ConnectManual()
    {
        // Get all selected game objects that have Waypoint components
        Waypoint[] selectedWaypoints = Selection.GetFiltered<Waypoint>(SelectionMode.Deep);

        if (selectedWaypoints.Length < 2)
        {
            EditorUtility.DisplayDialog("Info", "Please select at least 2 waypoints to connect", "OK");
            return;
        }

        // Connect each waypoint to all other selected waypoints
        for (int i = 0; i < selectedWaypoints.Length; i++)
        {
            Waypoint currentWaypoint = selectedWaypoints[i];

            for (int j = 0; j < selectedWaypoints.Length; j++)
            {
                Waypoint targetWaypoint = selectedWaypoints[j];

                // Skip if it's the same waypoint or if targetWaypoint is already in the list
                if (currentWaypoint == targetWaypoint) continue;
                if (currentWaypoint.connectedWaypoints.Contains(targetWaypoint)) continue;

                // Add to connected waypoints
                currentWaypoint.connectedWaypoints.Add(targetWaypoint);
            }
        }
    }

        private void ConnectRightTurn()
    {
        Waypoint[] selectedWaypoints = Selection.GetFiltered<Waypoint>(SelectionMode.Deep);

        if (selectedWaypoints.Length != 2)
        {
            EditorUtility.DisplayDialog("Info", "Please select exactly 2 waypoints", "OK");
            return;
        }

        // First selected is the one getting the right turn, second is the right turn target
        Waypoint first = selectedWaypoints[0];
        Waypoint second = selectedWaypoints[1];

        first.rightTurn = second.transform;
        second.leftTurn = first.transform;

        first.isIntersection = true;
        second.isIntersection = true;
    }

    private void ConnectLeftTurn()
    {
        Waypoint[] selectedWaypoints = Selection.GetFiltered<Waypoint>(SelectionMode.Deep);

        if (selectedWaypoints.Length != 2)
        {
            EditorUtility.DisplayDialog("Info", "Please select exactly 2 waypoints", "OK");
            return;
        }

        // First selected is the one getting the left turn, second is the left turn target
        Waypoint first = selectedWaypoints[0];
        Waypoint second = selectedWaypoints[1];

        first.leftTurn = second.transform;
        second.rightTurn = first.transform;

        first.isIntersection = true;
        second.isIntersection = true;
    }
}
