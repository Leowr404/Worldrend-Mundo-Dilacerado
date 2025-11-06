using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatsUI : MonoBehaviour
{
    [Header("Referências de texto")]
    public TMP_Text levelText;
    public TMP_Text xpText;
    public TMP_Text statPointsText;
    public TMP_Text strengthText;
    public TMP_Text defenseText;
    public TMP_Text vitalityText;
    public TMP_Text enduranceText;
    public TMP_Text intelligenceText;
    public TMP_Text agilityText;

    [Header("Botões de distribuição")]
    public Button addStrengthButton;
    public Button addDefenseButton;
    public Button addVitalityButton;
    public Button addEnduranceButton;
    public Button addIntelligenceButton;
    public Button addAgilityButton;

    private PlayerStats player;

    private void Start()
    {
        player = FindAnyObjectByType<PlayerStats>();
        ConnectButtons();
        UpdateUI(player);
    }

    private void ConnectButtons()
    {
        addStrengthButton.onClick.AddListener(() => player.AddStrength());
        addDefenseButton.onClick.AddListener(() => player.AddDefense());
        addVitalityButton.onClick.AddListener(() => player.AddVitality());
        addEnduranceButton.onClick.AddListener(() => player.AddEndurance());
        addIntelligenceButton.onClick.AddListener(() => player.AddIntelligence());
        addAgilityButton.onClick.AddListener(() => player.AddAgility());
    }

    public void UpdateUI(PlayerStats stats)
    {
        levelText.text = $"Nível: {stats.level}";
        xpText.text = $"XP: {stats.currentXP}/{stats.xpToNextLevel}";
        statPointsText.text = $"Pontos: {stats.statPoints}";

        strengthText.text = $"Força: {stats.strength}";
        defenseText.text = $"Defesa: {stats.defense}";
        vitalityText.text = $"Vitalidade: {stats.vitality}";
        enduranceText.text = $"Vigor: {stats.endurance}";
        intelligenceText.text = $"Inteligência: {stats.intelligence}";
        agilityText.text = $"Agilidade: {stats.agility}";

        bool hasPoints = stats.statPoints > 0;
        addStrengthButton.interactable = hasPoints;
        addDefenseButton.interactable = hasPoints;
        addVitalityButton.interactable = hasPoints;
        addEnduranceButton.interactable = hasPoints;
        addIntelligenceButton.interactable = hasPoints;
        addAgilityButton.interactable = hasPoints;
    }
}
