using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RideRequestDatabaseSO", menuName = "Database/Ride Request Database")]
public class RideRequestDatabaseSO : SearchableDatabaseSO<RideRequestEntry>
{
    public List<RideRequestEntry> entries = new List<RideRequestEntry>();

    protected override List<RideRequestEntry> Entries => entries;
    protected override string GetNameKey(RideRequestEntry entry) => entry.requestName;
    protected override string GetIDKey(RideRequestEntry entry) => entry.requestId;
}