using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RideRequestDatabaseSO", menuName = "Database/Ride Request Database")]
public class RideRequestDatabaseSO : ScriptableObject
{
    public List<RideRequestEntry> entries = new List<RideRequestEntry>();
    private Dictionary<string, RideRequestEntry> lookup;

    //Build lookup on enable, entries are created via Creator tool
    private void OnEnable()
    {
        lookup = new Dictionary<string, RideRequestEntry>();
        foreach (RideRequestEntry entry in entries)
        {
            if (!lookup.TryAdd(entry.requestId, entry))
                Debug.LogWarning($"RideRequestDatabaseSO: Duplicate request ID {entry.requestId}");
        }
    }

    //Embedded search function
    public RideRequestEntry? Search(string requestId)
    {
        return lookup.TryGetValue(requestId, out RideRequestEntry entry) ? entry : null;
    }

    //Embedded search all function
    public List<RideRequestEntry> GetAll()
    {
        return entries;
    }
}