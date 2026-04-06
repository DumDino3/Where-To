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
    public static event Action<int> initializeTimeSeg;
    
    
    // Hieu adjustment to start first segment right away
    private void Start()
    {
        currentTimeSeg = 0;
        EstablishSegs();
        
        // Plays only once per start to set first segment transition right away
        currentActualTime = timePerSegs - 1f;

        onTimeSegsChanged?.Invoke(currentTimeSeg);
        onDayStarted?.Invoke();
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
        for (int i = 1; i < timeSegs; i++)
        {
            initializeTimeSeg.Invoke(i);
        }
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
