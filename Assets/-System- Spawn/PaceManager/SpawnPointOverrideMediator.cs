using System;


public static class SpawnPointOverrideMediator
{
    public static event Action<int> OnSpawnPointOverride;
    
    public static void OverrideSpawnPoint(int spawnPointID)
    {
        OnSpawnPointOverride?.Invoke(spawnPointID);
    }
}
