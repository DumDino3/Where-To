using UnityEngine;

[DisallowMultipleComponent]
public class CarGroundRaySensor : MonoBehaviour
{
    [Header("Visualization")]
    [SerializeField] private bool visualizeInEditor = true;
    [SerializeField] private bool visualizeInPlay = true;
    [SerializeField] private Color rayColor = Color.cyan;
    [SerializeField] private Color hitColor = Color.green;
    [SerializeField] private float hitSphereRadius = 0.05f;

    [Header("Ground Points")]
    [SerializeField] private Vector3 frontGroundOffset = new Vector3(0f, 0f, 1.5f);
    [SerializeField] private Vector3 rearGroundOffset = new Vector3(0f, 0f, -1.5f);

    [Header("Ground Detection")]
    [SerializeField] private float rayStartHeight = 3f;
    [SerializeField] private float rayLength = 6f;
    [SerializeField] private LayerMask groundLayers = ~0;

    private void OnValidate()
    {
        rayStartHeight = Mathf.Max(0f, rayStartHeight);
        rayLength = Mathf.Max(0.01f, rayLength);
        hitSphereRadius = Mathf.Max(0f, hitSphereRadius);
    }

    public CarGroundRaySample Sample()
    {
        Vector3 frontGroundPoint = GetFrontGroundPoint();
        Vector3 rearGroundPoint = GetRearGroundPoint();
        Vector3 frontRayOrigin = GetRayOrigin(frontGroundPoint);
        Vector3 rearRayOrigin = GetRayOrigin(rearGroundPoint);

        bool frontHitGround = CastGroundRay(frontRayOrigin, out RaycastHit frontHit);
        bool rearHitGround = CastGroundRay(rearRayOrigin, out RaycastHit rearHit);

        CarGroundRaySample sample = new CarGroundRaySample(
            frontGroundPoint,
            rearGroundPoint,
            frontRayOrigin,
            rearRayOrigin,
            frontHitGround,
            rearHitGround,
            frontHit,
            rearHit);

        if (visualizeInPlay && Application.isPlaying)
        {
            DrawRuntimeDebug(sample);
        }

        return sample;
    }

    private bool CastGroundRay(Vector3 origin, out RaycastHit hit)
    {
        return Physics.Raycast(origin, Vector3.down, out hit, rayLength, groundLayers, QueryTriggerInteraction.Ignore);
    }

    private Vector3 GetFrontGroundPoint()
    {
        return transform.TransformPoint(frontGroundOffset);
    }

    private Vector3 GetRearGroundPoint()
    {
        return transform.TransformPoint(rearGroundOffset);
    }

    private Vector3 GetRayOrigin(Vector3 groundPoint)
    {
        return groundPoint + Vector3.up * rayStartHeight;
    }

    private void DrawRuntimeDebug(CarGroundRaySample sample)
    {
        DrawDebugRay(sample.frontRayOrigin, sample.frontHitGround, sample.frontHit);
        DrawDebugRay(sample.rearRayOrigin, sample.rearHitGround, sample.rearHit);
    }

    private void DrawDebugRay(Vector3 origin, bool hasHit, RaycastHit hit)
    {
        Debug.DrawLine(origin, origin + Vector3.down * rayLength, rayColor);

        if (hasHit)
        {
            Debug.DrawLine(origin, hit.point, hitColor);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!visualizeInEditor) return;

        DrawGizmoRay(GetRayOrigin(GetFrontGroundPoint()));
        DrawGizmoRay(GetRayOrigin(GetRearGroundPoint()));
    }

    private void DrawGizmoRay(Vector3 origin)
    {
        Gizmos.color = rayColor;
        Gizmos.DrawLine(origin, origin + Vector3.down * rayLength);

        if (CastGroundRay(origin, out RaycastHit hit))
        {
            Gizmos.color = hitColor;
            Gizmos.DrawSphere(hit.point, hitSphereRadius);
        }
    }
}

public readonly struct CarGroundRaySample
{
    public readonly Vector3 frontGroundPoint;
    public readonly Vector3 rearGroundPoint;
    public readonly Vector3 frontRayOrigin;
    public readonly Vector3 rearRayOrigin;
    public readonly bool frontHitGround;
    public readonly bool rearHitGround;
    public readonly RaycastHit frontHit;
    public readonly RaycastHit rearHit;

    public CarGroundRaySample(
        Vector3 frontGroundPoint,
        Vector3 rearGroundPoint,
        Vector3 frontRayOrigin,
        Vector3 rearRayOrigin,
        bool frontHitGround,
        bool rearHitGround,
        RaycastHit frontHit,
        RaycastHit rearHit)
    {
        this.frontGroundPoint = frontGroundPoint;
        this.rearGroundPoint = rearGroundPoint;
        this.frontRayOrigin = frontRayOrigin;
        this.rearRayOrigin = rearRayOrigin;
        this.frontHitGround = frontHitGround;
        this.rearHitGround = rearHitGround;
        this.frontHit = frontHit;
        this.rearHit = rearHit;
    }

    public bool HasAnyHit => frontHitGround || rearHitGround;
    public bool HasFullSupport => frontHitGround && rearHitGround;
}
