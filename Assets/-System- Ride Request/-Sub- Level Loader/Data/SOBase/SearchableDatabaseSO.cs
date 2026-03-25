using System.Collections.Generic;
using UnityEngine;

public abstract class SearchableDatabaseSO<TEntry> : ScriptableObject where TEntry : struct
{
    private Dictionary<string, TEntry> nameLookup;
    private Dictionary<string, TEntry> idLookup;

    protected abstract List<TEntry> Entries { get; }
    protected abstract string GetNameKey(TEntry entry);
    protected abstract string GetIDKey(TEntry entry);

    protected virtual void OnEnable()
    {
        RebuildLookups();
    }

    public TEntry? SearchByName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return null;

        EnsureLookups();
        if (nameLookup.TryGetValue(name, out TEntry entry))
            return entry;

        return null;
    }

    public TEntry? SearchByID(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return null;

        EnsureLookups();
        if (idLookup.TryGetValue(id, out TEntry entry))
            return entry;

        return null;
    }

    public TEntry? SearchByID(int id)
    {
        return SearchByID(id.ToString("D3"));
    }

    public List<TEntry> GetAll()
    {
        return Entries;
    }

    private void EnsureLookups()
    {
        if (nameLookup == null || idLookup == null)
            RebuildLookups();
    }

    private void RebuildLookups()
    {
        nameLookup = new Dictionary<string, TEntry>();
        idLookup = new Dictionary<string, TEntry>();

        List<TEntry> entries = Entries;
        if (entries == null)
            return;

        foreach (TEntry entry in entries)
        {
            string nameKey = GetNameKey(entry);
            if (!string.IsNullOrWhiteSpace(nameKey) && !nameLookup.TryAdd(nameKey, entry))
                Debug.LogWarning($"{GetType().Name}: Duplicate name key {nameKey}");

            string idKey = GetIDKey(entry);
            if (!string.IsNullOrWhiteSpace(idKey) && !idLookup.TryAdd(idKey, entry))
                Debug.LogWarning($"{GetType().Name}: Duplicate ID key {idKey}");
        }
    }
}