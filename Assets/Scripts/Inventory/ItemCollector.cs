using UnityEngine;

public class ItemCollector : MonoBehaviour
{
    [Header("Referência ao Inventário")]
    public InventorySlot[] inventorySlots; // arraste seus slots aqui no Inspector
    public int maxStack = 5; // máximo de itens empilháveis
    AudioManager audioManager;
    public void Start()
    {
        audioManager = AudioManager.instancia;
    }

    private void OnTriggerEnter(Collider other)
    {
        // verifica se o objeto tem o script ObjectType
        ObjectType objectType = other.GetComponent<ObjectType>();
        if (objectType == null) return;

        Objects item = objectType.objecType; // pega o ScriptableObject
        if (item == null) return;

        // tenta adicionar o item ao inventário
        bool added = AddItemToInventory(item);

        if (added)
        {
            Debug.Log($"Item coletado: {item.itemName}");
            audioManager.PlaySFX(AudioManager.instancia.Coletavel);
            Destroy(other.gameObject); // remove o item da cena
        }
        else
        {
            Debug.Log("Inventário cheio! Não foi possível coletar o item.");
        }
    }

    private bool AddItemToInventory(Objects item)
    {
        // 🔹 1. Verifica se o item já existe e pode empilhar
        if (item.isStackable)
        {
            foreach (var slot in inventorySlots)
            {
                if (slot.currentItem == item && slot.itemCount < maxStack)
                {
                    slot.itemCount++;
                    slot.itemCountText.text = slot.itemCount.ToString();
                    return true;
                }
            }
        }

        // 🔹 2. Procura um slot vazio
        foreach (var slot in inventorySlots)
        {
            if (slot.currentItem == null)
            {
                slot.SetItem(item, 1, item.isStackable);
                return true;
            }
        }

        // 🔹 3. Se não couber em lugar nenhum
        return false;
    }
}
