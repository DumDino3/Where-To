using UnityEngine;

[CreateAssetMenu(fileName = "NewRideRequest", menuName = "Data/Ride Request")]
public class RideRequestSO : ScriptableObject
{
    public string requestName;
    public int id;
    public int duration;
    public int npc;
    public int dialogue;
    public int spawn;
    public int destination;
    public int priority;
    public int tag;

    /// Hydrates into a runtime RideRequest instance.
    public RideRequest ToRideRequest()
    {
        return new RideRequest
        {
            NAME = requestName,
            ID = id,
            DURATION = duration,
            NPC = npc,
            DIALOGUE = dialogue,
            SPAWN = spawn,
            DESTINATION = destination,
            PRIORITY = priority,
            TAG = tag
        };
    }
}