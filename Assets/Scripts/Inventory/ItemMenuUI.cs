using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class ItemMenuUI : MonoBehaviour
{
    public static ItemMenuUI Instance;

    [Header("UI")]
    public GameObject panel;
    public Button useButton;
    public Button descriptionButton;
    public Button moveButton;
    public Button deleteButton;

    [Header("Offset do Menu")]
    public Vector2 menuOffset = new Vector2(40f, -10f); // Pode ajustar no Inspector

    private InventorySlot currentSlot;

    private void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    private void Update()
    {
        if (!panel.activeSelf) return;

        // Fecha menu se clicar fora
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();

            if (!RectTransformUtility.RectangleContainsScreenPoint(
                panel.GetComponent<RectTransform>(),
                mousePos,
                null))
            {
                Close();
            }
        }
    }

    // CHAMADO PELO INVENTORYSLOT
    public void OpenMenu(InventorySlot slot, Vector2 mousePosition, Vector2 offset)
    {
        currentSlot = slot;

        panel.SetActive(true);

        RectTransform rect = panel.GetComponent<RectTransform>();
        rect.position = mousePosition + offset;
    }

    public void Close()
    {
        panel.SetActive(false);
        currentSlot = null;
    }

    // BOTÕES ===============================

    public void OnUseItem()
    {
        Debug.Log("Usou item: " + currentSlot.currentItem.itemName);
        Close();
    }

    public void OnShowDescription()
    {
        TooltipUI.Instance.ShowTooltip(
            currentSlot.currentItem.itemName,
            currentSlot.currentItem.descricaoItem
        );
        Close();
    }

    public void OnMoveItem()
    {
        Debug.Log("Mover item");
        Close();
    }

    public void OnDeleteItem()
    {
        Debug.Log("Item deletado: " + currentSlot.currentItem.itemName);
        currentSlot.ClearSlot();
        Close();
    }
}
