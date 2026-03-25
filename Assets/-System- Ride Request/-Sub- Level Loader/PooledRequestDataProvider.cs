using System.Collections.Generic;
using UnityEngine;

public class PooledRequestDataProvider : MonoBehaviour
{
    private SpawnPaceManager paceManager; // Reference to the system that accepts quest IDs
    private PooledRequestIDCompiler idCompiler;

    private void Awake()
    {
        idCompiler = GameObject.FindAnyObjectByType<PooledRequestIDCompiler>();
        paceManager = FindAnyObjectByType<SpawnPaceManager>();
        PushAllToQueue(idCompiler.CompileIds());
    }

    public void PushAllToQueue(List<string> ids)
    {
        foreach (var id in ids)
        {
            paceManager.PushDataIntoQueue(id);
        }
    }
}