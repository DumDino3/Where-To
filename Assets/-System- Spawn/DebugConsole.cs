using UnityEngine;

public class DebugConsole : MonoBehaviour
{
    private SpawnPaceManager manager;
    private DayCycleManager daycycleManager;

    [RuntimeInitializeOnLoadMethod]
    static void Init()
    {
        var go = new GameObject("DebugConsole");
        go.AddComponent<DebugConsole>();
        DontDestroyOnLoad(go);
    }

    void Start()
    {
        manager = FindFirstObjectByType<SpawnPaceManager>();
        daycycleManager = FindFirstObjectByType<DayCycleManager>();
    }

    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 200, 20), "Debug Active");

        if (manager != null)
        {
            GUI.Label(new Rect(10, 30, 300, 20),
                "Current Segment: " + daycycleManager.currentTimeSeg);

            GUI.Label(new Rect(10, 50, 300, 20),
                "Time Remaining: " + daycycleManager.currentActualTime.ToString("F2"));
        }
    }
}