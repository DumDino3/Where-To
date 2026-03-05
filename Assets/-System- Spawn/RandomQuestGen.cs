using System;
using UnityEngine;

public class RandomRequestGen : MonoBehaviour
{
    
    public static event Action<string> onQuestGenerated;
    private int questLimits = 10;
    private int durationID;
    private int pickupID;
    private int dropOffID;
    private int priorityID;
    private int timeSeg;
    
    private void Start()
    {
        GenerateRandomQuest();
    }
    
    private void GenerateRandomQuest()
    {
        for (int i = 0; i < questLimits; i++)
        {
            durationID = UnityEngine.Random.Range(10, 20); 
            pickupID = UnityEngine.Random.Range(1, 10);
            dropOffID = UnityEngine.Random.Range(1, 10);
            priorityID = 0;
            timeSeg = UnityEngine.Random.Range(0, 3);
            
            string questID = $"{durationID:D3}{pickupID:D3}{dropOffID:D3}{priorityID:D2}{timeSeg:D2}";
            onQuestGenerated?.Invoke(questID);
        }
    }
    
    
}
