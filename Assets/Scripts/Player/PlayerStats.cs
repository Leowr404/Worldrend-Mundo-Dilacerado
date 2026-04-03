using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    [Header("Nível e Experiência")]
    public int level = 1;
    public int currentXP = 0;
    public int xpToNextLevel = 100;
    public int statPoints = 0;
    public float xpMultiplier = 1f;

    [Header("Atributos Base")]
    public int strength = 5;
    public int defense = 5;
    public int vitality = 5;
    public int endurance = 5;
    public int Lucky = 5;
    //public int agility = 5;

    [Header("Status Atuais")]
    public int maxHealth;
    public int currentHealth;
    public int maxStamina;
    public int currentStamina;
    public int attackPower;
    public int defensePower;

    [Header("Regeneração")]
    [Tooltip("Quanto de HP regenera a cada intervalo")]
    public int healthRegenAmount = 10;
    [Tooltip("Intervalo entre as curas (em segundos)")]
    public float healthRegenInterval = 5f;
    [Tooltip("Velocidade de regeneração da stamina por segundo")]
    public float staminaRegenRate = 15f;
    public float regenDelay = 1.2f;
    private float nextHealthRegenTime = 0f;
    private float staminaRegenDelayTimer;
    private float lastHitTime;
    public float combatCooldown = 4f;

    [Header("Custos Fixos de Stamina")]
    public int sprintCostPerSecond = 5;
    public int attackCost = 20;
    public int dodgeCost = 30;
    public int jumpCost = 10;

    [Header("Referências de UI")]
    public Slider healthBar;
    public Slider staminaBar;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI xpText;
    public TextMeshProUGUI levelUpText;
   
    [Header("Buffs Temporários")]       // buff que aumenta a stamina por um tempo 60 segundos
    private Coroutine staminaBuffCoroutine;
    private int originalMaxStamina;

    private bool isConsumingStamina = false;

    private void Start()
    {
        RecalculateStats(true);
        UpdateUI();
    }

    private void Update()
    {
        RegenerateHealth();
        RegenerateStamina();
        UpdateUI();

#if UNITY_EDITOR
        RecalculateStats(false);
#endif
    }

    // 🧮 Cálculo de status baseado nos atributos
    public void RecalculateStats(bool refill = false)
    {
        float oldMaxHealth = maxHealth;
        float oldMaxStamina = maxStamina;

        maxHealth = 150 + vitality * 25;
        maxStamina = 300 + endurance * 10;
        attackPower = 15 + strength * 3 + level * 2;
        defensePower = 5 + defense * 2;

        if (!refill)
        {
            float hpPercent = oldMaxHealth > 0 ? (float)currentHealth / oldMaxHealth : 1f;
            float spPercent = oldMaxStamina > 0 ? (float)currentStamina / oldMaxStamina : 1f;

            currentHealth = Mathf.RoundToInt(maxHealth * hpPercent);
            currentStamina = Mathf.RoundToInt(maxStamina * spPercent);
        }
        else
        {
            currentHealth = maxHealth;
            currentStamina = maxStamina;
        }

        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        UpdateUI();
    }

    // ⚔️ Enum para ações que gastam stamina
    public enum StaminaAction { Sprint, Attack, Dodge, Jump }

    public bool SpendStamina(StaminaAction action)
    {
        int cost = action switch
        {
            StaminaAction.Sprint => Mathf.CeilToInt(sprintCostPerSecond * Time.deltaTime),
            StaminaAction.Attack => attackCost,
            StaminaAction.Dodge => dodgeCost,
            StaminaAction.Jump => jumpCost,
            _ => 0
        };

        if (currentStamina < cost)
            return false;

        currentStamina -= cost;
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        isConsumingStamina = true;
        staminaRegenDelayTimer = Time.time + regenDelay;
        UpdateUI();
        return true;
    }

    public void StopConsumingStamina() => isConsumingStamina = false;

    // ⚡ Regeneração contínua de stamina
    private void RegenerateStamina()
    {
        if (isConsumingStamina || Time.time < staminaRegenDelayTimer) return;

        if (currentStamina < maxStamina)
        {
            currentStamina += Mathf.RoundToInt(staminaRegenRate * Time.deltaTime);
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        }
    }

    // ❤️ Regeneração de vida a cada X segundos
    private void RegenerateHealth()
    {
        if (Time.time - lastHitTime > combatCooldown && currentHealth < maxHealth)
        {
            if (Time.time >= nextHealthRegenTime)
            {
                currentHealth += healthRegenAmount;
                currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
                nextHealthRegenTime = Time.time + healthRegenInterval;
            }
        }
    }

    // ☠️ Dano e cura
    public void TakeDamage(int amount)
    {
        int dmg = Mathf.Max(amount - defensePower, 1);
        currentHealth -= dmg;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        lastHitTime = Time.time;
        UpdateUI();
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateUI();
    }

    // 🧾 XP e Level Up
    public void AddXP(int amount)
    {
        currentXP += Mathf.RoundToInt(amount * xpMultiplier);
        if (currentXP >= xpToNextLevel)
            LevelUp();
        UpdateUI();
    }

    private void LevelUp()
    {
        currentXP -= xpToNextLevel;
        level++;
        xpToNextLevel = Mathf.RoundToInt(100 * Mathf.Pow(1.35f, level));
        statPoints += 3;
        RecalculateStats(true);

        if (levelUpText)
        {
            levelUpText.gameObject.SetActive(true);
            levelUpText.text = $"Level {level}!";
            levelUpText.color = Color.yellow;
            levelUpText.alpha = 1;
            levelUpText.transform.localScale = Vector3.one;

            levelUpText.transform.DOScale(1.5f, 0.5f)
                .SetLoops(2, LoopType.Yoyo)
                .SetEase(Ease.OutBack);

            levelUpText.DOFade(0, 1.5f)
                .SetDelay(0.3f)
                .OnComplete(() => levelUpText.gameObject.SetActive(false));
        }
    }

    // 🎛️ Atualização da UI
    private void UpdateUI()
    {
        if (healthBar)
            healthBar.value = (float)currentHealth / maxHealth;
        if (staminaBar)
            staminaBar.value = (float)currentStamina / maxStamina;
        if (levelText)
            levelText.text = $"LVL {level}";
        if (xpText)
            xpText.text = $"{currentXP} / {xpToNextLevel}";
    }

    // ➕ Distribuição de pontos
    public void AddStrength() => AddPoint(ref strength);
    public void AddDefense() => AddPoint(ref defense);
    public void AddVitality() => AddPoint(ref vitality);
    public void AddEndurance() => AddPoint(ref endurance);
    public void AddIntelligence() => AddPoint(ref Lucky);
    //public void AddAgility() => AddPoint(ref agility);

    private void AddPoint(ref int attribute)
    {
        if (statPoints <= 0) return;
        attribute++;
        statPoints--;
        RecalculateStats(false);
    }

    public void ApplyStaminaBuff(int bonus, float duration)     // Método para aplicar um buff temporário de stamina
    {
        Debug.Log("Buff de stamina aplicado");

        // Se já tiver buff ativo, cancela o anterior
        if (staminaBuffCoroutine != null)
        {
            StopCoroutine(staminaBuffCoroutine);
            ResetStaminaBuff();
        }

        staminaBuffCoroutine = StartCoroutine(StaminaBuffRoutine(bonus, duration));
    }

    private IEnumerator StaminaBuffRoutine(int bonus, float duration)   // Coroutine que aplica o buff e depois reseta
    {
        originalMaxStamina = maxStamina;

        maxStamina += bonus;
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);

        Debug.Log("Stamina aumentada para: " + maxStamina);

        yield return new WaitForSeconds(duration);

        ResetStaminaBuff();
    }

    private void ResetStaminaBuff()       // Método para resetar o buff de stamina, voltando ao valor original
    {
        maxStamina = originalMaxStamina;
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);

        Debug.Log("Buff de stamina terminou. Voltou para: " + maxStamina);

        staminaBuffCoroutine = null;
    }
}
