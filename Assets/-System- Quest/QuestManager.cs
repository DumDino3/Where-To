using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Manages all runtime quest state. Tracks which <see cref="QuestNode"/> steps are
/// currently active and advances the quest graph when steps are completed.
/// 
/// Place a single instance of this component in the scene (alongside the other
/// manager prefabs) and reference it wherever quests need to be started or queried.
/// </summary>
public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    [System.Serializable]
    public class QuestNodeEvent : UnityEvent<QuestNode> { }

    [System.Serializable]
    public class QuestEvent : UnityEvent<Quest> { }

    [Header("Events")]
    [Tooltip("Raised when a quest node becomes active.")]
    public QuestNodeEvent onNodeActivated;

    [Tooltip("Raised when a quest node is completed.")]
    public QuestNodeEvent onNodeCompleted;

    [Tooltip("Raised when an entire quest is completed (no further nodes remain).")]
    public QuestEvent onQuestCompleted;

    // Active quest nodes, keyed by questId → set of active QuestNodes
    private readonly Dictionary<string, HashSet<QuestNode>> _activeNodes =
        new Dictionary<string, HashSet<QuestNode>>();

    // Completed quest node ids (global, so replaying is avoided)
    private readonly HashSet<string> _completedNodeIds = new HashSet<string>();

    // Quests that have been started, for completion checking
    private readonly Dictionary<string, Quest> _startedQuests = new Dictionary<string, Quest>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>Starts a quest, activating its start node.</summary>
    public void StartQuest(Quest quest)
    {
        if (quest == null || quest.startNode == null)
        {
            Debug.LogWarning("[QuestManager] Cannot start quest: null quest or missing start node.");
            return;
        }

        if (_startedQuests.ContainsKey(quest.questId))
        {
            Debug.LogWarning($"[QuestManager] Quest '{quest.questId}' is already started.");
            return;
        }

        _startedQuests[quest.questId] = quest;
        _activeNodes[quest.questId] = new HashSet<QuestNode>();
        ActivateNode(quest.questId, quest.startNode);
    }

    /// <summary>Marks a node as completed and activates its successors.</summary>
    public void CompleteNode(QuestNode node)
    {
        if (node == null) return;

        string questId = FindQuestIdForNode(node);
        if (questId == null)
        {
            Debug.LogWarning($"[QuestManager] Node '{node.nodeId}' is not currently active in any quest.");
            return;
        }

        _activeNodes[questId].Remove(node);
        _completedNodeIds.Add(node.nodeId);
        onNodeCompleted?.Invoke(node);

        if (node.nextNodes == null || node.nextNodes.Count == 0)
        {
            // No successors — check if this quest is now fully complete
            if (_activeNodes[questId].Count == 0)
            {
                onQuestCompleted?.Invoke(_startedQuests[questId]);
            }
            return;
        }

        foreach (QuestNode next in node.nextNodes)
        {
            if (next != null && !_completedNodeIds.Contains(next.nodeId) && !_activeNodes[questId].Contains(next))
            {
                ActivateNode(questId, next);
            }
        }

        // After advancing, check if all paths are done
        if (_activeNodes[questId].Count == 0)
        {
            onQuestCompleted?.Invoke(_startedQuests[questId]);
        }
    }

    /// <summary>Returns true if the given node is currently active.</summary>
    public bool IsNodeActive(QuestNode node)
    {
        if (node == null) return false;
        foreach (var set in _activeNodes.Values)
        {
            if (set.Contains(node)) return true;
        }
        return false;
    }

    /// <summary>Returns true if the given node has been completed.</summary>
    public bool IsNodeCompleted(string nodeId) => _completedNodeIds.Contains(nodeId);

    /// <summary>Returns true if the given quest has been started.</summary>
    public bool IsQuestStarted(string questId) => _startedQuests.ContainsKey(questId);

    // ── helpers ──────────────────────────────────────────────────────────────

    private void ActivateNode(string questId, QuestNode node)
    {
        _activeNodes[questId].Add(node);
        onNodeActivated?.Invoke(node);
    }

    private string FindQuestIdForNode(QuestNode node)
    {
        foreach (var kvp in _activeNodes)
        {
            if (kvp.Value.Contains(node)) return kvp.Key;
        }
        return null;
    }
}
