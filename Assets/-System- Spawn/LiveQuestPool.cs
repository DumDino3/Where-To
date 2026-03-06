using System;
using UnityEngine;



public class LiveQuestPool : MonoBehaviour
{
    public GameObject liveQuestPrefab;
    
    private int poolLimit = 10;

    private void OnEnable()
    {
        SpawnPaceManager.OnRequestSpawned += EnableLiveQuest;
        SpawnPaceManager.OnCabinStateChanged += FlushPool;
    }

    private void OnDisable()
    {
        SpawnPaceManager.OnRequestSpawned -= EnableLiveQuest;
        SpawnPaceManager.OnCabinStateChanged -= FlushPool;
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
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject questObj = transform.GetChild(i).gameObject;

            if (!questObj.activeSelf) 
            {
                LiveQuestInstance instance = questObj.GetComponent<LiveQuestInstance>();
                instance.Initialize(durationID, pickupID, dropOffID);
                questObj.SetActive(true);
                return; 
            }
        }
    }
    
    public void FlushPool()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject questObj = transform.GetChild(i).gameObject;
            questObj.SetActive(false);
        }
    }
    
    public void ReturnToPool(LiveQuestInstance questInstance)
    {
        questInstance.gameObject.SetActive(false);
    }
}
