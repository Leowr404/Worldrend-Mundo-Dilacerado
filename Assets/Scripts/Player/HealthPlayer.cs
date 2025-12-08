using UnityEngine;
using UnityEngine.UI;

public class HealthPlayer : MonoBehaviour
{
    [Header("Configura��o de Vida")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;

    [Header("UI")]
    [SerializeField] private Slider healthBar;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    // Fun��o para levar dano
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthBar();
    }

    // Fun��o para curar
    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.value = (float)currentHealth / maxHealth;  
        }
        
    }
    private void Update()
    {
        Die();
    }

    public int CurrentHealth => currentHealth;

        public void SetHealth(int newHealth)
    {
        currentHealth = Mathf.Clamp(newHealth, 0, maxHealth);
        UpdateHealthBar();
    }

    void Die() 
    {
        if(currentHealth <= 0)
        {
            Debug.Log("Player Morreu");

        }
    }
}
