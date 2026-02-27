using System;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPaceManager : MonoBehaviour
{
    public class dataPackage
    {
        private int duration;
        private int pickUpId;
        private int dropOffId;
    }
    
    
    
    [SerializeField]
    int travelIDRaw;

    public int chunkSize = 4;
    [SerializeField] private int duration;
    [SerializeField] private int priority;
    [SerializeField] private int currentID;
    [SerializeField] private int pickUpID;
    [SerializeField] private int dropOffID;

    [Header("Day settings")] 
    public int timeSegs;
    public float timePerSegs;
    public int currentTimeSeg;

    public bool isDayStarted;


    public float currentActualTime;
    private float totalTime;

    [Header("Debug time")] 
    public float currentSegTimeRemaining;

    private void Start()
    {
        EstablishSegs();
    }

    private void Update()
    {
        StartDay();
    }

    private void EstablishSegs()
    {
        currentTimeSeg = 1;
        currentActualTime = 0;
        totalTime = timeSegs * timePerSegs;
    }
    private void StartDay()
    {
        
        if (currentTimeSeg <= timeSegs && isDayStarted == true)
        {
            
            currentActualTime += Time.deltaTime;
            if (currentActualTime >= timePerSegs)
            {
                currentTimeSeg += 1;
                currentActualTime = 0;
            }
        }
        else
        {
            Debug.Log(timeSegs + "done day");
        }
        
        
        
    }

    [SerializeField] private CurrentTaxiState currentState;

    public static event Action<int> OnSpawnPointChanged; //this command the director
    
    public static event Action<int> OnChunkChanged;
    
    enum CurrentTaxiState
    {
        PickUp,
        DropOff,
    }



    [ContextMenu("Parse Travel ID")]
    private void ParsingId()
    {
        duration = travelIDRaw / 1000;
        pickUpID = travelIDRaw / 1000000;
        dropOffID = travelIDRaw % 100000;
        priority = dropOffID % 1000;

    }

    [ContextMenu("Test Spawn Event")]
    private void TestSpawnEvent()
    {
        switch (currentState)
        {
            case CurrentTaxiState.PickUp:
                OnSpawnPointChanged?.Invoke(pickUpID);
                Debug.Log($"Invocated PickUp event with ID: {pickUpID}");
                break;

            case CurrentTaxiState.DropOff:
                OnSpawnPointChanged?.Invoke(dropOffID);
                Debug.Log($"Invocated DropOff event with ID: {dropOffID}");
                break;
        }
    }
}