using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatsUI : MonoBehaviour
{
    [Header("Nível e XP")]
    public TMP_Text levelText;
    public TMP_Text xpText;
    public TMP_Text statPointsText;

    [Header("Atributo Base (esquerda - o 10)")]
    public TMP_Text strengthText;
    public TMP_Text defenseText;
    public TMP_Text vitalityText;
    public TMP_Text enduranceText;
    public TMP_Text luckyText;

    [Header("Resultado Calculado (direita - o 400)")]
    public TMP_Text strengthResultText;   // attackPower
    public TMP_Text defenseResultText;    // defensePower
    public TMP_Text vitalityResultText;   // maxHealth
    public TMP_Text enduranceResultText;  // maxStamina
    public TMP_Text luckyResultText;      // Lucky (sem fórmula ainda)

    [Header("Botões de distribuição")]
    public Button addStrengthButton;
    public Button addDefenseButton;
    public Button addVitalityButton;
    public Button addEnduranceButton;
    public Button addLuckyButton;

    private PlayerStats player;

    private void Start()
    {
        player = FindAnyObjectByType<PlayerStats>();
        if (player == null) { enabled = false; return; }
        ConnectButtons();
        UpdateUI();
    }

    private void Update()
    {
        if (player != null) UpdateUI();
    }

    private void ConnectButtons()
    {
        addStrengthButton.onClick.AddListener(() => OnAddStat(player.AddStrength));
        addDefenseButton.onClick.AddListener(() => OnAddStat(player.AddDefense));
        addVitalityButton.onClick.AddListener(() => OnAddStat(player.AddVitality));
        addEnduranceButton.onClick.AddListener(() => OnAddStat(player.AddEndurance));
        addLuckyButton.onClick.AddListener(() => OnAddStat(player.AddIntelligence));
    }

    private void OnAddStat(System.Action addAction)
    {
        addAction.Invoke();
        UpdateUI();
    }

    public void UpdateUI()
    {
        // Nível e XP
        levelText.text = $"Nível: {player.level}";
        xpText.text = $"XP: {player.currentXP}/{player.xpToNextLevel}";
        statPointsText.text = $"Pontos: {player.statPoints}";

        // Atributo base (esquerda)
        strengthText.text = $"{player.strength}";
        defenseText.text = $"{player.defense}";
        vitalityText.text = $"{player.vitality}";
        enduranceText.text = $"{player.endurance}";
        luckyText.text = $"{player.Lucky}";

        // Resultado calculado (direita)
        if (strengthResultText) strengthResultText.text = $"{player.attackPower}";
        if (defenseResultText) defenseResultText.text = $"{player.defensePower}";
        if (vitalityResultText) vitalityResultText.text = $"{player.maxHealth}";
        if (enduranceResultText) enduranceResultText.text = $"{player.maxStamina}";
        if (luckyResultText) luckyResultText.text = $"{player.Lucky}";

        // Botões
        bool hasPoints = player.statPoints > 0;
        addStrengthButton.interactable = hasPoints;
        addDefenseButton.interactable = hasPoints;
        addVitalityButton.interactable = hasPoints;
        addEnduranceButton.interactable = hasPoints;
        addLuckyButton.interactable = hasPoints;
    }
}