using UnityEngine;

public class Destructible : MonoBehaviour
{
    [Header("Info")]
    public string objectName = "Objeto";

    [Header("Vida")]
    public int maxHealth = 50;
    public int currentHealth;

    [Header("Ao Destruir")]
    public GameObject dropPrefab;       // item que cai ao destruir (opcional)
    public Transform dropSpawnPoint;    // onde o item aparece (opcional)

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;        // objeto não tem defesa, dano direto
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log($"{objectName} recebeu {amount} de dano! HP: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
            Break();
    }

    void Break()
    {
        Debug.Log($"{objectName} foi destruído!");

        // Spawna drop se tiver configurado
        if (dropPrefab != null)
        {
            Vector3 spawnPos = dropSpawnPoint != null ? dropSpawnPoint.position : transform.position;
            Instantiate(dropPrefab, spawnPos, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}