using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialoguePoolDatabaseSO", menuName = "Data/Dialogue Pool Database")]
public class DialoguePoolDatabaseSO : ScriptableObject
{
    public List<DialoguePoolEntry> entries = new List<DialoguePoolEntry>();
    private Dictionary<string, DialoguePoolEntry> lookup;

    //Build lookup on enable, entries are parsed from CSV via Importer
    private void OnEnable()
    {
        lookup = new Dictionary<string, DialoguePoolEntry>();
        foreach (DialoguePoolEntry entry in entries)
        {
            if (!lookup.TryAdd(entry.poolId, entry))
                Debug.LogWarning($"DialoguePoolDatabaseSO: Duplicate pool ID {entry.poolId}");
        }
    }

    //Embedded search function
    public DialoguePoolEntry? Search(string poolId)
    {
        return lookup.TryGetValue(poolId, out DialoguePoolEntry entry) ? entry : null;
    }

    //Embedded search all function
    public List<DialoguePoolEntry> GetAll()
    {
        return entries;
    }
}