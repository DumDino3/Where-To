using System.Collections.Generic;
using UnityEngine;

public class PooledRequest : MonoBehaviour
{
    private const string REQUEST_DB_PATH = "SO/Asset/RideRequestDatabaseSO";

    public static PooledRequest Instance { get; private set;}

    public List<RideRequestEntry> PooledRequests => pooledRequest;

    private RideRequestDatabaseSO requestDatabase;
    private List<RideRequestEntry> pooledRequest;

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

}