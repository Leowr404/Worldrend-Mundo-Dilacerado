using UnityEngine;

public class SpawnItensShop : MonoBehaviour
{
    public Vector2 spawnRange;

    public void Spawn(GameObject itemPrefab)
    {
        Vector3 randPos = new Vector3(
            Random.Range(-spawnRange.x, spawnRange.x),
            0f,
            Random.Range(-spawnRange.y, spawnRange.y)
        );

        // Aqui o item é criado na área
        Instantiate(itemPrefab, transform.position + randPos, Quaternion.identity);
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(spawnRange.x * 2, 0.1f, spawnRange.y * 2));
    }
}
