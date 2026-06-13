using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.EventSystems;

public class ItemMenuUI : MonoBehaviour
{
    public static ItemMenuUI Instance;

    [Header("UI")]
    public GameObject panel;
    public Button useButton;
    public Button descriptionButton;
    public Button deleteButton;

    [Header("Offset do Menu")]
    public Vector2 menuOffset = new Vector2(150f, -20f);

    private InventorySlot currentSlot;

    private void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    private void Update()
    {
        if (!panel.activeSelf) return;

        // Fechar ao clicar fora
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

    public void OpenMenu(InventorySlot slot, Vector2 mousePos, Vector2 offset)
    {
        currentSlot = slot;

        panel.SetActive(true);

        RectTransform rect = panel.GetComponent<RectTransform>();
        rect.position = mousePos + offset;
    }

    public void Close()
    {
        panel.SetActive(false);
        currentSlot = null;
    }

    // ============================
    // BOTÕES
    // ============================

    public void OnUseItem()
    {
        if (currentSlot == null || currentSlot.currentItem == null) { Close(); return; }

        Objects item = currentSlot.currentItem;

        if (item.itemType == ItemType.Consumable)
        {
            PlayerStats player = FindAnyObjectByType<PlayerStats>();
            if (player != null)
            {
                if (item.healthRestore > 0) player.Heal(item.healthRestore);
                if (item.staminaRestore > 0) player.RestoreStamina(item.staminaRestore);
            }

            // consome 1 unidade
            currentSlot.itemCount--;
            if (currentSlot.itemCount <= 0)
                currentSlot.ClearSlot();
            else
                currentSlot.itemCountText.text = currentSlot.itemCount.ToString();

            UiManager.Notify($"{item.itemName} usado!");
        }
        else if (item.itemType == ItemType.Equipment)
        {
            UiManager.Notify("Arraste a peça para o slot de equipamento.");
        }
        else
        {
            UiManager.Notify("Esse item não pode ser usado.");
        }

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
    public void OnDeleteItem()
    {
        //  Agora usa confirmação!
        DeleteItemConfirmPanel.Instance.OpenConfirm(currentSlot);
        Close();
    }
}
