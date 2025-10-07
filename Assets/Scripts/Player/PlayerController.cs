using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 4.8f;
    public float sprintMultiplier = 1.5f;
    public float acceleration = 20f;
    public float deceleration = 25f;

    [Header("Rotation")]
    public bool faceCameraWhileMoving = true;
    public float rotationSmoothTime = 0.08f;
    public Transform cameraTransform;

    [Header("Jump & Gravity")]
    public float gravity = -25f;
    public float fallMultiplier = 1.5f;
    public float jumpHeight = 1.6f;
    public float coyoteTime = 0.1f;
    public float jumpBuffer = 0.1f;

    private CharacterController controller;
    private float currentSpeed, verticalVel, rotVel;
    private float coyoteCounter, jumpBufferCounter;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        float dt = Time.deltaTime;

        // Timers QoL
        coyoteCounter = controller.isGrounded ? coyoteTime : Mathf.Max(0f, coyoteCounter - dt);
        jumpBufferCounter = InputManager.Instance.Jump ? jumpBuffer : Mathf.Max(0f, jumpBufferCounter - dt);

        HandleMovement(dt);
        HandleJumpAndGravity(dt);
    }

    void HandleMovement(float dt)
    {
        Vector2 input = InputManager.Instance.Move;
        bool hasInput = input.sqrMagnitude > 0.0001f;
        bool sprint = InputManager.Instance.Sprint;

        // Direções baseadas na câmera
        Vector3 f = cameraTransform != null ? cameraTransform.forward : Vector3.forward;
        Vector3 r = cameraTransform != null ? cameraTransform.right : Vector3.right;
        f.y = 0f; r.y = 0f; f.Normalize(); r.Normalize();

        Vector3 moveDir = (r * input.x + f * input.y);
        if (hasInput) moveDir.Normalize();

        float targetSpeed = hasInput ? walkSpeed * (sprint ? sprintMultiplier : 1f) : 0f;
        float accel = targetSpeed > currentSpeed ? acceleration : deceleration;
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, accel * dt);

        if (hasInput && faceCameraWhileMoving && cameraTransform != null)
        {
            float camYaw = Mathf.Atan2(f.x, f.z) * Mathf.Rad2Deg;
            float y = Mathf.SmoothDampAngle(transform.eulerAngles.y, camYaw, ref rotVel, rotationSmoothTime);
            transform.rotation = Quaternion.Euler(0f, y, 0f);
        }

        Vector3 horizontal = moveDir * currentSpeed;
        Vector3 velocity = horizontal + Vector3.up * verticalVel;
        controller.Move(velocity * dt);
    }

    void HandleJumpAndGravity(float dt)
    {
        if (controller.isGrounded && verticalVel < 0f) verticalVel = -2f;

        if (coyoteCounter > 0f && jumpBufferCounter > 0f)
        {
            verticalVel = Mathf.Sqrt(jumpHeight * -2f * gravity);
            coyoteCounter = 0f; jumpBufferCounter = 0f;
        }

        float g = verticalVel >= 0f ? gravity : gravity * fallMultiplier;
        verticalVel += g * dt;
    }
}
