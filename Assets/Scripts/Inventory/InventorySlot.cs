using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class InventorySlot :
    MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerDownHandler,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler,
    IDropHandler
{
    [Header("UI")]
    public Image itemIcon;
    public TMP_Text itemCountText;

    [Header("Item")]
    public Objects currentItem;
    public int itemCount;
    public bool isStackable;

    [Header("Atributos rolados (equipável)")]
    public int rolledDefense;
    public int rolledAttack;

    [Header("Tooltip")]
    public float hoverDelay = 1f;

    private bool pointerOver = false;
    private float hoverStart;
    private bool tooltipVisible = false;
    private bool dragging = false;

    private void Update()
    {
        // Tooltip com delay
        if (pointerOver && !tooltipVisible && !dragging && currentItem != null)
        {
            if (Time.unscaledTime - hoverStart >= hoverDelay)
            {
                TooltipUI.Instance.ShowTooltip(
                    currentItem.itemName,
                    BuildDescription()
                );

                tooltipVisible = true;
            }
        }
    }

    // defense/attack < 0 => rola novo valor (item recém-coletado).
    // >= 0 => usa valor pronto (swap de slots / load de save).
    public void SetItem(Objects newItem, int count, bool stackable, int defense = -1, int attack = -1)
    {
        currentItem = newItem;
        itemCount = count;
        isStackable = stackable;

        if (newItem.itemType == ItemType.Equipment)
        {
            rolledDefense = defense >= 0 ? defense : Random.Range(newItem.minDefense, newItem.maxDefense + 1);
            rolledAttack = attack >= 0 ? attack : Random.Range(newItem.minAttack, newItem.maxAttack + 1);
        }
        else
        {
            rolledDefense = 0;
            rolledAttack = 0;
        }

        itemIcon.sprite = newItem.itemSprite;
        itemIcon.enabled = true;

        itemCountText.text = (stackable && count > 1) ? count.ToString() : "";
    }

    public void ClearSlot()
    {
        currentItem = null;
        itemCount = 0;
        isStackable = false;
        rolledDefense = 0;
        rolledAttack = 0;

        itemIcon.sprite = null;
        itemIcon.enabled = false;
        itemCountText.text = "";
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (currentItem == null) return;

        pointerOver = true;
        hoverStart = Time.unscaledTime;
        tooltipVisible = false;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        pointerOver = false;

        if (tooltipVisible)
        {
            TooltipUI.Instance.HideTooltip();
            tooltipVisible = false;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (currentItem == null) return;

        // BOTÃO DIREITO = abrir Item Menu
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();

            ItemMenuUI.Instance.OpenMenu(
                this,
                mousePos,
                ItemMenuUI.Instance.menuOffset  // <<< AGORA ENVIAMOS O OFFSET
            );
        }
    }

    // ---------- DRAG ----------
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (currentItem == null) return;

        // Somente botão esquerdo pode arrastar
        if (!Mouse.current.leftButton.isPressed)
            return;

        dragging = true;

        if (tooltipVisible)
        {
            TooltipUI.Instance.HideTooltip();
            tooltipVisible = false;
        }

        DragItem.Instance.BeginDrag(itemIcon.sprite, this);
    }

    public void OnDrag(PointerEventData eventData) { }

    public void OnEndDrag(PointerEventData eventData)
    {
        dragging = false;
        DragItem.Instance.EndDrag();
    }

    // Troca de slots
    public void OnDrop(PointerEventData eventData)
    {
        if (DragItem.Instance.sourceSlot == null) return;

        InventorySlot from = DragItem.Instance.sourceSlot;

        if (from == this) return;

        Objects tempItem = currentItem;
        int tempCount = itemCount;
        bool tempStack = isStackable;
        int tempDef = rolledDefense;
        int tempAtk = rolledAttack;

        SetItem(from.currentItem, from.itemCount, from.isStackable, from.rolledDefense, from.rolledAttack);

        if (tempItem != null)
            from.SetItem(tempItem, tempCount, tempStack, tempDef, tempAtk);
        else
            from.ClearSlot();
    }

    public void DeleteItemConfirmed()
    {
        ClearSlot();
    }

    // Monta a descrição do tooltip incluindo atributos do item
    public string BuildDescription()
    {
        string desc = currentItem.descricaoItem;

        if (currentItem.itemType == ItemType.Equipment)
        {
            if (rolledDefense > 0) desc += $"\n<color=#7FD8FF>Defesa +{rolledDefense}</color>";
            if (rolledAttack > 0) desc += $"\n<color=#FF8888>Ataque +{rolledAttack}</color>";
        }
        else if (currentItem.itemType == ItemType.Consumable)
        {
            if (currentItem.healthRestore > 0) desc += $"\n<color=#88FF88>Cura +{currentItem.healthRestore}</color>";
            if (currentItem.staminaRestore > 0) desc += $"\n<color=#FFFF88>Stamina +{currentItem.staminaRestore}</color>";
        }

        return desc;
    }
}
