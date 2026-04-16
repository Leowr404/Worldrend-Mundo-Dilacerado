using System.Collections.Generic;
using UnityEngine;

public class ItemQuestGenerator : MonoBehaviour
{
    [Header("Configurações de Spawn")]
    public GameObject itemPrefab;       // Prefab do item (Maçã, Gold etc)
    public int maxItems = 5;            // Quantidade máxima do item na área
    public Vector2 spawnRange = new Vector2(3f, 3f); // Tamanho da área de spawn (X,Z)

    private List<GameObject> spawnedItems = new List<GameObject>();

    void Start()
    {
        SpawnInitial();
    }

    void Update()
    {
        CheckRespawn();
    }

    void SpawnInitial()
    {
        for (int i = 0; i < maxItems; i++)
        {
            SpawnItem();
        }
    }

    void CheckRespawn()
    {
        // remove objetos que foram coletados (ficaram null)
        spawnedItems.RemoveAll(item => item == null);

        if (spawnedItems.Count < maxItems)
        {
            SpawnItem();
        }
    }

    void SpawnItem()
    {
        Vector3 randPos = new Vector3(
            Random.Range(-spawnRange.x, spawnRange.x),
            0f,
            Random.Range(-spawnRange.y, spawnRange.y)
        );

        GameObject obj = Instantiate(itemPrefab, transform.position + randPos, Quaternion.identity);
        spawnedItems.Add(obj);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(spawnRange.x * 2, 0.1f, spawnRange.y * 2));
    }
}
