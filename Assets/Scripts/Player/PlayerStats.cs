using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    [Header("UI de Level Up")]
    public TextMeshProUGUI levelUpText;

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
    public int maxHealth;
    public int currentHealth;
    public int maxStamina;
    public int currentStamina;
    public int attackPower;
    public int defensePower;

    [Header("Multiplicadores Globais")]
    public float xpMultiplier = 1.0f;
    public float staminaRegenRate = 10f;
    public float healthRegenRate = 2f; // 🩸 HP/s fora de combate
    private float lastHitTime;
    private float combatCooldown = 5f; // tempo sem levar dano pra começar a curar

    [Header("Referências de UI")]
    public Slider healthBar;
    public Slider staminaBar;
    public PlayerStatsUI statsUI;
    public Image fadeOverlay; // imagem preta na tela para fade

    [Header("Respawn")]
    public Transform respawnPoint; // ponto de respawn configurado
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
    }

    // 🧮 Recalcula status com base nos atributos
    public void RecalculateStats()
    {
        maxHealth = 100 + vitality * 25;
        maxStamina = 50 + endurance * 10;
        attackPower = 10 + strength * 3 + (level * 2);
        defensePower = 5 + defense * 2;

        if (currentHealth > maxHealth) currentHealth = maxHealth;
        if (currentStamina > maxStamina) currentStamina = maxStamina;
    }

    // 🩸 Dano e cura
    public void TakeDamage(int amount)
    {
        if (isDead) return;

        int damage = Mathf.Max(amount - defensePower, 1);
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        lastHitTime = Time.time;

        UpdateBars();

        if (currentHealth <= 0)
            Die();
    }

    public void Heal(int amount)
    {
        if (isDead) return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateBars();
    }

    // 🩹 Regenera HP gradualmente fora de combate
    private void RegenerateHealth()
    {
        if (Time.time - lastHitTime > combatCooldown && currentHealth < maxHealth && !isDead)
        {
            currentHealth += Mathf.RoundToInt(healthRegenRate * Time.deltaTime);
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        }
    }

    // ⚡ Regenera stamina com o tempo
    private void RegenerateStamina()
    {
        if (currentStamina < maxStamina)
        {
            currentStamina += Mathf.RoundToInt(staminaRegenRate * Time.deltaTime);
            if (currentStamina > maxStamina)
                currentStamina = maxStamina;
        }
    }

    // 🧃 Atualiza as barras
    private void UpdateBars()
    {
        if (healthBar)
            healthBar.value = (float)currentHealth / maxHealth;
        if (staminaBar)
            staminaBar.value = (float)currentStamina / maxStamina;
    }

    // ⚡ Gastar stamina
    public bool UseStamina(int cost)
    {
        if (currentStamina < cost)
        {
            Debug.Log("⚠️ Sem stamina suficiente!");
            return false;
        }

        currentStamina -= cost;
        UpdateBars();
        return true;
    }

    // 💀 Morte do jogador
    private void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("💀 Player morreu!");
        StartCoroutine(DeathSequence());
    }

    // ☠️ Sequência de morte e respawn
    private IEnumerator DeathSequence()
    {
        // desativa controle do player
        InputManager.Instance.SwitchToUI();

        // fade para preto
        if (fadeOverlay)
            fadeOverlay.DOFade(1f, 1.2f);

        // tempo da “morte”
        yield return new WaitForSeconds(2.5f);

        Respawn();

        // fade de volta
        if (fadeOverlay)
            fadeOverlay.DOFade(0f, 1f);
    }

    // 🔁 Respawn
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

    // ⛨ Invulnerabilidade breve ao reviver
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

    // 🏆 XP e Level Up
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
        xpToNextLevel = Mathf.RoundToInt(100 * Mathf.Pow(1.25f, level));
        statPoints += 3;

        // ⚙️ Atualiza atributos e UI
        RecalculateStats();
        if (statsUI) statsUI.UpdateUI(this);

        // ✨ Animação de "Level Up"
        transform.DOScale(1.1f, 0.25f).SetLoops(2, LoopType.Yoyo);
        Debug.Log($"✨ Subiu para o nível {level}!");

        // 🧾 Mostra texto de Level Up na tela
        if (levelUpText != null)
        {
            levelUpText.gameObject.SetActive(true);
            levelUpText.text = $"Level {level}!";
            levelUpText.color = Color.yellow;
            levelUpText.alpha = 1;

            // Efeito com DOTween
            levelUpText.transform.localScale = Vector3.one;
            levelUpText.transform.DOScale(1.5f, 0.5f)
                .SetLoops(2, LoopType.Yoyo)
                .SetEase(Ease.OutBack);

            levelUpText.DOFade(0, 1.5f)
                .SetDelay(0.3f)
                .OnComplete(() => levelUpText.gameObject.SetActive(false));
        }
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
        RecalculateStats();
        if (statsUI) statsUI.UpdateUI(this);
    }
}
