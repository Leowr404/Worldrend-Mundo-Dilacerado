using System.Collections.Generic;
using UnityEngine;

public class InventorySaver : MonoBehaviour
{
    public static InventorySaver Instance { get; private set; }

    [Header("Inventory")]
    public InventorySlot[] slots;

    private void Awake()
    {
        Instance = this;
    }

    // ============================================================
    // SALVAR INVENTÁRIO
    // ============================================================
    public List<SaveData.InventoryItem> SaveInventory()
    {
        List<SaveData.InventoryItem> list = new List<SaveData.InventoryItem>();

        for (int i = 0; i < slots.Length; i++)
        {
            InventorySlot slot = slots[i];

            if (slot.currentItem != null)
            {
                SaveData.InventoryItem entry = new SaveData.InventoryItem(
                    slotIndex: i,
                    itemId: slot.currentItem.itemId,        // ← ID único
                    itemName: slot.currentItem.itemName,    // ← Nome amigável
                    amount: slot.itemCount
                );

                list.Add(entry);
            }
        }

        return list;
    }

    // ============================================================
    // CARREGAR INVENTÁRIO
    // ============================================================
    public void LoadInventory(List<SaveData.InventoryItem> savedList)
    {
        foreach (var s in slots)
            s.ClearSlot();

        if (savedList == null)
            return;

        foreach (var data in savedList)
        {
            if (data.slotIndex < 0 || data.slotIndex >= slots.Length)
                continue;

            InventorySlot slot = slots[data.slotIndex];

            // 🟩 Agora buscamos pelo ID único
            Objects obj = ItemDatabase.Instance.GetItemById(data.itemId);

            if (obj != null)
            {
                slot.SetItem(obj, data.amount, obj.isStackable);
            }
            else
            {
                Debug.LogWarning($"InventorySaver: itemId '{data.itemId}' não encontrado no ItemDatabase.");
            }
        }
    }
}
