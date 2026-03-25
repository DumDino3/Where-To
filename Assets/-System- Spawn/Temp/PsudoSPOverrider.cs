using System;
using UnityEngine;

public class PsudoSPOverrider: MonoBehaviour
{
    public int spawnPointIDToOverride;

    [ContextMenu("Override SP")]
    public void OverrideSP()
    {
        SpawnPointOverrideMediator.OverrideSpawnPoint(spawnPointIDToOverride);
    }
    
}
