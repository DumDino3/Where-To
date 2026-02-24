using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private Transform target;
    [Tooltip("Leave empty to use Target field above")]
    [SerializeField] private string targetTag = "";

    [Header("Position Following")]
    [SerializeField] private bool followPosition = false;
    [SerializeField] private bool positionX = true;
    [SerializeField] private bool positionY = true;
    [SerializeField] private bool positionZ = true;
    [SerializeField] private Vector3 positionOffset = Vector3.zero;

    [Header("Rotation Following")]
    [SerializeField] private bool followRotation = false;
    [SerializeField] private bool rotationX = true;
    [SerializeField] private bool rotationY = true;
    [SerializeField] private bool rotationZ = true;
    [SerializeField] private Vector3 rotationOffset = Vector3.zero;

    [Header("Scale Following")]
    [SerializeField] private bool followScale = false;
    [SerializeField] private bool scaleX = true;
    [SerializeField] private bool scaleY = true;
    [SerializeField] private bool scaleZ = true;

    [Header("Smoothing")]
    [SerializeField] private bool smoothPosition = false;
    [Range(0.01f, 1f)]
    [SerializeField] private float positionSmoothSpeed = 0.1f;
    [SerializeField] private bool smoothRotation = false;
    [Range(0.01f, 1f)]
    [SerializeField] private float rotationSmoothSpeed = 0.1f;

    [Header("Update Mode")]
    [SerializeField] private UpdateMode updateMode = UpdateMode.Update;

    private enum UpdateMode
    {
        Update,
        LateUpdate,
        FixedUpdate
    }

    void Awake()
    {
        if (!string.IsNullOrWhiteSpace(targetTag))
        {
            GameObject targetGameObject = GameObject.FindWithTag(targetTag);

            if (targetGameObject != null)
                target = targetGameObject.transform;
            else
                Debug.LogWarning($"FollowTarget: No GameObject found with tag '{targetTag}'");
        }
    }

    void Update()
    {
        if (updateMode == UpdateMode.Update)
            FollowTargetTransform();
    }

    void LateUpdate()
    {
        if (updateMode == UpdateMode.LateUpdate)
            FollowTargetTransform();
    }

    void FixedUpdate()
    {
        if (updateMode == UpdateMode.FixedUpdate)
            FollowTargetTransform();
    }

    private void FollowTargetTransform()
    {
        if (target == null) return;

        if (followPosition)
            UpdatePosition();

        if (followRotation)
            UpdateRotation();

        if (followScale)
            UpdateScale();
    }

    private void UpdatePosition()
    {
        Vector3 targetPos = target.position + positionOffset;
        Vector3 currentPos = transform.position;

        float xPos = positionX ? targetPos.x : currentPos.x;
        float yPos = positionY ? targetPos.y : currentPos.y;
        float zPos = positionZ ? targetPos.z : currentPos.z;

        Vector3 newPosition = new Vector3(xPos, yPos, zPos);

        if (smoothPosition)
            transform.position = Vector3.Lerp(transform.position, newPosition, positionSmoothSpeed);
        else
            transform.position = newPosition;
    }

    private void UpdateRotation()
    {
        Vector3 targetRot = target.eulerAngles + rotationOffset;
        Vector3 currentRot = transform.eulerAngles;

        float xRot = rotationX ? targetRot.x : currentRot.x;
        float yRot = rotationY ? targetRot.y : currentRot.y;
        float zRot = rotationZ ? targetRot.z : currentRot.z;

        Vector3 newRotation = new Vector3(xRot, yRot, zRot);

        if (smoothRotation)
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(newRotation), rotationSmoothSpeed);
        else
            transform.eulerAngles = newRotation;
    }

    private void UpdateScale()
    {
        Vector3 targetScale = target.localScale;
        Vector3 currentScale = transform.localScale;

        float xScale = scaleX ? targetScale.x : currentScale.x;
        float yScale = scaleY ? targetScale.y : currentScale.y;
        float zScale = scaleZ ? targetScale.z : currentScale.z;

        transform.localScale = new Vector3(xScale, yScale, zScale);
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void SetFollowPosition(bool value)
    {
        followPosition = value;
    }

    public void SetFollowRotation(bool value)
    {
        followRotation = value;
    }

    public void SetFollowScale(bool value)
    {
        followScale = value;
    }
}
