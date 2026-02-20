using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public List<Waypoint> connectedWaypoints = new List<Waypoint>();
    public Transform leftTurn;
    public Transform rightTurn;
    public bool isIntersection = false;
}
