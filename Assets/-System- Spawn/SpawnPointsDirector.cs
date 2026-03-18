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

    private void OnEnable()
    {
        SpawnPoint.onSpawnPointChanged -= CleanAndFlush; // guard against double-sub
        SpawnPoint.onSpawnPointChanged += CleanAndFlush;
        SpawnPaceManager.OnSpawnPointToggle -= ActivateSpawnPointByID;
        SpawnPaceManager.OnSpawnPointToggle += ActivateSpawnPointByID;
        SpawnPaceManager.OnRequestDone -= DisableAllChildren;
        SpawnPaceManager.OnRequestDone += DisableAllChildren;
        
        AddSpawnPointInChildren();
        CleanAndFlush();
    }

    private void OnDisable()
    {
        SpawnPoint.onSpawnPointChanged -= CleanAndFlush;
        SpawnPaceManager.OnSpawnPointToggle -= ActivateSpawnPointByID;
        SpawnPaceManager.OnRequestDone -= DisableAllChildren; 
    }

    private void Update()
    {
        if (!Application.isPlaying)
        {
            DisableAllChildren();
        }
    }

    [ContextMenu("Refresh Spawn Points")]
    public void AddSpawnPointInChildren()
    {
        SpawnPoint[] childSpawnPoints = GetComponentsInChildren<SpawnPoint>(includeInactive: true);
        foreach (var sp in childSpawnPoints)
        {
            AddSpawnPoint(sp);
        }
    }

    public void AddSpawnPoint(SpawnPoint newPoint)
    {
        if (newPoint == null) return;

        bool alreadyRegistered = spawnPointsList.Any(sp => sp.spawnPoint == newPoint);
        if (alreadyRegistered) return;

        spawnPointsList.Add(new SpawnPointData {
            spawnPoint = newPoint,
            spawnPointID = newPoint.spawnPointID
        });
    }

    public void CleanAndFlush()
    {
        int before = spawnPointsList.Count;

        spawnPointsList = spawnPointsList
            .Where(sp => sp != null && sp.spawnPoint != null)
            .Select(sp => new SpawnPointData {
                spawnPoint = sp.spawnPoint,
                spawnPointID = sp.spawnPoint.spawnPointID
            })
            .ToList();
        SpawnPoint[] childSpawnPoints = GetComponentsInChildren<SpawnPoint>(includeInactive: true);
        foreach (var sp in childSpawnPoints)
        {
            AddSpawnPoint(sp);
        }
    }

    public void DisableAllChildren()
    {
        foreach (var data in spawnPointsList)
        {
            if (data.spawnPoint != null)
                data.spawnPoint.gameObject.SetActive(false);
        }
    }

    private void ActivateSpawnPointByID(int targetID)
    {
        foreach (var data in spawnPointsList)
        {
            if (data.spawnPoint == null) continue;
            bool shouldBeActive = data.spawnPointID == targetID;
            data.spawnPoint.gameObject.SetActive(shouldBeActive);
        }
        
    }
}