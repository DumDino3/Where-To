using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIPointerDebugger : MonoBehaviour
{
    private PointerEventData pointerData;
    private EventSystem eventSystem;

    void Awake()
    {
        eventSystem = EventSystem.current;
        pointerData = new PointerEventData(eventSystem);
    }

    void Update()
    {
        pointerData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        eventSystem.RaycastAll(pointerData, results);

        if (results.Count > 0)
        {
            RaycastResult top = results[0];
            Debug.Log($"🖱️ Pointer over UI: {GetFullPath(top.gameObject)}");
        }
    }

    string GetFullPath(GameObject obj)
    {
        string path = obj.name;
        Transform current = obj.transform.parent;

        while (current != null)
        {
            path = current.name + "/" + path;
            current = current.parent;
        }

        return path;
    }
}