using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Analytics;

public class WaypointFollower : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float turnSpeed = 1f;

    //public Transform previousWaypoint;
    public Transform nextTarget;
    public Transform previousTarget;
    private Vector3 direction;
    public float distance;

    [SerializeField] private int turnDesire = 1;
    private bool stop = false;
    
    //Use this to manipulate the car turn signal's animation
    [SerializeField] private Animator SignalAnimator;
    private int signalID;
    //-------------------------------//
    
    void Update()
    {
        if (SignalAnimator != null)
        {
            signalID = turnDesire;
            SignalAnimator.SetInteger("Signal", signalID);
        }
        
        //Turn desire 0 = left desire ; 1 = no desire  ; 2 = right desire
        if (Input.GetKeyDown(KeyCode.A))
        {
            turnDesire = 0;
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            turnDesire = 2;
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            turnDesire = 1;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            Transform tempNextTarget = nextTarget;
            Transform tempPreviousTarget = previousTarget;
            
            nextTarget = tempPreviousTarget;
            previousTarget = tempNextTarget;

            stop = false;
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            stop = !stop;
        }

        //Calculate distance and set waypoint
        distance = Vector3.Distance(this.transform.position, nextTarget.position);

        //If target hasn's been reached, rotate towards it and move foward
        if (distance > 0.5)
        {
            CalculateDirection();
            MoveTowardsWaypoint();
        }

        //If target has been reached find a new target
        else if (distance <= 1)
        {
            Waypoint targetWaypointScript = nextTarget.gameObject.GetComponent<Waypoint>();
            signalID = 1;

            //If target ISN'T a waypoint, move to the next target
            if (targetWaypointScript.isIntersection != true)
            {
                bool foundNext = false;
                foreach (Waypoint connectedWaypoint in targetWaypointScript.connectedWaypoints)
                {
                    if (connectedWaypoint != null && connectedWaypoint.gameObject.transform != previousTarget)
                    {
                        previousTarget = nextTarget;
                        nextTarget = connectedWaypoint.gameObject.transform;
                        stop = false;
                        foundNext = true;
                        break;
                    }
                }
                if (!foundNext) stop = true;
            }

            //If target IS a waypoint, set target according to desire
            else if(targetWaypointScript.isIntersection == true)
            {
                //Turn desire 0 = left desire ; 1 = no desire  ; 2 = right desire
                switch (turnDesire)
                {
                    case 0:
                        if (targetWaypointScript.leftTurn != null)
                        {
                            previousTarget = nextTarget;
                            nextTarget = targetWaypointScript.leftTurn.gameObject.transform;
                            stop = false;
                        }
                        else
                        {
                            stop = true;
                        }
                        turnDesire = 1;
                        break;

                    //If no desire, go straight if possible, else stop
                    case 1:
                        
                        bool foundStraight = false;
                        foreach (Waypoint connectedWaypoint in targetWaypointScript.connectedWaypoints)
                        {
                            if (connectedWaypoint != null && connectedWaypoint.gameObject.transform != previousTarget)
                            {
                                previousTarget = nextTarget;
                                nextTarget = connectedWaypoint.gameObject.transform;
                                stop = false;
                                foundStraight = true;
                                break;
                            }
                        }

                        if (!foundStraight) stop = true;
                        
                        break;

                    case 2:
                        if (targetWaypointScript.rightTurn != null)
                        {
                            previousTarget = nextTarget;
                            nextTarget = targetWaypointScript.rightTurn.gameObject.transform;
                            stop = false;
                        }
                        else
                        {
                            stop = true;
                        }
                        turnDesire = 1;
                        break;
                }
            }
        }
    }

    private void CalculateDirection()
    {
        direction = new Vector3(nextTarget.position.x - this.transform.position.x,
                                nextTarget.position.y - this.transform.position.y,
                                nextTarget.position.z - this.transform.position.z).normalized;
    }

    private void MoveTowardsWaypoint()
    {
        float actualTurn = 0;
        float actualSpeed = 0;

        if (stop)
        {
            actualTurn = 0;
            actualSpeed = 0;
        }

        if (!stop)
        {
            actualTurn = turnSpeed;
            actualSpeed = moveSpeed;
        }

        Quaternion targetRot = Quaternion.LookRotation(direction, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, actualTurn * Time.deltaTime);

        transform.position += this.transform.forward * actualSpeed * Time.deltaTime;
    }
}
