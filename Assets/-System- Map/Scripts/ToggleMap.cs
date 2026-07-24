using UnityEngine;

public class ToggleMap : MonoBehaviour
{
    [SerializeField] private Canvas fullMap;

    public Transform mapTransform1;
    public Transform mapTransform2;

    private bool usingTransform1;

    void Awake()
    {
        ApplyTransform(mapTransform2);
        usingTransform1 = false;

        fullMap.enabled = true;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.M))
        {
            usingTransform1 = !usingTransform1;
            ApplyTransform(usingTransform1 ? mapTransform1 : mapTransform2);
        }
    }

    private void ApplyTransform(Transform target)
    {
        if (fullMap == null || target == null)
            return;

        Transform canvasTransform = fullMap.transform;
        canvasTransform.position = target.position;
        canvasTransform.rotation = target.rotation;
    }
}
