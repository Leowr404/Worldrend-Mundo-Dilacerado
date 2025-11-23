using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [Header("Painéis")]
    [SerializeField] private GameObject inventoryPanel;

    private InputSystem_Actions input;
    private bool isInventoryOpen;

    private void Awake()
    {
        input = InputManager.Instance.InputActions;
    }

    private void OnEnable()
    {
        input.Player.Inventory.performed += OnInventoryToggle;
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

    // ================================
    // INVENTORY TOGGLE FINAL
    // ================================
    public void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;

        // habilita/desabilita inventário
        inventoryPanel.SetActive(isInventoryOpen);

        // controla cursor e input
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

    // botão UI chama isso
    public void InventoryClose()
    {
        if (!isInventoryOpen) return;
        ToggleInventory();
    }
}
