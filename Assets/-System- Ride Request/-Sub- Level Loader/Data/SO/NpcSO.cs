using UnityEngine;

[CreateAssetMenu(fileName = "NewNpc", menuName = "Data/NPC")]
public class NpcSO : ScriptableObject
{
    public string npcName;
    public string id;
    public string displayName;
    public string modelId;
}