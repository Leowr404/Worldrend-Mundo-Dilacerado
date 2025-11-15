using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler,
    IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [Header("Referências de UI")]
    public Image itemIcon;
    public TMP_Text itemNameText;
    public TMP_Text itemCountText;

    [Header("Dados do Slot")]
    public Objects currentItem;
    public int itemCount;
    public bool isStackable;

    // máximo empilhável (configurável)
    public static int maxStack = 5;

    // ---------------------------
    // Set / Clear
    // ---------------------------
    public void SetItem(Objects newItem, int count = 1, bool stackable = false)
    {
        currentItem = newItem;
        itemCount = count;
        isStackable = stackable;

        if (newItem != null)
        {
            itemIcon.sprite = newItem.itemSprite;
            itemIcon.enabled = true;
            itemNameText.text = newItem.itemName;
        }
        else
        {
            itemIcon.sprite = null;
            itemIcon.enabled = false;
            itemNameText.text = "";
        }

        itemCountText.text = (isStackable && itemCount > 1) ? itemCount.ToString() : "";
    }

    public void ClearSlot()
    {
        currentItem = null;
        itemCount = 0;
        isStackable = false;

        itemIcon.sprite = null;
        itemIcon.enabled = false;
        itemNameText.text = "";
        itemCountText.text = "";
    }

    // ---------------------------
    // Tooltip (já existente)
    // ---------------------------
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (currentItem == null) return;

        string text = $"{currentItem.itemName}\n\n{currentItem.descricaoItem}";
        if (isStackable)
            text += $"\n\nQuantidade: {itemCount}";

        TooltipUI.Instance.Show(text);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipUI.Instance.Hide();
    }

    // ---------------------------
    // Drag & Drop
    // ---------------------------
    public void OnBeginDrag(PointerEventData eventData)
    {
        // só inicia se houver item
        if (currentItem == null) return;

        // começa visual de arrasto
        DragItem.Instance.BeginDrag(itemIcon.sprite, this);
    }

    public void OnDrag(PointerEventData eventData)
    {
        // nada extra aqui — o DragItem já atualiza a posição no Update()
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // se não houve drop válido, apenas termina o arrasto sem alterar os slots
        DragItem.Instance.EndDrag();
    }

    public void OnDrop(PointerEventData eventData)
    {
        // quando outro slot solta em cima deste slot
        InventorySlot from = DragItem.Instance.sourceSlot;
        InventorySlot to = this;

        if (from == null) return; // nada para fazer

        // mesma referência: soltar no mesmo slot -> nada
        if (from == to)
        {
            DragItem.Instance.EndDrag();
            return;
        }

        // Caso 1: to está vazio -> move tudo do from para to
        if (to.currentItem == null)
        {
            to.SetItem(from.currentItem, from.itemCount, from.isStackable);
            from.ClearSlot();
            DragItem.Instance.EndDrag();
            return;
        }

        // Caso 2: mesmo item e empilhável -> tenta empilhar
        if (to.currentItem == from.currentItem && from.isStackable && to.isStackable)
        {
            int space = maxStack - to.itemCount;
            if (space <= 0)
            {
                // sem espaço, faz swap
                SwapSlots(from, to);
            }
            else if (from.itemCount <= space)
            {
                // cabe tudo
                to.itemCount += from.itemCount;
                to.itemCountText.text = to.itemCount.ToString();
                from.ClearSlot();
            }
            else
            {
                // cabe parcialmente
                to.itemCount = maxStack;
                to.itemCountText.text = to.itemCount.ToString();
                from.itemCount -= space;
                from.itemCountText.text = from.itemCount.ToString();
            }

            DragItem.Instance.EndDrag();
            return;
        }

        // Caso 3: itens diferentes -> swap
        SwapSlots(from, to);
        DragItem.Instance.EndDrag();
    }

    private void SwapSlots(InventorySlot a, InventorySlot b)
    {
        // salva A
        Objects aItem = a.currentItem;
        int aCount = a.itemCount;
        bool aStack = a.isStackable;

        // move B para A
        a.SetItem(b.currentItem, b.itemCount, b.isStackable);

        // move A salvo para B
        b.SetItem(aItem, aCount, aStack);
    }
}
