using UnityEngine;

[RequireComponent(typeof(CarGroundRaySensor))]
[DisallowMultipleComponent]
public class CarCustomPhysics : MonoBehaviour
{
    [Header("Forced Grounding")]
    [SerializeField] private bool forceInLateUpdate = true;
    [SerializeField] private float maxRotationLerpSpeed = 12f;
    [SerializeField] private float turnCoefficientAcceleration = 8f;

    [Header("Speed Enforcement")]
    [SerializeField] private float acceleration = 8f;
    [SerializeField] private float deceleration = 12f;

    private CarGroundRaySensor groundSensor;
    private float currentTurnCoefficient;
    private float currentSpeed;
    private float targetSpeed;

    public float CurrentSpeed => currentSpeed;
    public float TargetSpeed => targetSpeed;

    private void OnValidate()
    {
        maxRotationLerpSpeed = Mathf.Max(0f, maxRotationLerpSpeed);
        turnCoefficientAcceleration = Mathf.Max(0f, turnCoefficientAcceleration);
        acceleration = Mathf.Max(0f, acceleration);
        deceleration = Mathf.Max(0f, deceleration);
    }

    private void Awake()
    {
        groundSensor = GetComponent<CarGroundRaySensor>();
    }

    private void Update()
    {
        if (!forceInLateUpdate)
        {
            ForceGroundPose();
        }
    }

    private void LateUpdate()
    {
        if (forceInLateUpdate)
        {
            ForceGroundPose();
        }
    }

    public void MoveForward(float requestedSpeed, float deltaTime, float decelerationMultiplier = 1f)
    {
        UpdateSpeed(requestedSpeed, deltaTime, decelerationMultiplier);
        transform.position += transform.forward * currentSpeed * deltaTime;
    }

    public void UpdateSpeed(float requestedSpeed, float deltaTime, float decelerationMultiplier = 1f)
    {
        targetSpeed = Mathf.Max(0f, requestedSpeed);
        float speedChangeRate = currentSpeed < targetSpeed
            ? acceleration
            : deceleration * Mathf.Max(0f, decelerationMultiplier);
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, speedChangeRate * deltaTime);
    }

    private void ForceGroundPose()
    {
        CarGroundRaySample sample = groundSensor.Sample();

        if (!sample.HasAnyHit)
        {
            currentTurnCoefficient = Mathf.MoveTowards(
                currentTurnCoefficient,
                0f,
                turnCoefficientAcceleration * Time.deltaTime);
            return;
        }

        ForceRotation(sample);
        ForceGroundDistance();
    }

    private void ForceRotation(CarGroundRaySample sample)
    {
        Vector3 desiredUp = GetDesiredUp(sample);
        Quaternion targetRotation = Quaternion.FromToRotation(transform.up, desiredUp) * transform.rotation;
        float alignmentAngle = Quaternion.Angle(transform.rotation, targetRotation);
        float targetTurnCoefficient = Mathf.Clamp01(alignmentAngle / 45f);

        currentTurnCoefficient = Mathf.MoveTowards(
            currentTurnCoefficient,
            targetTurnCoefficient,
            turnCoefficientAcceleration * Time.deltaTime);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            currentTurnCoefficient * maxRotationLerpSpeed * Time.deltaTime);
    }

    private Vector3 GetDesiredUp(CarGroundRaySample sample)
    {
        if (sample.HasFullSupport)
        {
            return (sample.frontHit.normal + sample.rearHit.normal).normalized;
        }

        return sample.frontHitGround ? sample.frontHit.normal : sample.rearHit.normal;
    }

    private void ForceGroundDistance()
    {
        CarGroundRaySample rotatedSample = groundSensor.Sample();

        if (!rotatedSample.HasAnyHit)
        {
            return;
        }

        transform.position += Vector3.up * GetGroundCorrection(rotatedSample);
    }

    private float GetGroundCorrection(CarGroundRaySample sample)
    {
        if (sample.HasFullSupport)
        {
            float frontCorrection = sample.frontHit.point.y - sample.frontGroundPoint.y;
            float rearCorrection = sample.rearHit.point.y - sample.rearGroundPoint.y;

            return (frontCorrection + rearCorrection) * 0.5f;
        }

        if (sample.frontHitGround)
        {
            return sample.frontHit.point.y - sample.frontGroundPoint.y;
        }

        return sample.rearHit.point.y - sample.rearGroundPoint.y;
    }
}
