using UnityEngine;
using System.Collections.Generic;

public class PooledRequestIDCompiler : MonoBehaviour
{
    private PooledRequest pooledRequest;

    private void Awake()
    {
        pooledRequest = PooledRequest.Instance ?? GameObject.FindAnyObjectByType<PooledRequest>();
        if (pooledRequest == null)
        Debug.LogWarning("PooledRequestIDCompiler: failed to find PooledRequest instance.");
    }

    public List<string> CompileIds()
    {
        var compiledIds = new List<string>();
        var entries = pooledRequest?.PooledRequests;

        if (entries == null)
            return compiledIds;

        foreach (var entry in entries)
        {
            string id = entry.duration + entry.spawnId + entry.destinationId + entry.priority + entry.timeSegment;
            compiledIds.Add(id);
        }
        return compiledIds;
    }
}