using System;

[Serializable]
public struct RideRequestEntry
{
    public string requestId;
    public string requestName;
    public string npcId;
    public string spawnId;
    public string destinationId;
    public string duration;
    //public string priority;
    public string conditionId;
    public string dialoguePoolId;
}