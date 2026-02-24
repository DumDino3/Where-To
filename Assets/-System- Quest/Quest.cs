using UnityEngine;

/// <summary>
/// Defines a quest as a graph of <see cref="QuestNode"/> steps.
/// The graph begins at <see cref="startNode"/> and progresses through
/// connected nodes until no further nodes remain.
/// </summary>
[CreateAssetMenu(fileName = "NewQuest", menuName = "Quest System/Quest")]
public class Quest : ScriptableObject
{
    [Tooltip("Unique identifier used to start this quest from code or Yarn scripts.")]
    public string questId;

    [Tooltip("Display name shown to the player.")]
    public string questTitle;

    [TextArea]
    [Tooltip("Brief summary of the quest shown before it begins.")]
    public string questDescription;

    [Tooltip("The first node in the quest graph. All progression starts here.")]
    public QuestNode startNode;
}
