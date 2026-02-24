using UnityEngine;
using Yarn.Unity;

/// <summary>
/// Exposes Quest System commands and functions to Yarn Spinner dialogue scripts.
///
/// Add this component to the same GameObject as your <see cref="DialogueRunner"/>
/// (or any active GameObject in the scene).
///
/// Available Yarn commands:
/// <code>
///   &lt;&lt;quest_start "questId"&gt;&gt;
///   &lt;&lt;quest_complete_node "nodeId"&gt;&gt;
/// </code>
///
/// Available Yarn functions:
/// <code>
///   quest_started("questId")      → bool
///   quest_node_active("nodeId")   → bool
///   quest_node_done("nodeId")     → bool
/// </code>
/// </summary>
public class QuestCommands : MonoBehaviour
{
    [Tooltip("All quests that can be started from Yarn scripts. Must contain matching questIds.")]
    public Quest[] quests;

    [Tooltip("The DialogueRunner to register quest functions with. If unassigned, auto-detected at Start.")]
    public DialogueRunner dialogueRunner;

    private void Start()
    {
        // Register functions with the DialogueRunner
        DialogueRunner runner = dialogueRunner != null ? dialogueRunner : FindObjectOfType<DialogueRunner>();
        if (runner != null)
        {
            runner.AddFunction("quest_started",     (string questId) => QuestStarted(questId));
            runner.AddFunction("quest_node_active", (string nodeId)  => QuestNodeActive(nodeId));
            runner.AddFunction("quest_node_done",   (string nodeId)  => QuestNodeDone(nodeId));
        }
    }

    // ── Commands (called via <<command>> syntax in Yarn) ─────────────────────

    [YarnCommand("quest_start")]
    public void StartQuest(string questId)
    {
        Quest quest = FindQuest(questId);
        if (quest == null)
        {
            Debug.LogWarning($"[QuestCommands] No quest found with id '{questId}'.");
            return;
        }

        if (QuestManager.Instance == null)
        {
            Debug.LogError("[QuestCommands] QuestManager instance not found in scene.");
            return;
        }

        QuestManager.Instance.StartQuest(quest);
    }

    [YarnCommand("quest_complete_node")]
    public void CompleteNode(string nodeId)
    {
        QuestNode node = FindNode(nodeId);
        if (node == null)
        {
            Debug.LogWarning($"[QuestCommands] No quest node found with id '{nodeId}'.");
            return;
        }

        if (QuestManager.Instance == null)
        {
            Debug.LogError("[QuestCommands] QuestManager instance not found in scene.");
            return;
        }

        QuestManager.Instance.CompleteNode(node);
    }

    // ── Functions (called inline in Yarn conditions) ──────────────────────────

    private bool QuestStarted(string questId)
    {
        return QuestManager.Instance != null && QuestManager.Instance.IsQuestStarted(questId);
    }

    private bool QuestNodeActive(string nodeId)
    {
        if (QuestManager.Instance == null) return false;
        QuestNode node = FindNode(nodeId);
        return node != null && QuestManager.Instance.IsNodeActive(node);
    }

    private bool QuestNodeDone(string nodeId)
    {
        return QuestManager.Instance != null && QuestManager.Instance.IsNodeCompleted(nodeId);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private Quest FindQuest(string questId)
    {
        if (quests == null) return null;
        foreach (Quest q in quests)
        {
            if (q != null && q.questId == questId) return q;
        }
        return null;
    }

    private QuestNode FindNode(string nodeId)
    {
        if (quests == null) return null;
        foreach (Quest q in quests)
        {
            if (q == null || q.startNode == null) continue;
            QuestNode found = SearchGraph(q.startNode, nodeId, new System.Collections.Generic.HashSet<string>());
            if (found != null) return found;
        }
        return null;
    }

    private QuestNode SearchGraph(QuestNode node, string targetId, System.Collections.Generic.HashSet<string> visited)
    {
        if (node == null || visited.Contains(node.nodeId)) return null;
        visited.Add(node.nodeId);

        if (node.nodeId == targetId) return node;

        foreach (QuestNode next in node.nextNodes)
        {
            QuestNode result = SearchGraph(next, targetId, visited);
            if (result != null) return result;
        }
        return null;
    }
}
