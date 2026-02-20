using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTransform : MonoBehaviour
{
    
    [SerializeField] private Transform target;
    [Header("Leave Target Name empty if you want to use Target above")]
    [SerializeField] private string targetName = "";

    void Awake()
    {
        if (!string.IsNullOrWhiteSpace(targetName))
        {
            GameObject targetGameObject = GameObject.Find(targetName);

            if (targetGameObject != null)
                target = targetGameObject.transform;
        }
    }

    void Update()
    {
        if (target != null)
            transform.position = target.position;
    }
}
