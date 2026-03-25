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
        for (int i = 0; i < timeSegs; i++)
        {
            initializeTimeSeg.Invoke(i);
        }
    }
    private void StartDay()
    {
        
        if (currentTimeSeg <= timeSegs && isDayStarted == true)
        {
            onDayStarted?.Invoke();
            
            //Hieu: added timeperseg - 1 for insant 1st seg initiation
            currentActualTime += Time.deltaTime + timePerSegs - 1;
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
