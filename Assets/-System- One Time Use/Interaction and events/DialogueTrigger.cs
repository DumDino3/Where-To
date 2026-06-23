using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private string yarnNodeName;

    public bool TryRunDialogue()
    {
        if (string.IsNullOrWhiteSpace(yarnNodeName))
        {
            Debug.LogWarning($"{nameof(DialogueTrigger)} on {name} has no Yarn node name.", this);
            return false;
        }

        if (DialogueHelper.Instance == null)
        {
            Debug.LogWarning($"{nameof(DialogueTrigger)} on {name} could not find a {nameof(DialogueHelper)}.", this);
            return false;
        }

        if (DialogueHelper.Instance.IsDialogueRunning)
            return false;

        DialogueHelper.Instance.RunDialogue(yarnNodeName);
        return true;
    }

    public void SetYarnNodeName(string nodeName)
    {
        yarnNodeName = nodeName;
    }
}
