using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [SerializeField] private GameObject inventoryPanel;

    private bool isInventoryOpen = false;
    private float lastToggleTime = 0f;
    private float toggleCooldown = 0.25f;

    private void Start()
    {
        if (inventoryPanel != null)
            inventoryPanel.SetActive(false);

        // garante que o cursor começa escondido
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if (InputManager.Instance == null)
            return;

        bool tabPressed =
            InputManager.Instance.InputActions.Player.Inventory.triggered ||
            InputManager.Instance.InputActions.UI.Inventory.triggered;

        if (tabPressed && Time.time - lastToggleTime > toggleCooldown)
        {
            ToggleInventory();
            lastToggleTime = Time.time;
        }
    }

    private void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;

        if (inventoryPanel != null)
            inventoryPanel.SetActive(isInventoryOpen);

        Debug.Log(isInventoryOpen ? "Inventário Aberto" : "Inventário Fechado");

        // alterna o mapa de input
        if (isInventoryOpen)
        {
            InputManager.Instance.SwitchToUI();

            // 🔹 mostra e libera o cursor
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            InputManager.Instance.SwitchToPlayer();

            // 🔹 esconde e trava o cursor
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void InventoryClose()
    {
        if (!isInventoryOpen)
            return;

        isInventoryOpen = false;

        if (inventoryPanel != null)
            inventoryPanel.SetActive(false);

        Debug.Log("Inventário Fechado via botão UI");

        InputManager.Instance.SwitchToPlayer();

        // 🔹 esconde e trava o cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
