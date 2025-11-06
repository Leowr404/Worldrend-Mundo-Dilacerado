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

    [Header("Stamina Config")]
    public int sprintStaminaCostPerSecond = 15; // quanto gasta por segundo

    private CharacterController controller;
    private PlayerStats stats;
    private float currentSpeed, verticalVel, rotVel;
    private float coyoteCounter, jumpBufferCounter;
    private bool isSprinting;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        stats = GetComponent<PlayerStats>(); // ligação direta com o sistema de status
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
        bool sprintInput = InputManager.Instance.Sprint;

        // Direções baseadas na câmera
        Vector3 f = cameraTransform ? cameraTransform.forward : Vector3.forward;
        Vector3 r = cameraTransform ? cameraTransform.right : Vector3.right;
        f.y = 0f; r.y = 0f; f.Normalize(); r.Normalize();

        Vector3 moveDir = (r * input.x + f * input.y);
        if (hasInput) moveDir.Normalize();

        // ⚡ Sprint — integrado ao PlayerStats
        if (sprintInput && hasInput)
        {
            if (stats.SpendStamina(PlayerStats.StaminaAction.Sprint))
            {
                isSprinting = true;
            }
            else
            {
                isSprinting = false; // sem stamina, para de correr
            }
        }
        else
        {
            stats.StopConsumingStamina();
            isSprinting = false;
        }

        // 🔹 velocidade e aceleração
        float targetSpeed = hasInput ? walkSpeed * (isSprinting ? sprintMultiplier : 1f) : 0f;
        float accel = targetSpeed > currentSpeed ? acceleration : deceleration;
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, accel * dt);

        // 🔹 rotação baseada na câmera
        if (hasInput && faceCameraWhileMoving && cameraTransform)
        {
            float camYaw = Mathf.Atan2(f.x, f.z) * Mathf.Rad2Deg;
            float y = Mathf.SmoothDampAngle(transform.eulerAngles.y, camYaw, ref rotVel, rotationSmoothTime);
            transform.rotation = Quaternion.Euler(0f, y, 0f);
        }

        // 🔹 movimento final
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
