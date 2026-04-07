using System.Collections.Generic;
using UnityEngine;

public class PooledRequest : MonoBehaviour
{
    public static PooledRequest Instance { get; private set; }

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
        pooledRequest = requestDatabase.GetAll();
    }

    [SerializeField] private RideRequestDatabaseSO requestDatabase;
    private List<RideRequestEntry> pooledRequest;
}