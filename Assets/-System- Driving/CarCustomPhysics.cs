using UnityEngine;

[RequireComponent(typeof(CarGroundRaySensor))]
[DisallowMultipleComponent]
public class CarCustomPhysics : MonoBehaviour
{
    [Header("Forced Grounding")]
    [SerializeField] private bool forceInLateUpdate = true;
    [SerializeField] private float rotationLerpSpeed = 12f;

    private CarGroundRaySensor groundSensor;

    private void OnValidate()
    {
        rotationLerpSpeed = Mathf.Max(0f, rotationLerpSpeed);
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

    private void ForceGroundPose()
    {
        CarGroundRaySample sample = groundSensor.Sample();

        if (!sample.HasAnyHit)
        {
            return;
        }

        ForceRotation(sample);
        ForceGroundDistance();
    }

    private void ForceRotation(CarGroundRaySample sample)
    {
        Vector3 desiredUp = GetDesiredUp(sample);
        Quaternion targetRotation = Quaternion.FromToRotation(transform.up, desiredUp) * transform.rotation;

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationLerpSpeed * Time.deltaTime);
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
