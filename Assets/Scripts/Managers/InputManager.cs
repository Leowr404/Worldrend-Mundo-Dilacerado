using Unity.Cinemachine;
using UnityEngine;
public class InputManager : MonoBehaviour
{
    public static InputManager Instance; // Singleton simples
    private InputSystem_Actions inputActions;
    CinemachineOrbitalFollow cinemachineOrbitalFollow;

    public Vector2 Move { get; private set; }
    public bool Attack { get; private set; }
    public bool Jump { get; private set; }
    public bool Sprint { get; private set; }
    public bool LockOn { get; private set; }
    public bool Pause { get; private set; }
    public bool Interact { get; private set; }
    public bool AdvanceDialogue { get; private set; }
    public bool OpenInventory { get; private set; }
    public bool CloseInventory { get; private set; }



    private void Awake()
    {
        if (Instance == null) Instance = this;
        inputActions = new InputSystem_Actions();
        cinemachineOrbitalFollow = FindAnyObjectByType<CinemachineOrbitalFollow>();

    }
    private void Start()
    {


    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }
    public void SwitchToPlayer()
    {
        inputActions.UI.Disable();
        inputActions.Player.Enable();
        Cursor.lockState = CursorLockMode.Locked;
        cinemachineOrbitalFollow.enabled = true;
    }

    public void SwitchToUI()
    {
        inputActions.Player.Disable();
        inputActions.UI.Enable();
        Cursor.lockState = CursorLockMode.None;
        cinemachineOrbitalFollow.enabled = false;
    }

    private void Update()
    {

        // Lê valores crus do Input System
        Attack = inputActions.Player.Attack.WasPressedThisFrame();
        Move = inputActions.Player.Move.ReadValue<Vector2>();
        Jump = inputActions.Player.Jump.WasPressedThisFrame();
        Sprint = inputActions.Player.Sprint.IsPressed();
        LockOn = inputActions.Player.LockOn.WasPressedThisFrame();
        Pause = inputActions.Player.Pause.WasPressedThisFrame();
        //Debug.Log("Move: " + Move);
        OpenInventory = inputActions.Player.OpenInventory.WasPressedThisFrame();
        Interact = inputActions.Player.Interact.WasPressedThisFrame();
        //========UI BUTTONS========//
        AdvanceDialogue = inputActions.UI.AdvanceDialogue.WasPressedThisFrame();
        CloseInventory = inputActions.UI.CloseInventory.WasPressedThisFrame();
        
    }
}
