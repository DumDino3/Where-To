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
    #endregion

}
