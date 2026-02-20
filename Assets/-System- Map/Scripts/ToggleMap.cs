using UnityEngine;

public class ToggleMap : MonoBehaviour
{
    [SerializeField] Canvas fullMap;

    void Awake()
    {
        fullMap.enabled = false;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.M))
        {
            fullMap.enabled = !fullMap.enabled;
        }
    }
}