using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [Header("Info")]
    public string enemyName = "Inimigo";
    public int level = 1;

    [Header("Vida")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Atributos")]
    public int defense = 3;
    public int attackPower = 10;
    public int xpReward = 50;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        int dmg = Mathf.Max(amount - defense, 1); // defesa reduz, mínimo 1
        currentHealth -= dmg;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log($"{enemyName} recebeu {dmg} de dano! HP: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        Debug.Log($"{enemyName} morreu!");

        // Dá XP pro player ao morrer
        PlayerStats player = FindObjectOfType<PlayerStats>();
        if (player != null)
            player.AddXP(xpReward);

        Destroy(gameObject);
    }
}