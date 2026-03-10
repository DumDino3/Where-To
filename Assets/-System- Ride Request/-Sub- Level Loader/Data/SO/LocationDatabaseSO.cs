using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LocationDatabaseSO", menuName = "Data/Location Database")]
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

    //Embedded search function
    public LocationEntry? Search(string id)
    {
        return lookup.TryGetValue(id, out LocationEntry entry) ? entry : null;
    }

    //Embedded search all function
    public List<LocationEntry> GetAll()
    {
        return entries;
    }
}