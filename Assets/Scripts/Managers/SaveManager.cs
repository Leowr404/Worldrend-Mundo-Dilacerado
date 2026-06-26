using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    // setado pelo MainMenu: true = carregar o save 0 ao entrar na cena (Continuar)
    public static bool loadOnStart = false;

    [Header("Referências")]
    public InventorySaver inventorySaver;
    public Transform player;
    public Transform cameraTransform;

    private float playtimeCounter;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        QuestManager.Instance.OnQuestCompleted += OnQuestCompleted;

        // veio do botão "Continuar" no menu
        if (loadOnStart)
        {
            loadOnStart = false;
            LoadFromSlot(0);
        }
    }

    private void OnDestroy()
    {
        if (QuestManager.Instance != null)
            QuestManager.Instance.OnQuestCompleted -= OnQuestCompleted;
    }

    private void Update()
    {
        playtimeCounter += Time.deltaTime;
    }

    private void OnQuestCompleted(Quest quest)
    {
        SaveToSlot(0);
        UiManager.Notify("Jogo salvo automaticamente.");
    }

    // ======================================================
    // SALVAR
    // ======================================================
    public void SaveToSlot(int slot)
    {
        SaveData data = new SaveData();

        // Posição e câmera
        if (player != null) data.playerPosition = player.position;
        if (cameraTransform != null) data.cameraRotation = cameraTransform.rotation;

        // Stats (PlayerStats é o sistema real de vida usado no combate)
        PlayerStats stats = player?.GetComponent<PlayerStats>();
        if (stats != null)
        {
            data.playerHealth = stats.currentHealth;
            data.level = stats.level;
            data.currentXP = stats.currentXP;
            data.xpToNextLevel = stats.xpToNextLevel;
            data.statPoints = stats.statPoints;
            data.strength = stats.strength;
            data.defense = stats.defense;
            data.vitality = stats.vitality;
            data.endurance = stats.endurance;
            data.lucky = stats.Lucky;
        }

        // Moeda
        data.coins = EconomyManager.Instance?.GetMoney() ?? 0;

        // Horário do mundo
        Skyboxspin sky = FindAnyObjectByType<Skyboxspin>();
        if (sky != null)
        {
            data.worldHours = sky.Hours;
            data.worldMinutes = sky.Minutes;
            data.worldDays = sky.Days;
        }

        // Quests completas
        data.completedQuestNames = new List<string>();
        foreach (var q in QuestManager.Instance.completedQuests)
            data.completedQuestNames.Add(q.questName);

        // Quests ativas com progresso
        data.activeQuestProgress = new List<SaveData.QuestProgress>();
        foreach (var q in QuestManager.Instance.activeQuests)
        {
            data.activeQuestProgress.Add(new SaveData.QuestProgress
            {
                questName = q.questName,
                currentAmount = q.objective.currentAmount,
                isReadyToDeliver = q.isReadyToDeliver
            });
        }

        // Inventário
        if (inventorySaver != null)
            data.inventory = inventorySaver.SaveInventory();

        // Equipamento equipado
        if (EquipmentManager.Instance != null)
            data.equipment = EquipmentManager.Instance.SaveEquipment();

        // Tempo e data
        data.playTimeSeconds = Mathf.FloorToInt(playtimeCounter);
        string now = System.DateTime.Now.ToString("dd/MM/yyyy HH:mm");
        data.saveDate = now;
        data.lastSaveDate = now;

        SaveSystem.Save(slot, JsonUtility.ToJson(data, true));
    }

    // ======================================================
    // CARREGAR
    // ======================================================
    public void LoadFromSlot(int slot)
    {
        string json = SaveSystem.Load(slot);
        if (string.IsNullOrEmpty(json)) return;

        SaveData data = JsonUtility.FromJson<SaveData>(json);
        if (data == null) return;

        // Posição
        if (player != null)
        {
            CharacterController cc = player.GetComponent<CharacterController>();
            if (cc) cc.enabled = false;
            player.position = data.playerPosition;
            if (cc) cc.enabled = true;
        }

        // Câmera
        if (cameraTransform != null)
            cameraTransform.rotation = data.cameraRotation;

        // Stats + vida (PlayerStats é o sistema real de combate)
        PlayerStats stats = player?.GetComponent<PlayerStats>();
        if (stats != null)
        {
            stats.level = data.level;
            stats.currentXP = data.currentXP;
            stats.xpToNextLevel = data.xpToNextLevel;
            stats.statPoints = data.statPoints;
            stats.strength = data.strength;
            stats.defense = data.defense;
            stats.vitality = data.vitality;
            stats.endurance = data.endurance;
            stats.Lucky = data.lucky;
            stats.RecalculateStats(false);

            // restaura a vida salva DEPOIS do recalc (senão fica proporcional)
            stats.currentHealth = Mathf.Clamp(data.playerHealth, 0, stats.maxHealth);
        }

        // Moeda
        if (EconomyManager.Instance != null)
        {
            int diff = data.coins - EconomyManager.Instance.GetMoney();
            if (diff > 0) EconomyManager.Instance.AddMoney(diff);
            else if (diff < 0) EconomyManager.Instance.RemoveMoney(-diff);
        }

        // Horário do mundo
        Skyboxspin sky = FindAnyObjectByType<Skyboxspin>();
        if (sky != null)
        {
            sky.Hours = data.worldHours;
            sky.Minutes = data.worldMinutes;
            sky.Days = data.worldDays;
        }

        // Quests — busca ScriptableObjects por nome
        Quest[] allQuests = QuestManager.Instance.allQuests;

        if (allQuests == null || allQuests.Length == 0)
        {
            // não aborta o resto do load (inventário/equipamento continuam)
            Debug.LogError("[SaveManager] allQuests VAZIO! Arraste as quests no Inspector do QuestManager. (quests não serão restauradas)");
        }
        else
        {
            // reseta estado runtime de TODAS as quests antes de restaurar
            foreach (var q in allQuests)
            {
                q.isCompleted = false;
                q.isReadyToDeliver = false;
                q.objective.currentAmount = 0;
            }

            QuestManager.Instance.activeQuests.Clear();
            QuestManager.Instance.completedQuests.Clear();

            foreach (var saved in data.activeQuestProgress)
            {
                foreach (var q in allQuests)
                {
                    if (q.questName == saved.questName)
                    {
                        q.ResetProgress();
                        q.objective.currentAmount = saved.currentAmount;
                        q.isReadyToDeliver = saved.isReadyToDeliver;
                        QuestManager.Instance.activeQuests.Add(q);
                        break;
                    }
                }
            }

            foreach (var name in data.completedQuestNames)
            {
                foreach (var q in allQuests)
                {
                    if (q.questName == name)
                    {
                        q.isCompleted = true;
                        QuestManager.Instance.completedQuests.Add(q);
                        break;
                    }
                }
            }

            // Atualiza UI de quests
            UiManager.Instance?.RefreshQuestHUDPublic();
        }

        // Re-checa as paredes de quest (libera/bloqueia conforme o estado carregado)
        foreach (var barrier in FindObjectsByType<QuestBarrier>(FindObjectsSortMode.None))
            barrier.Refresh();

        // Inventário
        if (inventorySaver != null)
            inventorySaver.LoadInventory(data.inventory);

        // Equipamento equipado (depois dos stats, pra reaplicar bônus por cima)
        if (EquipmentManager.Instance != null)
            EquipmentManager.Instance.LoadEquipment(data.equipment);

        // Tempo de jogo
        playtimeCounter = data.playTimeSeconds;
    }

    // ======================================================
    // UTILITÁRIOS
    // ======================================================
    public SaveData Peek(int slot)
    {
        string json = SaveSystem.Load(slot);
        if (string.IsNullOrEmpty(json)) return null;
        return JsonUtility.FromJson<SaveData>(json);
    }

    public bool SlotExists(int slot) => SaveSystem.Exists(slot);

    public void DeleteSlot(int slot) => SaveSystem.Delete(slot);
}
