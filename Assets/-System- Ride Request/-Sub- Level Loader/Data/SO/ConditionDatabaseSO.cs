using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ConditionDatabaseSO", menuName = "Database/Condition Database")]
public class ConditionDatabaseSO : ScriptableObject
{
    public List<ConditionEntry> entries = new List<ConditionEntry>();
    private Dictionary<string, ConditionEntry> lookup;

    //Build lookup on enable, entries are created via Creator tool
    private void OnEnable()
    {
        lookup = new Dictionary<string, ConditionEntry>();
        foreach (ConditionEntry entry in entries)
        {
            if (!lookup.TryAdd(entry.conditionId, entry))
                Debug.LogWarning($"ConditionDatabaseSO: Duplicate ID {entry.conditionId}");
        }
    }

    //Embedded search function
    public ConditionEntry? Search(string conditionId)
    {
        return lookup.TryGetValue(conditionId, out ConditionEntry entry) ? entry : null;
    }

    //Embedded search all function
    public List<ConditionEntry> GetAll()
    {
        return entries;
    }
}