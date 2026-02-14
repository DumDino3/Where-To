using UnityEngine;

[CreateAssetMenu(fileName = "NewNPC", menuName = "ScriptableObject/NPC")]
public class NPC: ScriptableObject
{
    public string npcId;
    public string npcName;
}