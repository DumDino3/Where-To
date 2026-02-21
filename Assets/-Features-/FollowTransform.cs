using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Xsl;
using UnityEngine;

public class FollowTransform : MonoBehaviour
{
    
    [SerializeField] private Transform target;

    [Header("Leave Target Tag empty if you want to use Target above")]
    [SerializeField] private string targetTag = "";

    [Header("Select axis to follow")]
    [SerializeField] public bool x = true;
    [SerializeField] public bool y = true;
    [SerializeField] public bool z = true;

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
        float xTarget;
        float yTarget;
        float zTarget;

        if(x)  {xTarget = target.transform.position.x;}
        else   {xTarget = transform.position.x;}

        if(y)  {yTarget = target.transform.position.y;}
        else   {yTarget = transform.position.y;}

        if(z)  {zTarget = target.transform.position.z;}
        else   {zTarget = transform.position.z;}

        transform.position = new Vector3(xTarget, yTarget, zTarget);
    }
}
