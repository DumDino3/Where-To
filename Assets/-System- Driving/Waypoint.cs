using System;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public List<Waypoint> connectedWaypoints = new List<Waypoint>();

    private void OnValidate()
    {
        connectedWaypoints.RemoveAll(item => item == null);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
    }
}