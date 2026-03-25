using UnityEngine;
using System.Collections.Generic;

public class PooledRequestIDCompiler : MonoBehaviour
{
    private RideRequestDatabaseSO database;
    private LocationDatabaseSO locationDatabase;

    private const string RIDEREQUEST_DB_PATH = "SO/Asset/RideRequestDatabaseSO";
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
        if (database == null)
            database = Resources.Load<RideRequestDatabaseSO>(RIDEREQUEST_DB_PATH);

        if (locationDatabase == null)
            locationDatabase = Resources.Load<LocationDatabaseSO>(LOCATION_DB_PATH);

        if (database == null || locationDatabase == null)
            Debug.LogWarning($"PooledRequestIDCompiler missing DB refs: database={database}, locationDatabase={locationDatabase}");
    }

    public List<string> CompileIds()
    {
        var compiledIds = new List<string>();

        foreach (var entry in database.GetAll())
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