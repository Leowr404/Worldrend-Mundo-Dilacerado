using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;

// Slot de equipamento. Aceita por drag só itens do tipo correto (slotType).
// Botão direito desequipa, devolvendo a peça pro inventário.
public class EquipmentSlot :
    MonoBehaviour,
    IDropHandler,
    IPointerDownHandler,
    IPointerEnterHandler,
    IPointerExitHandler
{
    [Header("Tipo aceito neste slot")]
    public EquipSlotType slotType;

    [Header("UI")]
    public Image itemIcon;

    [HideInInspector] public Objects currentItem;
    [HideInInspector] public int rolledDefense;
    [HideInInspector] public int rolledAttack;

    private bool tooltipVisible;

    // ---------- EQUIPAR (drop vindo do inventário) ----------
    public void OnDrop(PointerEventData eventData)
    {
        InventorySlot from = DragItem.Instance.sourceSlot;
        Debug.Log($"[EquipSlot {slotType}] OnDrop. sourceSlot={(from == null ? "NULL" : from.name)}");

        if (from == null || from.currentItem == null) { Debug.Log("[EquipSlot] sem item de origem"); return; }

        Debug.Log($"[EquipSlot] item={from.currentItem.itemName} type={from.currentItem.itemType} equipSlot={from.currentItem.equipSlot} | slotType={slotType}");

        // só aceita equipamento do tipo certo
        if (from.currentItem.itemType != ItemType.Equipment) { Debug.Log("[EquipSlot] item não é Equipment"); return; }
        if (from.currentItem.equipSlot != slotType)
        {
            UiManager.Notify("Esse item não vai nesse slot.");
            return;
        }

        // guarda o que já estava equipado (pra trocar)
        Objects oldItem = currentItem;
        int oldDef = rolledDefense;
        int oldAtk = rolledAttack;

        Equip(from.currentItem, from.rolledDefense, from.rolledAttack);

        // devolve a peça antiga pro slot de origem (ou esvazia)
        if (oldItem != null)
            from.SetItem(oldItem, 1, false, oldDef, oldAtk);
        else
            from.ClearSlot();

        EquipmentManager.Instance.RecalculateBonus();
    }

    public void Equip(Objects item, int def, int atk)
    {
        currentItem = item;
        rolledDefense = def;
        rolledAttack = atk;

        itemIcon.sprite = item.itemSprite;
        itemIcon.enabled = true;
    }

    public void Clear()
    {
        currentItem = null;
        rolledDefense = 0;
        rolledAttack = 0;

        itemIcon.sprite = null;
        itemIcon.enabled = false;
    }

    // ---------- DESEQUIPAR (botão direito) ----------
    public void OnPointerDown(PointerEventData eventData)
    {
        if (currentItem == null) return;
        if (!Mouse.current.rightButton.wasPressedThisFrame) return;

        if (EquipmentManager.Instance.ReturnToInventory(currentItem, rolledDefense, rolledAttack))
        {
            if (tooltipVisible) { TooltipUI.Instance.HideTooltip(); tooltipVisible = false; }
            Clear();
            EquipmentManager.Instance.RecalculateBonus();
        }
    }

    // ---------- TOOLTIP ----------
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (currentItem == null) return;

        string desc = currentItem.descricaoItem;
        if (rolledDefense > 0) desc += $"\n<color=#7FD8FF>Defesa +{rolledDefense}</color>";
        if (rolledAttack > 0) desc += $"\n<color=#FF8888>Ataque +{rolledAttack}</color>";

        TooltipUI.Instance.ShowTooltip(currentItem.itemName, desc);
        tooltipVisible = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (tooltipVisible)
        {
            TooltipUI.Instance.HideTooltip();
            tooltipVisible = false;
        }
    }
}
