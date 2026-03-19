using UnityEngine;
using System.Collections.Generic;

public class PooledRequestIDCompiler : MonoBehaviour
{
    public RideRequestDatabaseSO database;
    public LocationDatabaseSO locationDatabase;

    public List<string> CompileIds()
    {
        var compiledIds = new List<string>();

        foreach (var entry in database.GetAll())
        {
            var spawnEntry = locationDatabase.Search(entry.spawnId);
            var destinationEntry = locationDatabase.Search(entry.destinationId);

            string spawnId = spawnEntry?.id ?? entry.spawnId;
            string destinationId = destinationEntry?.id ?? entry.destinationId;

            string id = entry.duration + spawnId + destinationId + entry.priority + entry.timeSegment;
            compiledIds.Add(id);
        }

        return compiledIds;
    }
}