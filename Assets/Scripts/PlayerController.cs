using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float jumpForce = 5f;

    [Header("Mouse Look Settings")]
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float maxLookAngle = 80f;

    private float verticalRotation = 0f;
    private bool isGrounded;
    private Vector3 velocity;
    private float currentMoveSpeed;

    private Transform cameraTransform;
    private CharacterController characterController;

    private void Start()
    {
        // Cache components
        characterController = GetComponent<CharacterController>();
        cameraTransform = transform.Find("CameraHolder").transform;
        
        // Lock and hide cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        currentMoveSpeed = moveSpeed;
    }

    private void Update()
    {
        HandleMovement();
        HandleMouseLook();
        HandleJump();
    }

    private void HandleMovement()
    {
        // Get input
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Calculate movement direction
        Vector3 moveDirection = transform.right * horizontalInput + transform.forward * verticalInput;
        moveDirection = moveDirection.normalized;

        // Apply movement
        characterController.Move(moveDirection * currentMoveSpeed * Time.deltaTime);

        // Handle sprint
        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentMoveSpeed = sprintSpeed;
        }
        else
        {
            currentMoveSpeed = moveSpeed;
        }

        // Apply gravity
        if (!characterController.isGrounded)
        {
            velocity.y += Physics.gravity.y * Time.deltaTime;
        }
        else
        {
            velocity.y = -2f; // Small downward force when grounded
        }

        characterController.Move(velocity * Time.deltaTime);
    }

    private void HandleMouseLook()
    {
        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Rotate the player (horizontal rotation)
        transform.Rotate(Vector3.up * mouseX);

        // Rotate the camera (vertical rotation)
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -maxLookAngle, maxLookAngle);
        cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }

    private void HandleJump()
    {
        if (characterController.isGrounded)
        {
            if (Input.GetButtonDown("Jump"))
            {
                velocity.y = Mathf.Sqrt(jumpForce * -2f * Physics.gravity.y);
            }
        }
    }
}
