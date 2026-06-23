using UnityEngine;
using Unity.Cinemachine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;

public class WaypointFollower : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float turnSpeed = 5f;
    [SerializeField] private float deadzoneAngle = 25f; 
    [SerializeField] private float maxTurnAngle = 100f; 

    [Header("Slow Down Input")]
    [SerializeField] private float slowDownDuration = 1f;
    [SerializeField] private float slowDownMoveSpeedMultiplier = 0.6666667f;
    [SerializeField] private float slowDownTurnSpeedMultiplier = 0.5f;
    [SerializeField] private float slowDownTurnRecoveryDuration = 0.5f;
    [SerializeField] private float turnAroundDecelerationMultiplier = 2f;

    [Header("Turn Signal Objects")]
    [SerializeField] private GameObject leftTurnSignalObject;
    [SerializeField] private GameObject rightTurnSignalObject;
    [SerializeField] private float turnSignalBlinkInterval = 0.35f;

    [Header("Requested Turn Slowdown")]
    [SerializeField] private float requestedTurnMoveSpeedMultiplier = 0.8f;
    [SerializeField] private float requestedTurnTurnSpeedMultiplier = 0.8f;

    [Header("Front Camera FOV")]
    [SerializeField] private CamManager camManager;
    [SerializeField] private int frontVirtualCameraIndex = 1;
    [SerializeField] private float frontCameraFovDecreaseIncrement = 10f;
    [SerializeField] private float frontCameraFovLerpSpeed = 5f;

    public Transform nextTarget;
    public Transform previousTarget;
    private Vector3 direction;
    public float distance;
    public float distanceFromReached;

    [SerializeField] private int turnDesire = 1; // 0: Left, 1: Straight, 2: Right
    [SerializeField] private int checkpointsBeforeSignalReset = 2;
    private bool stop = true;
    private bool inputLockedByTurn = false;
    private int checkpointsSinceTurnStarted = 0;
    private float slowDownTimer = 0f;
    private float slowDownTurnRecoveryTimer = 0f;
    private float turnSignalTimer = 0f;
    private bool turnSignalVisible = false;
    private int lastTurnSignalDesire = 1;
    private float requestedMoveSpeed = 0f;
    private CinemachineCamera frontVirtualCamera;
    private float defaultFrontCameraFov;
    private bool hasDefaultFrontCameraFov = false;
    private CarCustomPhysics customPhysics;
    
    [SerializeField] private Animator SignalAnimator;

    private void Awake()
    {
        customPhysics = GetComponent<CarCustomPhysics>();
        ResolveFrontVirtualCamera();
    }

    void Update()
    {
        if (SignalAnimator != null) SignalAnimator.SetInteger("Signal", turnDesire);
        UpdateTurnSignals();

        if (slowDownTimer > 0f) slowDownTimer -= Time.deltaTime;
        else if (slowDownTurnRecoveryTimer > 0f) slowDownTurnRecoveryTimer -= Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            stop = !stop;
        }
        if (inputLockedByTurn) return;
        
        // Toggle Left
        if (Input.GetKeyDown(KeyCode.A))
        {
            turnDesire = 0;
            stop = false;
        }
        // Toggle Right
        else if (Input.GetKeyDown(KeyCode.D))
        {
            turnDesire = 2;
            stop = false;
        }
        // Manual Straight
        else if (Input.GetKeyDown(KeyCode.W))
        {
            turnDesire = 1;
            stop = false;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            slowDownTimer = slowDownDuration;
            slowDownTurnRecoveryTimer = slowDownTurnRecoveryDuration;
            turnDesire = 1;

            Transform temp = nextTarget;
            nextTarget = previousTarget;
            previousTarget = temp;
            stop = false;
        }
    }

    void FixedUpdate()
    {
        if (nextTarget == null || previousTarget == null)
        {
            requestedMoveSpeed = 0f;
            return;
        }

        Vector3 targetOffset = nextTarget.position - transform.position;
        targetOffset.y = 0f;
        distance = targetOffset.magnitude;

        if (distance > distanceFromReached)
        {
            Vector3 directionToTarget = targetOffset;
            direction = Vector3.ProjectOnPlane(directionToTarget, Vector3.up).normalized;
            float speedMultiplier = (slowDownTimer > 0f ? slowDownMoveSpeedMultiplier : 1f) * GetRequestedTurnMoveSpeedMultiplier();
            float turnMultiplier = GetCurrentTurnSpeedMultiplier();
            float decelerationMultiplier = GetCurrentDecelerationMultiplier();
            float actualTurn = stop ? 0 : turnSpeed * turnMultiplier;
            float actualSpeed = stop ? 0 : moveSpeed * speedMultiplier;
            requestedMoveSpeed = actualSpeed;

            if (direction != Vector3.zero)
            {
                Quaternion targetRot = Quaternion.LookRotation(direction, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, actualTurn * Time.fixedDeltaTime);
            }

            if (customPhysics != null)
            {
                customPhysics.MoveForward(actualSpeed, Time.fixedDeltaTime, decelerationMultiplier);
            }
            else
            {
                transform.position += transform.forward * actualSpeed * Time.fixedDeltaTime;
            }
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
                SetNewTarget(bestMatch.transform, turnDesire != 1 && !inputLockedByTurn);
            }
            else if (straightPath != null)
            {
                SetNewTarget(straightPath.transform);
            }
            else if (turnDesire == 1 && anyForwardPaths.Count == 1)
            {
                SetNewTarget(anyForwardPaths[0].transform);
            }
            else
            {
                stop = true;
                requestedMoveSpeed = 0f;
                if (customPhysics != null) customPhysics.UpdateSpeed(0f, Time.fixedDeltaTime, GetCurrentDecelerationMultiplier());
            }
        }
    }

    private void LateUpdate()
    {
        UpdateFrontCameraFov();
    }

    private float GetCurrentTurnSpeedMultiplier()
    {
        float requestedTurnMultiplier = GetRequestedTurnTurnSpeedMultiplier();

        if (slowDownTimer > 0f)
        {
            return slowDownTurnSpeedMultiplier * requestedTurnMultiplier;
        }

        if (slowDownTurnRecoveryTimer <= 0f || slowDownTurnRecoveryDuration <= 0f)
        {
            return requestedTurnMultiplier;
        }

        float progress = 1f - Mathf.Clamp01(slowDownTurnRecoveryTimer / slowDownTurnRecoveryDuration);

        return Mathf.Lerp(slowDownTurnSpeedMultiplier, 1f, progress) * requestedTurnMultiplier;
    }

    private float GetCurrentDecelerationMultiplier()
    {
        return slowDownTimer > 0f ? turnAroundDecelerationMultiplier : 1f;
    }

    private void UpdateFrontCameraFov()
    {
        if (frontVirtualCamera == null)
        {
            ResolveFrontVirtualCamera();
        }

        if (frontVirtualCamera == null)
        {
            return;
        }

        float speed = customPhysics != null ? customPhysics.CurrentSpeed : requestedMoveSpeed;
        float speedT = Mathf.Clamp01(speed / Mathf.Max(0.01f, moveSpeed));
        float targetFov = defaultFrontCameraFov - ((1f - speedT) * frontCameraFovDecreaseIncrement);
        float lerpT = 1f - Mathf.Exp(-Mathf.Max(0f, frontCameraFovLerpSpeed) * Time.deltaTime);
        LensSettings lens = frontVirtualCamera.Lens;
        lens.FieldOfView = Mathf.Lerp(lens.FieldOfView, targetFov, lerpT);
        frontVirtualCamera.Lens = lens;
    }

    private void ResolveFrontVirtualCamera()
    {
        if (frontVirtualCamera != null)
        {
            return;
        }

        if (camManager == null)
        {
            camManager = FindFirstObjectByType<CamManager>();
        }

        if (camManager == null ||
            camManager.CamAngles == null ||
            frontVirtualCameraIndex < 0 ||
            frontVirtualCameraIndex >= camManager.CamAngles.Count)
        {
            return;
        }

        frontVirtualCamera = camManager.CamAngles[frontVirtualCameraIndex];

        if (frontVirtualCamera != null && !hasDefaultFrontCameraFov)
        {
            defaultFrontCameraFov = frontVirtualCamera.Lens.FieldOfView;
            hasDefaultFrontCameraFov = true;
        }
    }

    private float GetRequestedTurnMoveSpeedMultiplier()
    {
        return IsRequestedTurnActive() ? requestedTurnMoveSpeedMultiplier : 1f;
    }

    private float GetRequestedTurnTurnSpeedMultiplier()
    {
        return IsRequestedTurnActive() ? requestedTurnTurnSpeedMultiplier : 1f;
    }

    private bool IsRequestedTurnActive()
    {
        return turnDesire == 0 || turnDesire == 2;
    }

    private void UpdateTurnSignals()
    {
        if (!IsRequestedTurnActive())
        {
            lastTurnSignalDesire = turnDesire;
            turnSignalTimer = 0f;
            turnSignalVisible = false;
            SetTurnSignalState(false, false);
            return;
        }

        if (lastTurnSignalDesire != turnDesire)
        {
            lastTurnSignalDesire = turnDesire;
            turnSignalTimer = 0f;
            turnSignalVisible = true;
        }

        float blinkInterval = Mathf.Max(0.01f, turnSignalBlinkInterval);
        turnSignalTimer += Time.deltaTime;

        if (turnSignalTimer >= blinkInterval)
        {
            turnSignalTimer -= blinkInterval;
            turnSignalVisible = !turnSignalVisible;
        }

        SetTurnSignalState(turnDesire == 0 && turnSignalVisible, turnDesire == 2 && turnSignalVisible);
    }

    private void SetTurnSignalState(bool leftActive, bool rightActive)
    {
        if (leftTurnSignalObject != null && leftTurnSignalObject.activeSelf != leftActive)
        {
            leftTurnSignalObject.SetActive(leftActive);
        }

        if (rightTurnSignalObject != null && rightTurnSignalObject.activeSelf != rightActive)
        {
            rightTurnSignalObject.SetActive(rightActive);
        }
    }

    private void SetNewTarget(Transform target, bool startedRequestedTurn = false)
    {
        previousTarget = nextTarget;
        nextTarget = target;

        if (startedRequestedTurn)
        {
            inputLockedByTurn = true;
            checkpointsSinceTurnStarted = 0;
        }
        else if (inputLockedByTurn)
        {
            checkpointsSinceTurnStarted++;

            if (checkpointsSinceTurnStarted >= checkpointsBeforeSignalReset)
            {
                turnDesire = 1;
                inputLockedByTurn = false;
                checkpointsSinceTurnStarted = 0;
            }
        }
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
