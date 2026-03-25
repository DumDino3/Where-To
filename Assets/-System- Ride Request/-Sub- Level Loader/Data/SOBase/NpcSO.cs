using UnityEngine;

[CreateAssetMenu(fileName = "NewNpc", menuName = "Database/NPC")]
public class NpcSO : ScriptableObject
{
    public string npcName;
    public string id;
    public string displayName;
    public string modelId;
}