using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObject2DPlane : MonoBehaviour
{
    public Transform target;

    void Update()
    {
       transform.position = new Vector3(target.transform.position.x, transform.position.y , target.transform.position.z);
    }
}
