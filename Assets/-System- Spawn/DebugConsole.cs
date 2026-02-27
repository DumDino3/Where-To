using UnityEngine;

public class DebugConsole : MonoBehaviour
{
    private SpawnPaceManager manager;

    [RuntimeInitializeOnLoadMethod]
    static void Init()
    {
        var go = new GameObject("DebugConsole");
        go.AddComponent<DebugConsole>();
        DontDestroyOnLoad(go);
    }

    void Start()
    {
        manager = FindObjectOfType<SpawnPaceManager>();
    }

    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 200, 20), "Debug Active");

        if (manager != null)
        {
            GUI.Label(new Rect(10, 30, 300, 20),
                "Current Segment: " + manager.currentTimeSeg);

            GUI.Label(new Rect(10, 50, 300, 20),
                "Time Remaining: " + manager.currentActualTime.ToString("F2"));
        }
    }
}