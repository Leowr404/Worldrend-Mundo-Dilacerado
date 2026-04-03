using UnityEngine;
using UnityEngine.UI;

public class HealthPlayer : MonoBehaviour
{
    [Header("Configuracao de Vida")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;

    [Header("UI")]
    [SerializeField] private Slider healthBar;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    // Funcao para levar dano
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthBar();
    }

    // Funcao para curar
    public void Heal(int amount)
    {

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);


        UpdateHealthBar();
    }

    private void UpdateHealthBar() // Atualiza a barra de vida

    {
        if (healthBar == null)
        {
            Debug.LogWarning("healthBar está NULL");
            return;
        }

        healthBar.value = currentHealth;

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
            //Debug.Log("Player Morreu");

        }
    }
}
