using UnityEngine;
using Yarn.Unity;

public class DialogueHelper : MonoBehaviour
{
    public static DialogueHelper Instance { get; private set; }

    private DialogueRunner diagRunner;

    private void Awake()
    {
        //Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }

        DialogueHelper.Instance.diagRunner = GameObject.FindAnyObjectByType<DialogueRunner>();
    }

    public void RunDialogue(string title)
    {
        if (diagRunner.IsDialogueRunning)
        {
            Debug.LogWarning("Dialogue is already running!");
            return;
        }
        diagRunner.StartDialogue(title);
    }

    public void ForceDialogue(string title)
    {
        if (diagRunner.IsDialogueRunning)
        {
            diagRunner.Stop();
        }
        diagRunner.StartDialogue(title);
    }

    // public void SkipLine()
    // {
    //     if (diagRunner.IsDialogueRunning)
    //     {
    //         // Yarn Spinner 2.0+ uses the LineView component to handle skipping
    //         // This is a common way to trigger a "skip" via the runner
    //         diagRunner.OnViewUserIntentNextLine();
    //     }
    // }
}