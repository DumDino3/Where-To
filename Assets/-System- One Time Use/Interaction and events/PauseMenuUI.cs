using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;

public class PauseMenuUI : MonoBehaviour
{
    [Header("Pause Menu")]
    [SerializeField] private Canvas pauseCanvas;
    [SerializeField] private KeyCode pauseKey = KeyCode.Escape;
    [SerializeField] private bool pauseTimeScale = true;

    [Header("Dialogue")]
    [SerializeField] private DialogueRunner dialogueRunner;
    [SerializeField] private string tutorialDialogueNode = "Tutorial";

    private bool isPaused;
    private float previousTimeScale = 1f;

    private void Reset()
    {
        pauseCanvas = GetComponent<Canvas>();
    }

    private void Awake()
    {
        if (pauseCanvas == null)
        {
            pauseCanvas = GetComponent<Canvas>();
        }

        if (dialogueRunner == null)
        {
            dialogueRunner = FindFirstObjectByType<DialogueRunner>(FindObjectsInactive.Include);
        }

        SetPauseMenu(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(pauseKey))
        {
            TogglePauseMenu();
        }
    }

    public void TogglePauseMenu()
    {
        SetPauseMenu(!isPaused);
    }

    public void ReloadScene()
    {
        Time.timeScale = previousTimeScale <= 0f ? 1f : previousTimeScale;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void RunTutorialDialogueAndClose()
    {
        if (dialogueRunner != null)
        {
            if (dialogueRunner.IsDialogueRunning)
            {
                dialogueRunner.Stop();
            }

            dialogueRunner.StartDialogue(tutorialDialogueNode);
        }
        else
        {
            Debug.LogWarning($"{nameof(PauseMenuUI)} could not find a {nameof(DialogueRunner)}.", this);
        }

        SetPauseMenu(false);
    }

    public void QuitApplication()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void SetPauseMenu(bool paused)
    {
        isPaused = paused;

        if (pauseCanvas != null)
        {
            pauseCanvas.enabled = paused;
        }

        if (!pauseTimeScale)
        {
            return;
        }

        if (paused)
        {
            previousTimeScale = Time.timeScale;
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = previousTimeScale <= 0f ? 1f : previousTimeScale;
        }
    }
}
