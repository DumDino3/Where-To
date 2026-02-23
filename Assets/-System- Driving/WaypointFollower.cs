using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class WaypointFollower : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float turnSpeed = 5f;
    [SerializeField] private float deadzoneAngle = 25f; 
    [SerializeField] private float maxTurnAngle = 100f; 

    public Transform nextTarget;
    public Transform previousTarget;
    private Vector3 direction;
    public float distance;

    [SerializeField] private int turnDesire = 1; // 0: Left, 1: Straight, 2: Right
    private bool stop = false;
    
    [SerializeField] private Animator SignalAnimator;

    void Update()
    {
        if (SignalAnimator != null) SignalAnimator.SetInteger("Signal", turnDesire);
        
        // Toggle Left
        if (Input.GetKeyDown(KeyCode.A))
        {
            turnDesire = (turnDesire == 0) ? 1 : 0;
        }
        // Toggle Right
        else if (Input.GetKeyDown(KeyCode.D))
        {
            turnDesire = (turnDesire == 2) ? 1 : 2;
        }
        // Manual Straight
        else if (Input.GetKeyDown(KeyCode.W))
        {
            turnDesire = 1;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            Transform temp = nextTarget;
            nextTarget = previousTarget;
            previousTarget = temp;
            stop = false;
        }
        else if (Input.GetKeyDown(KeyCode.Space)) stop = !stop;
    }

    void FixedUpdate()
    {
        if (nextTarget == null || previousTarget == null) return;

        distance = Vector3.Distance(transform.position, nextTarget.position);

        if (distance > 0.5f)
        {
            direction = (nextTarget.position - transform.position).normalized;
            float actualTurn = stop ? 0 : turnSpeed;
            float actualSpeed = stop ? 0 : moveSpeed;

            if (direction != Vector3.zero)
            {
                Quaternion targetRot = Quaternion.LookRotation(direction, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, actualTurn * Time.deltaTime);
            }
            transform.position += transform.forward * actualSpeed * Time.deltaTime;
        }
        else
        {
            Waypoint currentWaypoint = nextTarget.GetComponent<Waypoint>();
            Vector3 approachDir = (nextTarget.position - previousTarget.position).normalized;
            
            Waypoint bestMatch = null;
            Waypoint straightPath = null;
            List<Waypoint> anyForwardPaths = new List<Waypoint>();

            foreach (Waypoint neighbor in currentWaypoint.connectedWaypoints)
            {
                if (neighbor == null || neighbor.transform == previousTarget) continue;

                Vector3 dirToNeighbor = (neighbor.transform.position - nextTarget.position).normalized;
                float angle = Vector3.SignedAngle(approachDir, dirToNeighbor, Vector3.up);

                bool inLeftZone = angle < -deadzoneAngle && angle > -maxTurnAngle;
                bool inRightZone = angle > deadzoneAngle && angle < maxTurnAngle;
                bool inStraightZone = Mathf.Abs(angle) <= deadzoneAngle;

                if (turnDesire == 0 && inLeftZone) { bestMatch = neighbor; break; }
                if (turnDesire == 2 && inRightZone) { bestMatch = neighbor; break; }

                if (inStraightZone) straightPath = neighbor;
                if (inLeftZone || inRightZone || inStraightZone) anyForwardPaths.Add(neighbor);
            }

            if (bestMatch != null) 
            {
                SetNewTarget(bestMatch.transform);
            }
            else if (turnDesire == 1) 
            {
                if (straightPath != null) SetNewTarget(straightPath.transform);
                else if (anyForwardPaths.Count == 1) SetNewTarget(anyForwardPaths[0].transform);
                else stop = true; 
            }
            else stop = true; 
        }
    }

    private void SetNewTarget(Transform target)
    {
        previousTarget = nextTarget;
        nextTarget = target;
        turnDesire = 1; 
        stop = false;
    }

    private void OnDrawGizmos()
    {
        if (nextTarget == null || previousTarget == null) return;
        Vector3 center = nextTarget.position;
        Vector3 approachDir = (nextTarget.position - previousTarget.position).normalized;

        #if UNITY_EDITOR
        // This is the "Spine" - the path the car just took
        Gizmos.color = Color.white;
        Gizmos.DrawLine(previousTarget.position, center);

        // Straight Deadzone
        Handles.color = new Color(0, 1, 0, 0.1f);
        Vector3 straightStart = Quaternion.Euler(0, -deadzoneAngle, 0) * approachDir;
        Handles.DrawSolidArc(center, Vector3.up, straightStart, deadzoneAngle * 2, 5f);

        // Wide Greedy Detection Sweep
        Handles.color = new Color(0, 1, 1, 0.03f); 
        Vector3 sweepStart = Quaternion.Euler(0, -maxTurnAngle, 0) * approachDir;
        Handles.DrawSolidArc(center, Vector3.up, sweepStart, maxTurnAngle * 2, 5f);
        #endif
    }
}