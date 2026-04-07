using System.Collections.Generic;
using UnityEngine;

public class PooledRequest : MonoBehaviour
{
    private const string REQUEST_DB_PATH = "SO/Asset/RideRequestDatabaseSO";
    private const string LOCATION_DB_PATH = "SO/Asset/LocationDatabaseSO";
    private const string DIALOGUE_POOL_DB_PATH = "SO/Asset/DialoguePoolDatabaseSO";

    public static PooledRequest Instance { get; private set;}

    public List<RideRequestEntry> PooledRequests => pooledRequest;

    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        EnsureRequestDatabase();

        if (requestDatabase != null)
        {
            pooledRequest = requestDatabase.GetAll();
            ResolveEntryIds();
        }
        else
        {
            pooledRequest = new List<RideRequestEntry>();
            Debug.LogWarning($"PooledRequest: failed to load RideRequestDatabaseSO at '{REQUEST_DB_PATH}'");
        }
    }

    private void OnValidate()
    {
        EnsureRequestDatabase();
    }

    private void EnsureRequestDatabase()
    {
        if (requestDatabase != null)
            return;

        requestDatabase = Resources.Load<RideRequestDatabaseSO>(REQUEST_DB_PATH);
    }

    private void ResolveEntryIds()
    {
        if (pooledRequest == null || pooledRequest.Count == 0)
            return;

        var locationDatabase = Resources.Load<LocationDatabaseSO>(LOCATION_DB_PATH);
        if (locationDatabase == null)
            Debug.LogWarning($"PooledRequest: failed to load LocationDatabaseSO at '{LOCATION_DB_PATH}'");

        var dialoguePoolDatabase = Resources.Load<DialoguePoolDatabaseSO>(DIALOGUE_POOL_DB_PATH);
        if (dialoguePoolDatabase == null)
            Debug.LogWarning($"PooledRequest: failed to load DialoguePoolDatabaseSO at '{DIALOGUE_POOL_DB_PATH}'");

        for (int i = 0; i < pooledRequest.Count; i++)
        {
            var entry = pooledRequest[i];

            if (!string.IsNullOrWhiteSpace(entry.spawnId) && locationDatabase != null)
            {
                var spawnEntry = locationDatabase.SearchByName(entry.spawnId);
                if (spawnEntry.HasValue)
                    entry.spawnId = spawnEntry.Value.id;
            }

            if (!string.IsNullOrWhiteSpace(entry.destinationId) && locationDatabase != null)
            {
                var destinationEntry = locationDatabase.SearchByName(entry.destinationId);
                if (destinationEntry.HasValue)
                    entry.destinationId = destinationEntry.Value.id;
            }

            if (!string.IsNullOrWhiteSpace(entry.dialoguePoolId) && dialoguePoolDatabase != null)
            {
                var poolEntry = dialoguePoolDatabase.SearchByName(entry.dialoguePoolId);
                if (poolEntry.HasValue)
                    entry.dialoguePoolId = poolEntry.Value.poolId;
            }

            pooledRequest[i] = entry;
        }
    }

    [SerializeField] private RideRequestDatabaseSO requestDatabase;
    private List<RideRequestEntry> pooledRequest;
}