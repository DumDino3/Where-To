using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RideRequestDatabaseSO", menuName = "Database/Ride Request Database")]
public class RideRequestDatabaseSO : ScriptableObject
{
    public List<RideRequestEntry> entries = new List<RideRequestEntry>();
    private Dictionary<string, RideRequestEntry> lookup;

    //Build lookup on enable, default value pair <name, entry>
    private void OnEnable()
    {
        lookup = new Dictionary<string, RideRequestEntry>();
        foreach (RideRequestEntry entry in entries)
        {
            if (!lookup.TryAdd(entry.requestName, entry))
                Debug.LogWarning($"RideRequestDatabaseSO: Duplicate request name {entry.requestName}");
        }

        //Just to check duplicate ID, then clear list
        Dictionary<string, RideRequestEntry> idCheck = new Dictionary<string, RideRequestEntry>();
        foreach (RideRequestEntry entry in entries)
        {
            if (!lookup.TryAdd(entry.requestId, entry))
                Debug.LogWarning($"DialoguePoolDatabaseSO: Duplicate pool ID {entry.requestId}");
        }
        idCheck.Clear();
    }

    //Embedded search function
    public RideRequestEntry? Search(string name)
    {
        if (lookup.TryGetValue(name, out RideRequestEntry entry))
            return entry;

        return null;
    }

    //Embedded search all function
    public List<RideRequestEntry> GetAll()
    {
        return entries;
    }
}