using TMPro;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.InputSystem;
public class InputManager : MonoBehaviour
{
    public static InputManager Instance; // Singleton simples
    private InputSystem_Actions inputActions;

    public Vector2 Move { get; private set; }
    public bool Jump { get; private set; }
    public bool Sprint { get; private set; }
    public bool LockOn { get; private set; }
    public bool Pause { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        inputActions = new InputSystem_Actions();
    }
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
    private void PauseGame()
    {
        Pause = true;
        Debug.Log("Pausado");
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void Update()
    {
        // Lê valores crus do Input System
        Move = inputActions.Player.Move.ReadValue<Vector2>();
        Jump = inputActions.Player.Jump.WasPressedThisFrame();
        Sprint = inputActions.Player.Sprint.IsPressed();
        LockOn = inputActions.Player.LockOn.WasPressedThisFrame();
        Pause = inputActions.Player.Pause.WasPressedThisFrame();
    }
}
