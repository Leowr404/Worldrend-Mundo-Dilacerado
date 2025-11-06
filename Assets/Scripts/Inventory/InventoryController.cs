using UnityEngine;
using System.Collections;

public class InventoryController : MonoBehaviour
{
    [SerializeField] private GameObject inventoryPanel;

    private bool isInventoryOpen = false;
    private float lastToggleTime = 0f;
    private float toggleCooldown = 0.25f;

    private void Start()
    {
            inventoryPanel.SetActive(false);

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
            StartCoroutine(ToggleInventoryDelayed()); // 🔹 usa Coroutine pra garantir troca suave
            lastToggleTime = Time.time;
        }
    }

    private IEnumerator ToggleInventoryDelayed()
    {
        // 🔸 Espera 1 frame pra evitar conflito entre mapas de input
        yield return null;
        ToggleInventory();
    }

    private void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;

        if (inventoryPanel != null)
            inventoryPanel.SetActive(isInventoryOpen);

        Debug.Log(isInventoryOpen ? "Inventário Aberto" : "Inventário Fechado");

        if (isInventoryOpen)
        {
            // 🔹 troca de mapa levemente atrasada pra não perder o input
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

    public void InventoryClose()
    {
        if (!isInventoryOpen)
            return;

        isInventoryOpen = false;

        if (inventoryPanel != null)
            inventoryPanel.SetActive(false);

        Debug.Log("Inventário Fechado via botão UI");

        InputManager.Instance.SwitchToPlayer();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
