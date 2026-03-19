using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LocationDatabaseSO", menuName = "Database/Location Database")]
public class LocationDatabaseSO : ScriptableObject
{
    public List<LocationEntry> entries = new List<LocationEntry>();
    private Dictionary<string, LocationEntry> lookup;

    //Load csv on enable, entries are parsed from CSV via Importer
    private void OnEnable()
    {
        lookup = new Dictionary<string, LocationEntry>();
        foreach (LocationEntry entry in entries)
        {
            if (!lookup.TryAdd(entry.id, entry))
                Debug.LogWarning($"LocationDatabaseSO: Duplicate ID {entry.id}");
        }
    }

    public LocationEntry? Search(string name)
    {
        return lookup.TryGetValue(name, out LocationEntry entry) ? entry : null;
    }

    //Embedded search all function
    public List<LocationEntry> GetAll()
    {
        return entries;
    }
}