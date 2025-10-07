using UnityEngine;
public class InputManager : MonoBehaviour
{
    public static InputManager Instance; // Singleton simples
    private InputSystem_Actions inputActions;

    public Vector2 Move { get; private set; }
    public bool Jump { get; private set; }
    public bool Sprint { get; private set; }
    public bool LockOn { get; private set; }
    public bool Pause { get; private set; }
    public bool Interact { get; private set; }
    public bool AdvanceDialogue { get; private set; }


    private void Awake()
    {
        if (Instance == null) Instance = this;
        inputActions = new InputSystem_Actions();
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
    }

    public void SwitchToUI()
    {
        inputActions.Player.Disable();
        inputActions.UI.Enable();
    }

    private void Update()
    {

        // Lê valores crus do Input System
        Move = inputActions.Player.Move.ReadValue<Vector2>();
        Jump = inputActions.Player.Jump.WasPressedThisFrame();
        Sprint = inputActions.Player.Sprint.IsPressed();
        LockOn = inputActions.Player.LockOn.WasPressedThisFrame();
        Pause = inputActions.Player.Pause.WasPressedThisFrame();
        //Debug.Log("Move: " + Move);
        Interact = inputActions.Player.Interact.WasPressedThisFrame();
        Debug.Log("Pressionado" + Interact);
        //========UI BUTTONS========//
        AdvanceDialogue = inputActions.UI.AdvanceDialogue.WasPressedThisFrame();
    }
}
