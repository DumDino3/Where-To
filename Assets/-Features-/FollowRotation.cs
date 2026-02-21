using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowRotation : MonoBehaviour
{
    [SerializeField] private Transform target;
    [Header("Leave Target Tag empty if you want to use Target above")]
    [SerializeField] private string targetTag = "";

    void Awake()
    {
        if (!string.IsNullOrWhiteSpace(targetTag))
        {
            GameObject targetGameObject = GameObject.Find(targetTag);

            if (targetGameObject != null)
                target = targetGameObject.transform;
        }
    }

    void Update()
    {
        transform.rotation = target.rotation;
    }
}
