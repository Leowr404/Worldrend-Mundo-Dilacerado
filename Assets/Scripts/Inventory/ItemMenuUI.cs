using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class ItemMenuUI : MonoBehaviour
{
    public static ItemMenuUI Instance;

    [Header("UI")]
    public GameObject panel;
    public Button useButton;
    public Button descriptionButton;
    public Button deleteButton;

    [Header("Offset do menu")]
    public Vector2 menuOffset = new Vector2(150f, -20f);

    [Header("Referências")]
    [SerializeField] private PlayerStats playerStats; // Aqui chamamos o PlayerStats para usar o método Heal

    private InventorySlot currentSlot;

    private void Awake()
    {
        Instance = this;

        if (panel != null)
            panel.SetActive(false);
    }

    private void Update()
    {
        if (panel == null || !panel.activeSelf)
            return;

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
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

        if (currentSlot == null || currentSlot.currentItem == null)
        {
            Debug.LogWarning("OpenMenu recebeu slot nulo ou item nulo.");
            return;
        }

        panel.SetActive(true);

        RectTransform rect = panel.GetComponent<RectTransform>();
        rect.position = mousePos + offset;
    }

    public void Close()
    {
        if (panel != null)
            panel.SetActive(false);

        currentSlot = null;
    }

    //  BOTÃO USAR
    public void OnUseItem()
    {

        if (currentSlot == null)
        {
            return;
        }

        if (currentSlot.currentItem == null)
        {
            return;
        }

        Objects item = currentSlot.currentItem;

        switch (item.itemType)
        {
            case ItemType.Consumable:
                UseConsumable(item);
                break;

            case ItemType.Weapon:
                Debug.Log("Equipar arma (futuro)");
                break;

            case ItemType.Armor:
                Debug.Log("Equipar armadura (futuro)");
                break;

            default:
                Debug.LogWarning("Item não utilizável");
                break;
        }

        Close();
    }

    //  USO DE CONSUMÍVEL 
    private void UseConsumable(Objects item)
    {
        if (playerStats == null)
        {
            Debug.LogError("PlayerStats não atribuído!");
            return;
        }

        // Cura
        if (item.healAmount > 0)
        {
            playerStats.Heal(item.healAmount);
        }

        // Buff de stamina
        if (item.staminaBonus > 0 && item.staminaDuration > 0)
        {
            playerStats.ApplyStaminaBuff(item.staminaBonus, item.staminaDuration);
        }

        currentSlot.RemoveOneItem();
    }

    //  DESCRIÇÃO
    public void OnShowDescription()
    {
        if (currentSlot == null || currentSlot.currentItem == null)
        {
            return;
        }

        TooltipUI.Instance.ShowTooltip(
            currentSlot.currentItem.itemName,
            currentSlot.currentItem.descricaoItem
        );

        Close();
    }

    //  DELETAR
    public void OnDeleteItem()
    {
        if (currentSlot == null || currentSlot.currentItem == null)
        {
            return;
        }

        DeleteItemConfirmPanel.Instance.OpenConfirm(currentSlot);
        Close();
    }
}