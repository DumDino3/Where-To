using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;
    public bool movementEnabled = true;

    [Header("View")]
    public CinemachineCamera vcam; // assign in Inspector
    public Transform cameraView;
    public float maxAngle = 90f;
    public float minAngle = -90f;

    [Tooltip("Mouse sensitivity (no deltaTime scaling)")]
    public float mouseSensitivity = 100f;

    [HideInInspector] public bool isTalking = false;
    private CharacterController characterController;
    private Vector3 moveDirection;
    private const float gravity = -9.81f;
    [SerializeField] private float gravityTweaker;
    private float verticalVelocity = 0f;
    private float xRotation = 0f;
    
    public bool IsFrozen { get; set; }
    void Awake()
    {
        var cc = GetComponent<CharacterController>();
        if (cc != null) cc.minMoveDistance = 0f; // prevent high-FPS stutter
    }

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    

    private void LateUpdate()
    {
        if (!isTalking)
        {
            HandleCameraRotation();
        }

        if (vcam != null)
             vcam.transform.rotation = cameraView.rotation;
    }



    private void Update()
    {
        if (!IsFrozen && movementEnabled)
            HandleMovement();
    }

    private void HandleMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 forward = cameraView.forward;
        Vector3 right = cameraView.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        Vector3 move = (forward * vertical + right * horizontal).normalized * speed;

        if (characterController != null)
        {
            if (characterController.isGrounded)
            {
                verticalVelocity = -1f; // keep grounded
            }
            else
            {
                verticalVelocity += (gravity - gravityTweaker) * Time.deltaTime;
            }

            move.y = verticalVelocity;
            characterController.Move(move * Time.deltaTime);
        }
        else
        {
            move.y = 0f;
            transform.position += move * Time.deltaTime;
        }
    }

    private void HandleCameraRotation()
    {
        var mouse = Mouse.current;
        if (mouse == null) return;

        float mouseX = mouse.delta.x.ReadValue() * mouseSensitivity;
        float mouseY = mouse.delta.y.ReadValue() * mouseSensitivity;

        // Rotate the player horizontally
        transform.Rotate(Vector3.up * mouseX);

        // Rotate the camera vertically
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, minAngle, maxAngle);
        cameraView.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    public void ResetHead()
    {
        xRotation = 0f;  // reset stored vertical rotation
    }

    public void SetMovementEnabled(bool enabled)
    {
        movementEnabled = enabled;
    }

    public void ToggleMovement()
    {
        movementEnabled = !movementEnabled;
    }
}
