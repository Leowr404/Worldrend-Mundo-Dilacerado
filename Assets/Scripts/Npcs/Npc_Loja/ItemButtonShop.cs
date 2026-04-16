using UnityEngine;

public class ItemButtonShop : MonoBehaviour
{
    public GameObject itemPrefab;           // O item desse botão
    public SpawnItensShop spawnArea;         // Referência da área de spawn

    public void SpawnItem()
    {
            spawnArea.Spawn(itemPrefab);    // 🔥 SPAWNA O ITEM ESPECÍFICO   
    }
}
