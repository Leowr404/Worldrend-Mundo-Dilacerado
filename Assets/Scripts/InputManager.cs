using UnityEngine;
using UnityEngine.InputSystem;
public class InputManager : MonoBehaviour
{
    [Header("Input (Workflow: Actions / Polling)")]
    public InputActionAsset actionsAsset; // ou use Project-wide Actions
    public string moveActionPath = "Player/Move";
    public string jumpActionPath = "Player/Jump";
    public string sprintActionPath = "Player/Sprint";
    public string LockOnActionPath = "Player/LockOn";

    [Header("Refs")]
    public Transform cameraTransform;   // Main Camera (com Cinemachine Brain)

    [Header("Movement")]
    public float walkSpeed = 4.8f;
    public float sprintMultiplier = 1.5f;
    public float acceleration = 20f;
    public float deceleration = 25f;

    [Header("Facing")]
    [Tooltip("Girar o player para o Yaw da câmera SOMENTE quando há input.")]
    public bool faceCameraWhileMoving = true;
    public float rotationSmoothTime = 0.08f;

    [Header("Jump & Gravity")]
    public float gravity = -25f;
    public float fallMultiplier = 1.5f;
    public float jumpHeight = 1.6f;
    public float coyoteTime = 0.1f;
    public float jumpBuffer = 0.1f;

    CharacterController controller;
    InputAction moveA, jumpA, sprintA, LockOnA;

    float currentSpeed, verticalVel, rotVel;
    float coyoteCounter, jumpBufferCounter;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        var asset = actionsAsset != null ? actionsAsset : InputSystem.actions;
        moveA = asset.FindAction(moveActionPath, true);
        jumpA = asset.FindAction(jumpActionPath, true);
        sprintA = asset.FindAction(sprintActionPath, false);
        LockOnA = asset.FindAction(LockOnActionPath, false);
        Cursor.lockState = CursorLockMode.Locked;
    }
    void OnEnable() { moveA.Enable(); jumpA.Enable(); sprintA?.Enable(); LockOnA?.Enable(); }
    void OnDisable() { moveA.Disable(); jumpA.Disable(); sprintA?.Disable(); LockOnA?.Disable(); }

    void Update()
    {
        float dt = Time.deltaTime;

        // QoL timers
        coyoteCounter = controller.isGrounded ? coyoteTime : Mathf.Max(0f, coyoteCounter - dt);
        jumpBufferCounter = (jumpA.WasPressedThisFrame() || jumpA.triggered) ? jumpBuffer : Mathf.Max(0f, jumpBufferCounter - dt);

        HandleMovement(dt);
        HandleJumpAndGravity(dt);

        if (LockOnA.WasPressedThisFrame())
        {
            Debug.Log("Lock ON");
        }
    }

    void HandleMovement(float dt)
    {
        Vector2 input = moveA.ReadValue<Vector2>(); // x=AD (strafe), y=WS (frente/trás)
        bool hasInput = input.sqrMagnitude > 0.0001f;
        bool sprint = sprintA != null && sprintA.IsPressed();

        // Eixos da câmera no plano
        Vector3 f = cameraTransform != null ? cameraTransform.forward : Vector3.forward;
        Vector3 r = cameraTransform != null ? cameraTransform.right : Vector3.right;
        f.y = 0f; r.y = 0f; f.Normalize(); r.Normalize();

        // Direção de deslocamento: W frente da câmera, A/D strafe, S trás
        Vector3 moveDir = (r * input.x + f * input.y);
        if (hasInput) moveDir.Normalize();

        // Velocidade alvo
        float targetSpeed = hasInput ? walkSpeed * (sprint ? sprintMultiplier : 1f) : 0f;
        float accel = targetSpeed > currentSpeed ? acceleration : deceleration;
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, accel * dt);

        // Rotação do player: só quando HÁ input (mover a câmera sozinha não gira o player)
        if (hasInput && faceCameraWhileMoving && cameraTransform != null)
        {
            float camYaw = Mathf.Atan2(f.x, f.z) * Mathf.Rad2Deg;
            float y = Mathf.SmoothDampAngle(transform.eulerAngles.y, camYaw, ref rotVel, rotationSmoothTime);
            transform.rotation = Quaternion.Euler(0f, y, 0f);
        }

        // Aplicar deslocamento
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
