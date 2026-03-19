using System;
using System.Collections.Generic;

[Serializable]
public struct RideRequestEntry
{
    public string day;
    public string requestName;
    public string requestId;
    public string timeSegment;
    public string priority;
    public string duration;
    public string npcId;
    public string dialoguePoolId;
    public string spawnId;
    public string destinationId;
    public List<string> condition;
}