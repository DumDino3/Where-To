using System;
using UnityEngine;

public class DayCycleManager : MonoBehaviour
{
    
    [Header("Day settings")] 
    public int timeSegs;
    public float timePerSegs;
    public int currentTimeSeg;

    public bool isDayStarted;


    public float currentActualTime;
    private float totalTime;

    [Header("Debug time")] 
    public float currentSegTimeRemaining;

    public static event Action onDayStarted;
    public static event Action onDayEnded;
    public static event Action<int> onTimeSegsChanged;
    
    
    
    private void Start()
    {
        EstablishSegs();
    }

    private void Update()
    {
        StartDay();
    }
    
    #region Day System
    private void EstablishSegs()
    {
        currentActualTime = 0;
        totalTime = timeSegs * timePerSegs;
    }
    private void StartDay()
    {
        
        if (currentTimeSeg <= timeSegs && isDayStarted == true)
        {
            onDayStarted?.Invoke();
            currentActualTime += Time.deltaTime;
            if (currentActualTime >= timePerSegs)
            {
                currentTimeSeg += 1;
                onTimeSegsChanged?.Invoke(currentTimeSeg);
                currentActualTime = 0;
            }
        }
        else
        {
        }
        
    }
    #endregion

}
