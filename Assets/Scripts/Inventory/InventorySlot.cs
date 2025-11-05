using UnityEngine;
using UnityEngine.UI;
using TMPro; // use isso se estiver usando TextMeshPro

public class InventorySlot : MonoBehaviour
{
    [Header("Partes visuais do Slot")]
    public Image itemIcon;        // mostra a imagem do item
    public TMP_Text itemNameText; // mostra o nome do item
    public TMP_Text itemCountText;// mostra a quantidade

    [Header("Dados do item")]
    public Objects currentItem;   // referência ao ScriptableObject do item
    public int itemCount;         // quantidade atual
    public bool isStackable;      // se o item pode empilhar

    // Esse método é usado pra preencher o slot
    public void SetItem(Objects newItem, int count = 1, bool stackable = false)
    {
        currentItem = newItem;
        itemCount = count;
        isStackable = stackable;

        // Atualiza as partes visuais
        itemIcon.sprite = newItem.itemSprite;
        itemIcon.enabled = true;

        itemNameText.text = newItem.itemName;

        if (stackable && count > 1)
            itemCountText.text = count.ToString();
        else
            itemCountText.text = "";
    }

    // Limpa o slot quando for remover o item
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
}
