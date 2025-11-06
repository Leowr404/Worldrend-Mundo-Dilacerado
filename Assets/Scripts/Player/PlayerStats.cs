using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    [Header("UI de Level Up")]
    public TextMeshProUGUI levelUpText;
    private float staminaRegenDelayTimer = 0f;
    private bool isConsumingStamina = false;

    [Header("Propriedades de Nível")]
    public int level = 1;
    public int currentXP = 0;
    public int xpToNextLevel = 100;
    public int statPoints = 0;

    [Header("Atributos Base")]
    public int strength = 5;
    public int defense = 5;
    public int vitality = 5;
    public int endurance = 5;
    public int intelligence = 5;
    public int agility = 5;

    [Header("Status Calculados")]
    public float maxHealth;
    public float currentHealth;
    public float maxStamina;
    public float currentStamina;
    public float attackPower;
    public float defensePower;

    [Header("Multiplicadores Globais")]
    public float xpMultiplier = 1.0f;
    public float staminaRegenRate = 10f;   // ⚡ Pode ser 0.1~10 e ainda regenera corretamente
    public float healthRegenRate = 2f;     // ❤️ Regenera mesmo com valores baixos
    private float lastHitTime;
    private float combatCooldown = 5f;

    [Header("Custos de Stamina")]
    public int sprintCostPerSecond = 15;
    public int attackCost = 20;
    public int dodgeCost = 30;

    [Header("Referências de UI")]
    public Slider healthBar;
    public Slider staminaBar;
    public PlayerStatsUI statsUI;
    public Image fadeOverlay;

    [Header("Respawn")]
    public Transform respawnPoint;
    private bool isDead = false;

    private void Start()
    {
        RecalculateStats();
        currentHealth = maxHealth;
        currentStamina = maxStamina;
        UpdateBars();

        if (fadeOverlay)
            fadeOverlay.color = new Color(0, 0, 0, 0);

        if (statsUI) statsUI.UpdateUI(this);
    }

    private void Update()
    {
        RegenerateStamina();
        RegenerateHealth();
        UpdateBars();
#if UNITY_EDITOR
        // 🔁 Atualiza em tempo real se algo for alterado no Inspector
        RecalculateStats(false);
#endif
    }

    // 🧮 Recalcula status com base nos atributos
    public void RecalculateStats(bool refill = false)
    {
        float oldMaxHealth = maxHealth;
        float oldMaxStamina = maxStamina;

        // Calcula novos valores
        maxHealth = 100 + vitality * 25;
        maxStamina = 50 + endurance * 10;
        attackPower = 10 + strength * 3 + (level * 2);
        defensePower = 5 + defense * 2;

        if (!refill)
        {
            float healthPercent = oldMaxHealth > 0 ? currentHealth / oldMaxHealth : 1f;
            float staminaPercent = oldMaxStamina > 0 ? currentStamina / oldMaxStamina : 1f;

            currentHealth = maxHealth * healthPercent;
            currentStamina = maxStamina * staminaPercent;
        }
        else
        {
            currentHealth = maxHealth;
            currentStamina = maxStamina;
        }

        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);

        UpdateBars();
        if (statsUI) statsUI.UpdateUI(this);
    }

    // ⚡ --- SISTEMA DE STAMINA CENTRALIZADO ---
    public enum StaminaAction { Sprint, Attack, Dodge }

    public bool SpendStamina(StaminaAction action)
    {
        int cost = 0;

        switch (action)
        {
            case StaminaAction.Sprint:
                cost = Mathf.CeilToInt(sprintCostPerSecond * Time.deltaTime);
                break;
            case StaminaAction.Attack:
                cost = attackCost;
                break;
            case StaminaAction.Dodge:
                cost = dodgeCost;
                break;
        }

        if (currentStamina < cost)
        {
            Debug.Log($"⚠️ Stamina insuficiente para {action}");
            return false;
        }

        // 🧠 Multiplicador de fadiga — quanto menor a stamina, mais caro agir
        float staminaPercent = currentStamina / maxStamina;
        float fatigueMultiplier = Mathf.Lerp(1.5f, 1.0f, staminaPercent);
        cost = Mathf.CeilToInt(cost * fatigueMultiplier);

        currentStamina -= cost;
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);

        // pausa regeneração
        staminaRegenDelayTimer = Time.time + 1.2f;
        isConsumingStamina = true;

        UpdateBars();
        return true;
    }

    public void StopConsumingStamina() => isConsumingStamina = false;

    // ⚡ Regenera stamina suavemente, sem truncar
    private void RegenerateStamina()
    {
        if (isConsumingStamina || Time.time < staminaRegenDelayTimer)
            return;

        if (currentStamina < maxStamina)
        {
            float regenAmount = staminaRegenRate * Time.deltaTime;
            currentStamina += regenAmount;
            currentStamina = Mathf.Min(currentStamina, maxStamina);
        }
    }

    // 🩸 Dano e cura
    public void TakeDamage(int amount)
    {
        if (isDead) return;

        float damage = Mathf.Max(amount - defensePower, 1);
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        lastHitTime = Time.time;

        UpdateBars();
        if (currentHealth <= 0)
            Die();
    }

    public void Heal(float amount)
    {
        if (isDead) return;

        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        UpdateBars();
    }

    // 🩹 Regenera HP fora de combate (contínuo)
    private void RegenerateHealth()
    {
        if (isDead) return;

        if (Time.time - lastHitTime > combatCooldown && currentHealth < maxHealth)
        {
            float regenAmount = healthRegenRate * Time.deltaTime;
            currentHealth += regenAmount;
            currentHealth = Mathf.Min(currentHealth, maxHealth);
        }
    }

    private void UpdateBars()
    {
        if (healthBar)
        {
            healthBar.maxValue = 1f;
            healthBar.value = currentHealth / maxHealth;
        }

        if (staminaBar)
        {
            staminaBar.maxValue = 1f;
            staminaBar.value = currentStamina / maxStamina;
        }
    }

    // --- XP / LEVEL UP ---
    public void AddXP(int amount)
    {
        currentXP += Mathf.RoundToInt(amount * xpMultiplier);
        if (currentXP >= xpToNextLevel)
            LevelUp();
        if (statsUI) statsUI.UpdateUI(this);
    }

    private void LevelUp()
    {
        currentXP -= xpToNextLevel;
        level++;
        xpToNextLevel = Mathf.RoundToInt(100 * Mathf.Pow(1.35f, level));
        statPoints += 3;

        RecalculateStats(true);
        if (statsUI) statsUI.UpdateUI(this);

        transform.DOScale(1.1f, 0.25f).SetLoops(2, LoopType.Yoyo);
        Debug.Log($"✨ Subiu para o nível {level}!");

        if (levelUpText != null)
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

    // 💀 Morte e Respawn
    private void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("💀 Player morreu!");
        StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        InputManager.Instance.SwitchToUI();

        if (fadeOverlay)
            fadeOverlay.DOFade(1f, 1.2f);

        yield return new WaitForSeconds(2.5f);
        Respawn();

        if (fadeOverlay)
            fadeOverlay.DOFade(0f, 1f);
    }

    private void Respawn()
    {
        Debug.Log("🔄 Player respawnado!");
        currentHealth = maxHealth;
        currentStamina = maxStamina;
        isDead = false;

        transform.position = respawnPoint ? respawnPoint.position : Vector3.zero;
        InputManager.Instance.SwitchToPlayer();
        StartCoroutine(TemporaryInvulnerability());
        UpdateBars();
    }

    private IEnumerator TemporaryInvulnerability()
    {
        float duration = 2f;
        float blinkRate = 0.2f;
        Renderer[] rends = GetComponentsInChildren<Renderer>();

        for (float t = 0; t < duration; t += blinkRate)
        {
            foreach (Renderer r in rends)
                r.enabled = !r.enabled;
            yield return new WaitForSeconds(blinkRate);
        }

        foreach (Renderer r in rends)
            r.enabled = true;
    }

    // ⚙️ Pontos de atributo
    public void AddStrength() => AddPoint(ref strength);
    public void AddDefense() => AddPoint(ref defense);
    public void AddVitality() => AddPoint(ref vitality);
    public void AddEndurance() => AddPoint(ref endurance);
    public void AddIntelligence() => AddPoint(ref intelligence);
    public void AddAgility() => AddPoint(ref agility);

    private void AddPoint(ref int attribute)
    {
        if (statPoints <= 0) return;
        attribute++;
        statPoints--;
        RecalculateStats(false);
        if (statsUI) statsUI.UpdateUI(this);
    }
}
