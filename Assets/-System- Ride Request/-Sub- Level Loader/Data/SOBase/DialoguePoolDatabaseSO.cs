using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialoguePoolDatabaseSO", menuName = "Database/Dialogue Pool Database")]
public class DialoguePoolDatabaseSO : SearchableDatabaseSO<DialoguePoolEntry>
{
    public List<DialoguePoolEntry> entries = new List<DialoguePoolEntry>();

    protected override List<DialoguePoolEntry> Entries => entries;
    protected override string GetNameKey(DialoguePoolEntry entry) => entry.poolName;
    protected override string GetIDKey(DialoguePoolEntry entry) => entry.poolId;
}