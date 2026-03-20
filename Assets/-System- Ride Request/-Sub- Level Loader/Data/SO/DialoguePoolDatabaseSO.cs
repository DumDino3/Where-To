using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialoguePoolDatabaseSO", menuName = "Database/Dialogue Pool Database")]
public class DialoguePoolDatabaseSO : ScriptableObject
{
    public List<DialoguePoolEntry> entries = new List<DialoguePoolEntry>();
    private Dictionary<string, DialoguePoolEntry> lookup;

    //Build lookup on enable, default value pair <name, entry>
    private void OnEnable()
    {
        lookup = new Dictionary<string, DialoguePoolEntry>();
        foreach (DialoguePoolEntry entry in entries)
        {
            if (!lookup.TryAdd(entry.poolName, entry))
                Debug.LogWarning($"DialoguePoolDatabaseSO: Duplicate pool name {entry.poolName}");
        }

        //Just to check duplicate ID, then clear list
        Dictionary<string, DialoguePoolEntry> idCheck = new Dictionary<string, DialoguePoolEntry>();
        foreach (DialoguePoolEntry entry in entries)
        {
            if (!lookup.TryAdd(entry.poolId, entry))
                Debug.LogWarning($"DialoguePoolDatabaseSO: Duplicate pool ID {entry.poolId}");
        }
        idCheck.Clear();
    }

    public DialoguePoolEntry? Search(string name)
    {
        if (lookup.TryGetValue(name, out DialoguePoolEntry entry))
            return entry;

        return null;
    }

    //Embedded search all function
    public List<DialoguePoolEntry> GetAll()
    {
        return entries;
    }
}