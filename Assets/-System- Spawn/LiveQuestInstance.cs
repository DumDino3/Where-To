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
        // Reset timer to 0
        currentTime = 0; 
    
        // Convert duration (minutes) to total seconds (e.g., 5 min * 60 = 300 seconds)
        // If durationID is 0, give it a default (like 5 mins) so it doesn't vanish!
        duration = (durationID > 0) ? durationID * 60 : 300; 
    
        this.pickupID = pickupID;
        this.dropOffID = dropOffID;
    }

    private void StartRequestCycle()
    {
        currentTime += Time.deltaTime;

        // While we are still under the time limit, do nothing (keep active)
        if (currentTime >= duration)
        {
            // Time is up! 
            liveQuestPool.ReturnToPool(this);
        }
    }
}
