using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI")]
    public TMP_Text itemCountText;
    public UnityEngine.UI.Image itemIcon;

    [Header("Item")]
    public Objects currentItem;
    public int itemCount;

    public void SetItem(Objects item, int count, bool stackable)
    {
        currentItem = item;
        itemCount = count;

        itemIcon.sprite = item.itemSprite;
        itemIcon.enabled = true;

        itemCountText.text = stackable ? count.ToString() : "";
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (currentItem != null)
        {
            TooltipUI.Instance.Show(currentItem.descricaoItem);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipUI.Instance.Hide();
    }
}
