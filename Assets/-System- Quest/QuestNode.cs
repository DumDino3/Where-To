using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A single step in a quest, represented as a node in a directed graph.
/// Connect nodes via <see cref="nextNodes"/> to define quest flow, including
/// branching and parallel paths.
/// </summary>
[CreateAssetMenu(fileName = "NewQuestNode", menuName = "Quest System/Quest Node")]
public class QuestNode : ScriptableObject
{
    public enum NodeType { Talk, GoTo, Collect, Inspect, Custom }

    [Tooltip("Unique identifier used to reference this node from code and Yarn scripts.")]
    public string nodeId;

    [Tooltip("Short title shown in quest log UI.")]
    public string title;

    [TextArea]
    [Tooltip("Description of what the player must do to complete this step.")]
    public string description;

    [Tooltip("Category of objective for this step.")]
    public NodeType nodeType = NodeType.Talk;

    [Tooltip("Optional: NPC ScriptableObject that must be spoken to for this step.")]
    public NPC requiredNpc;

    [Tooltip("Nodes that become active once this node is completed. Supports branching.")]
    public List<QuestNode> nextNodes = new List<QuestNode>();
}
