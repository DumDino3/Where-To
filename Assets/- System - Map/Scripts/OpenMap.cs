using UnityEngine;

public class OpenMap : MonoBehaviour
{
    public Canvas map;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            map.enabled = !map.enabled;
        }
    }
}