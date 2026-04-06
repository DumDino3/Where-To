using System.Collections.Generic;
using UnityEngine;

public class PooledRequest : MonoBehaviour
{
    [SerializeField] private RideRequestDatabaseSO requestDatabase;
    private List<RideRequestEntry> requestEntries;

    private const string RIDEREQUEST_DB_PATH = "SO/Asset/RideRequestDatabaseSO";
}