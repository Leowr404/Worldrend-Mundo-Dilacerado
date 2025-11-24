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
    public TMP_Text LuckyText;
    //public TMP_Text agilityText;

    [Header("Botões de distribuição")]
    public Button addStrengthButton;
    public Button addDefenseButton;
    public Button addVitalityButton;
    public Button addEnduranceButton;
    public Button addLuckyButton;
    //public Button addAgilityButton;

    private PlayerStats player;

    private void Start()
    {
        player = FindAnyObjectByType<PlayerStats>();
        if (player == null)
        {
            Debug.LogError("⚠️ PlayerStats não encontrado na cena!");
            enabled = false;
            return;
        }

        ConnectButtons();
        UpdateUI();
    }

    private void Update()
    {
        // 🔁 Atualiza em tempo real (quando valores mudam)
        if (player != null)
            UpdateUI();
    }

    private void ConnectButtons()
    {
        addStrengthButton.onClick.AddListener(() => OnAddStat(player.AddStrength));
        addDefenseButton.onClick.AddListener(() => OnAddStat(player.AddDefense));
        addVitalityButton.onClick.AddListener(() => OnAddStat(player.AddVitality));
        addEnduranceButton.onClick.AddListener(() => OnAddStat(player.AddEndurance));
        addLuckyButton.onClick.AddListener(() => OnAddStat(player.AddIntelligence));
        //addAgilityButton.onClick.AddListener(() => OnAddStat(player.AddAgility));
    }

    private void OnAddStat(System.Action addAction)
    {
        addAction.Invoke();
        UpdateUI();
    }

    public void UpdateUI()
    {
        levelText.text = $"Nível: {player.level}";
        xpText.text = $"XP: {player.currentXP}/{player.xpToNextLevel}";
        statPointsText.text = $"Pontos: {player.statPoints}";

        strengthText.text = $"{player.attackPower}";
        defenseText.text = $"{player.defensePower}";
        vitalityText.text = $"{player.maxHealth}";
        enduranceText.text = $"{player.maxStamina}";
        LuckyText.text = $"{player.Lucky}";
        //agilityText.text = $"Agilidade: {player.agility}";

        bool hasPoints = player.statPoints > 0;

        addStrengthButton.interactable = hasPoints;
        addDefenseButton.interactable = hasPoints;
        addVitalityButton.interactable = hasPoints;
        addEnduranceButton.interactable = hasPoints;
        addLuckyButton.interactable = hasPoints;
        //addAgilityButton.interactable = hasPoints;
    }
}
