using System;
using UnityEngine;



public class LiveQuestPool : MonoBehaviour
{
    GameObject liveQuestPrefab;
    
    private int poolLimit = 20;

    private void OnEnable()
    {
        SpawnPaceManager.OnRequestSpawned += EnableLiveQuest;
    }

    private void OnDisable()
    {
        SpawnPaceManager.OnRequestSpawned -= EnableLiveQuest;
    }

    private void Start()
    {
        for (int i = 0; i <  poolLimit; i++)
        {
                GameObject questInstance = Instantiate(liveQuestPrefab, transform);
                LiveQuestInstance liveQuestInstance = questInstance.GetComponent<LiveQuestInstance>();
                liveQuestInstance.liveQuestPool = this;
                questInstance.SetActive(false);
        }
    }

    private void EnableLiveQuest(int durationID, int pickupID, int dropOffID)
    {
        foreach (Transform child in transform)
        {
            if (!child.gameObject.activeSelf)
            {
                LiveQuestInstance quest = child.GetComponent<LiveQuestInstance>();
                quest.Initialize(durationID, pickupID, dropOffID);
       
                child.gameObject.SetActive(true);
            
                return;
            }
        }
    }
    
    public void ReturnToPool(LiveQuestInstance questInstance)
    {
        questInstance.gameObject.SetActive(false);
    }
}
