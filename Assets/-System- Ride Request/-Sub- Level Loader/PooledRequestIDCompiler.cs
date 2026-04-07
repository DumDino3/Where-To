using UnityEngine;
using System.Collections.Generic;

public class PooledRequestIDCompiler : MonoBehaviour
{
    private PooledRequest pooledRequest;
    private LocationDatabaseSO locationDatabase;

    private const string LOCATION_DB_PATH = "SO/Asset/LocationDatabaseSO";

    private void Awake()
    {
        EnsureDatabases();
    }

    private void OnValidate()
    {
        EnsureDatabases();
    }

    private void EnsureDatabases()
    {
        if (pooledRequest == null)
            pooledRequest = PooledRequest.Instance ?? GameObject.FindAnyObjectByType<PooledRequest>();

        if (locationDatabase == null)
            locationDatabase = Resources.Load<LocationDatabaseSO>(LOCATION_DB_PATH);

        if (pooledRequest == null || locationDatabase == null)
            Debug.LogWarning($"PooledRequestIDCompiler missing refs: pooledRequest={pooledRequest}, locationDatabase={locationDatabase}");
    }

    public List<string> CompileIds()
    {
        var compiledIds = new List<string>();
        var entries = pooledRequest?.PooledRequests;

        if (entries == null)
            return compiledIds;

        foreach (var entry in entries)
        {
            var spawnEntry = locationDatabase.SearchByName(entry.spawnId);
            var destinationEntry = locationDatabase.SearchByName(entry.destinationId);

            string spawnId = spawnEntry?.id ?? entry.spawnId;
            string destinationId = destinationEntry?.id ?? entry.destinationId;

            string id = entry.duration + spawnId + destinationId + entry.priority + entry.timeSegment;
            compiledIds.Add(id);
        }
    
        return compiledIds;
    }
}