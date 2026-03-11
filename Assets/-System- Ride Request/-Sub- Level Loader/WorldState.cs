using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class WorldState: MonoBehaviour
{
    private static WorldState _instance;
    public static WorldState Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<WorldState>();
            }
            return _instance;
        }
    }
    public List<string> Tags = new List<string>();
}
