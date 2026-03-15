using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WorldState))]
public class WorldStateEditor : Editor
{
    private TagDatabaseSO tagDb;
    private int addTagIndex = 0;

    public override void OnInspectorGUI()
    {
        WorldState worldState = (WorldState)target;

        DrawDefaultInspector();
        EditorGUILayout.Space(10);

        EditorGUILayout.LabelField("WORLD STATE TAGS", EditorStyles.boldLabel);
        tagDb = (TagDatabaseSO)EditorGUILayout.ObjectField("Tag DB", tagDb, typeof(TagDatabaseSO), false);

        if (tagDb == null)
        {
            EditorGUILayout.HelpBox("Assign a TagDatabaseSO to add tags from dropdown.", MessageType.Info);
            return;
        }

        List<string> availableTags = new List<string>();
        foreach (string tag in tagDb.tags)
        {
            if (!worldState.Tags.Contains(tag))
                availableTags.Add(tag);
        }

        for (int i = worldState.Tags.Count - 1; i >= 0; i--)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(worldState.Tags[i]);
            if (GUILayout.Button("x", GUILayout.Width(24)))
            {
                Undo.RecordObject(worldState, "Remove World State Tag");
                worldState.Tags.RemoveAt(i);
                EditorUtility.SetDirty(worldState);
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.Space(5);

        if (availableTags.Count > 0)
        {
            if (addTagIndex >= availableTags.Count)
                addTagIndex = 0;

            EditorGUILayout.BeginHorizontal();
            addTagIndex = EditorGUILayout.Popup("Add Tag", addTagIndex, availableTags.ToArray());

            if (GUILayout.Button("+ Add", GUILayout.Width(60)))
            {
                Undo.RecordObject(worldState, "Add World State Tag");
                worldState.Tags.Add(availableTags[addTagIndex]);
                EditorUtility.SetDirty(worldState);
            }
            EditorGUILayout.EndHorizontal();
        }
        else
        {
            EditorGUILayout.HelpBox("All tags from the database are already added.", MessageType.None);
        }
    }
}