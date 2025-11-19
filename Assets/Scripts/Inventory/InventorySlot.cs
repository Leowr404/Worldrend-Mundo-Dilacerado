using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

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

    // ============================
    // HOVER DELAY
    // ============================

    private float hoverDelay = 2f;          // tempo para exibir tooltip
    private float hoverTimer = 0f;          // cronômetro
    private bool isHovering = false;        // se o mouse está em cima
    private bool tooltipShown = false;      // previne mostrar 2x

    private void Update()
    {
        if (isHovering && !tooltipShown && currentItem != null)
        {
            hoverTimer += Time.deltaTime;

            if (hoverTimer >= hoverDelay)
            {
                tooltipShown = true;

                TooltipUI.Instance.ShowTooltip(
                    currentItem.itemName,
                    currentItem.descricaoItem,
                    transform.position
                );
            }
        }
    }

    // ============================
    // SET / CLEAR
    // ============================

    public void SetItem(Objects newItem, int count, bool stackable)
    {
        currentItem = newItem;
        itemCount = count;
        isStackable = stackable;

        itemIcon.sprite = newItem.itemSprite;
        itemIcon.enabled = true;

        itemCountText.text = (stackable && count > 1) ? count.ToString() : "";
    }

    public void ClearSlot()
    {
        currentItem = null;
        itemCount = 0;
        isStackable = false;

        itemIcon.sprite = null;
        itemIcon.enabled = false;
        itemCountText.text = "";
    }

    // ============================
    // TOOLTIP
    // ============================

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (currentItem == null) return;

        isHovering = true;
        hoverTimer = 0f;
        tooltipShown = false;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        hoverTimer = 0f;
        tooltipShown = false;

        TooltipUI.Instance.HideTooltip();
    }

    // ============================
    // DRAG & DROP
    // ============================

    public void OnPointerDown(PointerEventData eventData)
    {
        if (currentItem == null) return;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (currentItem == null) return;

        // cancela tooltip durante drag
        isHovering = false;
        hoverTimer = 0f;
        tooltipShown = false;
        TooltipUI.Instance.HideTooltip();

        DragItem.Instance.BeginDrag(itemIcon.sprite, this);
    }

    public void OnDrag(PointerEventData eventData)
    {
        // DragItem já cuida do movimento
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        DragItem.Instance.EndDrag();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (DragItem.Instance.sourceSlot == null) return;

        InventorySlot from = DragItem.Instance.sourceSlot;
        if (from == this) return;

        Objects tempItem = currentItem;
        int tempCount = itemCount;
        bool tempStack = isStackable;

        SetItem(from.currentItem, from.itemCount, from.isStackable);

        if (tempItem != null)
            from.SetItem(tempItem, tempCount, tempStack);
        else
            from.ClearSlot();
    }
}
