using UnityEngine;

[CreateAssetMenu(fileName = "NewNPC", menuName = "ScriptableObject/NPC")]
public class NPC: ScriptableObject
{
    public int npcId;
    //public string npcName;
    public int spawnLocation;
    public int destination;
    public float availableDuration;
}