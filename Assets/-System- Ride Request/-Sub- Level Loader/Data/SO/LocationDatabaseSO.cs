using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LocationDatabaseSO", menuName = "Database/Location Database")]
public class LocationDatabaseSO : ScriptableObject
{
    public List<LocationEntry> entries = new List<LocationEntry>();
    private Dictionary<string, LocationEntry> lookup;

    //Build lookup on enable, default value pair <name, entry>
    private void OnEnable()
    {
        lookup = new Dictionary<string, LocationEntry>();

        foreach (LocationEntry entry in entries)
        {
            if (!lookup.TryAdd(entry.name, entry))
                Debug.LogWarning($"LocationDatabaseSO: Duplicate Name {entry.name}");
        }

        //Just to check duplicate ID, then clear list
        Dictionary<string, LocationEntry> idCheck = new Dictionary<string, LocationEntry>();
        foreach (LocationEntry entry in entries)
        {
            if (!lookup.TryAdd(entry.id, entry))
                Debug.LogWarning($"DialoguePoolDatabaseSO: Duplicate pool ID {entry.id}");
        }
        idCheck.Clear();
    }

    public LocationEntry? Search(string name)
    {
        if (lookup.TryGetValue(name, out LocationEntry entry))
            return entry;

        return null;
    }

    //Embedded search all function
    public List<LocationEntry> GetAll()
    {
        return entries;
    }
}