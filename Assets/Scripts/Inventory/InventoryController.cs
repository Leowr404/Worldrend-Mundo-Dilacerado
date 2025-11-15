using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [SerializeField] private GameObject inventoryPanel;

    private InputSystem_Actions input;
    private bool isInventoryOpen;

    private void Awake()
    {
        input = InputManager.Instance.InputActions;
    }

    private void OnEnable()
    {
        // PLAYER: abre inventário
        input.Player.Inventory.performed += OnInventoryToggle;

        // UI: fecha inventário
        input.UI.CloseInventory.performed += OnCloseInventory;
    }

    private void OnDisable()
    {
        input.Player.Inventory.performed -= OnInventoryToggle;
        input.UI.CloseInventory.performed -= OnCloseInventory;
    }

    private void OnInventoryToggle(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        ToggleInventory();
    }

    private void OnCloseInventory(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        if (isInventoryOpen)
            ToggleInventory();
    }

    private void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;
        inventoryPanel.SetActive(isInventoryOpen);

        if (isInventoryOpen)
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

    // Botão UI chama isso
    public void InventoryClose()
    {
        if (!isInventoryOpen) return;
        ToggleInventory();
    }
}
