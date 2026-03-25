using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LocationDatabaseSO", menuName = "Database/Location Database")]
public class LocationDatabaseSO : SearchableDatabaseSO<LocationEntry>
{
    public List<LocationEntry> entries = new List<LocationEntry>();

    protected override List<LocationEntry> Entries => entries;
    protected override string GetNameKey(LocationEntry entry) => entry.name;
    protected override string GetIDKey(LocationEntry entry) => entry.id;
}