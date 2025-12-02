using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [Header("Painéis")]
    public GameObject inventoryPanel;

    private InputSystem_Actions input;
    private bool isOpen;

    private void Awake()
    {
        input = InputManager.Instance.InputActions;
    }

    private void OnEnable()
    {
        input.Player.Inventory.performed += OnToggle;
        input.UI.CloseInventory.performed += OnClose;
    }

    private void OnDisable()
    {
        input.Player.Inventory.performed -= OnToggle;
        input.UI.CloseInventory.performed -= OnClose;
    }

    private void OnToggle(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        Toggle();
    }

    private void OnClose(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        if (isOpen)
            Toggle();
    }

    public void Toggle()
    {
        isOpen = !isOpen;
        inventoryPanel.SetActive(isOpen);

        if (isOpen)
        {
            InputManager.Instance.SwitchToUI();
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            InputManager.Instance.SwitchToPlayer();
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
