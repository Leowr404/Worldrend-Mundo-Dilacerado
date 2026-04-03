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
                    currentItem.descricaoItem
                );

                tooltipVisible = true;
            }
        }
    }

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

        SetItem(from.currentItem, from.itemCount, from.isStackable);

        if (tempItem != null)
            from.SetItem(tempItem, tempCount, tempStack);
        else
            from.ClearSlot();
    }
    public void RemoveOneItem()
    {
        Debug.Log("RemoveOneItem entrou");

        if (currentItem == null)
        {
            Debug.LogWarning("currentItem está null em RemoveOneItem");
            return;
        }

        Debug.Log("Antes de remover -> item: " + currentItem.itemName + " | quantidade: " + itemCount);

        if (isStackable && itemCount > 1)
        {
            itemCount--;
            itemCountText.text = itemCount.ToString();
            Debug.Log("Depois de remover -> nova quantidade: " + itemCount);
        }
        else
        {
            Debug.Log("Limpando slot");
            ClearSlot();
        }
    }
    public void DeleteItemConfirmed()
    {
        ClearSlot();
    }
}
