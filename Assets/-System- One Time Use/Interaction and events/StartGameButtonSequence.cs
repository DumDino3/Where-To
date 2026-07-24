using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

public class StartGameButtonSequence : MonoBehaviour
{
    [Header("Button Canvas")]
    [SerializeField] private Canvas canvasToFade;
    [SerializeField] private float fadeDuration = 1f;

    [Header("Objects Turned On In Order")]
    [SerializeField] private GameObject[] orderedObjects;
    [SerializeField] private float orderedObjectInterval = 0.1f;

    [Header("Blink Objects")]
    [SerializeField] private GameObject[] blinkObjects;
    [SerializeField] private int blinkCount = 2;
    [SerializeField] private float blinkInterval = 0.1f;

    [Header("Dialogue")]
    [SerializeField] private DialogueRunner dialogueRunner;
    [SerializeField] private string winnerDialogueNode = "Winner";

    private bool isRunning;
    private readonly List<GraphicFadeData> canvasGraphics = new List<GraphicFadeData>();

    private void Reset()
    {
        canvasToFade = GetComponent<Canvas>();
    }

    private void Awake()
    {
        if (canvasToFade == null)
        {
            canvasToFade = GetComponent<Canvas>();
        }

        if (dialogueRunner == null)
        {
            dialogueRunner = FindFirstObjectByType<DialogueRunner>(FindObjectsInactive.Include);
        }

        SetObjectsActive(orderedObjects, false);
        SetObjectsActive(blinkObjects, false);
    }

    public void StartGameButton()
    {
        if (isRunning)
        {
            return;
        }

        StartCoroutine(StartGameSequence());
    }

    private IEnumerator StartGameSequence()
    {
        isRunning = true;

        yield return FadeCanvasOut();
        yield return TurnObjectsOnInOrder();
        yield return BlinkObjects();

        if (dialogueRunner != null)
        {
            dialogueRunner.StartDialogue(winnerDialogueNode);
        }
        else
        {
            Debug.LogWarning($"{nameof(StartGameButtonSequence)} could not find a {nameof(DialogueRunner)}.", this);
        }

        isRunning = false;
    }

    private IEnumerator FadeCanvasOut()
    {
        if (canvasToFade == null)
        {
            yield break;
        }

        CacheCanvasGraphics();
        float duration = Mathf.Max(0.01f, fadeDuration);

        for (float elapsed = 0f; elapsed < duration; elapsed += Time.deltaTime)
        {
            SetCanvasGraphicsAlpha(1f - (elapsed / duration));
            yield return null;
        }

        SetCanvasGraphicsAlpha(0f);
        canvasToFade.enabled = false;
    }

    private IEnumerator TurnObjectsOnInOrder()
    {
        foreach (GameObject orderedObject in orderedObjects)
        {
            if (orderedObject != null)
            {
                orderedObject.SetActive(true);
            }

            yield return new WaitForSeconds(orderedObjectInterval);
        }
    }

    private IEnumerator BlinkObjects()
    {
        int safeBlinkCount = Mathf.Max(0, blinkCount);

        for (int i = 0; i < safeBlinkCount; i++)
        {
            SetObjectsActive(blinkObjects, true);
            yield return new WaitForSeconds(blinkInterval);

            SetObjectsActive(blinkObjects, false);
            yield return new WaitForSeconds(blinkInterval);
        }

        SetObjectsActive(blinkObjects, true);
    }

    private void SetObjectsActive(GameObject[] objects, bool active)
    {
        if (objects == null)
        {
            return;
        }

        foreach (GameObject targetObject in objects)
        {
            if (targetObject != null)
            {
                targetObject.SetActive(active);
            }
        }
    }

    private void CacheCanvasGraphics()
    {
        canvasGraphics.Clear();

        Graphic[] graphics = canvasToFade.GetComponentsInChildren<Graphic>(true);
        foreach (Graphic graphic in graphics)
        {
            canvasGraphics.Add(new GraphicFadeData(graphic, graphic.color.a));
        }
    }

    private void SetCanvasGraphicsAlpha(float normalizedAlpha)
    {
        foreach (GraphicFadeData graphicData in canvasGraphics)
        {
            if (graphicData.Graphic == null)
            {
                continue;
            }

            Color color = graphicData.Graphic.color;
            color.a = graphicData.StartAlpha * normalizedAlpha;
            graphicData.Graphic.color = color;
        }
    }

    private readonly struct GraphicFadeData
    {
        public readonly Graphic Graphic;
        public readonly float StartAlpha;

        public GraphicFadeData(Graphic graphic, float startAlpha)
        {
            Graphic = graphic;
            StartAlpha = startAlpha;
        }
    }
}
