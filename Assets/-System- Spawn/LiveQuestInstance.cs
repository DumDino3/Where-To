using System;
using UnityEngine;

public class LiveQuestInstance : MonoBehaviour
{
    public LiveQuestPool liveQuestPool;
    public int duration;
    public int pickupID;
    public int dropOffID;
    public float currentTime;

    private void Update()
    {
        StartRequestCycle();
    }

    //this work when the player decides to accept the quest, then it will be pushed to the active quests list. flag it to the pacemanager.
    public void PushToActive()
    {
        
    }

    public void Initialize(int durationID, int pickupID, int dropOffID)
    {
        
    }
    private void StartRequestCycle()
    {
        currentTime += Time.deltaTime;
        if (currentTime <= duration)
        {
            
        }
        else
        {
            
            liveQuestPool.ReturnToPool(this);
        }
    }
}
