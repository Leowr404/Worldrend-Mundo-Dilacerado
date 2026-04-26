using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [Header("Info")]
    public string enemyName = "Inimigo";
    public string enemyID = "inimigo";
    public int level = 1;

    [Header("Stats Base (nível 1)")]
    public int baseHealth = 100;
    public int baseDefense = 3;
    public int baseAttackPower = 10;
    public int baseXpReward = 50;

    [Header("Escalonamento por nível")]
    public float healthPerLevel = 20f;
    public float defensePerLevel = 1f;
    public float attackPerLevel = 2f;
    public float xpPerLevel = 15f;

    [Header("Stats Calculados (somente leitura)")]
    public int maxHealth;
    public int currentHealth;
    public int defense;
    public int attackPower;
    public int xpReward;

    [Header("Drops")]
    public List<EnemyDrop> drops = new List<EnemyDrop>();

    private void OnValidate()
    {
        ApplyLevelScaling();
    }

    void Start()
    {
        ApplyLevelScaling();
    }

    void ApplyLevelScaling()
    {
        int lvl = Mathf.Max(level - 1, 0);

        maxHealth = baseHealth + Mathf.RoundToInt(healthPerLevel * lvl);
        defense = baseDefense + Mathf.RoundToInt(defensePerLevel * lvl);
        attackPower = baseAttackPower + Mathf.RoundToInt(attackPerLevel * lvl);
        xpReward = baseXpReward + Mathf.RoundToInt(xpPerLevel * lvl);

        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        int dmg = Mathf.Max(amount - defense, 1);
        currentHealth -= dmg;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        PlayerStats player = FindAnyObjectByType<PlayerStats>();
        if (player != null)
            player.AddXP(xpReward);

        QuestManager.Instance.ReportKill(enemyID);
        UiManager.Notify($"{enemyName} derrotado!");

        foreach (var drop in drops)
            drop.TryDrop(transform.position);

        Destroy(gameObject);
    }
}

[System.Serializable]
public class EnemyDrop
{
    public GameObject prefab;
    [Range(0f, 100f)]
    public float chance = 50f;
    public int minAmount = 1;
    public int maxAmount = 1;

    public void TryDrop(Vector3 position)
    {
        if (prefab == null) return;
        if (Random.Range(0f, 100f) > chance) return;

        int amount = Random.Range(minAmount, maxAmount + 1);
        for (int i = 0; i < amount; i++)
        {
            Vector3 offset = new Vector3(
                Random.Range(-0.5f, 0.5f),
                0.5f,
                Random.Range(-0.5f, 0.5f)
            );
            GameObject.Instantiate(prefab, position + offset, Quaternion.identity);
        }
    }
}