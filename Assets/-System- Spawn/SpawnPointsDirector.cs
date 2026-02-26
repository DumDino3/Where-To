using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[ExecuteAlways]
public class SpawnPointsDirector : MonoBehaviour
{
    [SerializeField] private List<SpawnPointData> spawnPointsList = new List<SpawnPointData>();
    public IReadOnlyList<SpawnPointData> SpawnPoints => spawnPointsList;

    [Serializable]
    public class SpawnPointData
    {
        public SpawnPoint spawnPoint;
        public int spawnPointID; 
    }

    private void OnEnable() => SpawnPoint.onSpawnPointChanged += CleanAndFlush;
    private void OnDisable() => SpawnPoint.onSpawnPointChanged -= CleanAndFlush;

    public void AddSpawnPoint(SpawnPoint newPoint)
    {
        if (newPoint == null || spawnPointsList.Any(sp => sp.spawnPoint == newPoint)) return;
        
        spawnPointsList.Add(new SpawnPointData { 
            spawnPoint = newPoint, 
            spawnPointID = newPoint.spawnPointID
        });
    }
    
    public void CleanAndFlush()
    {
        spawnPointsList = spawnPointsList
            .Where(sp => sp != null && sp.spawnPoint != null)
            .Select(sp => new SpawnPointData {
                spawnPoint = sp.spawnPoint,
                spawnPointID = sp.spawnPoint.spawnPointID
            })
            .ToList();
    }
}