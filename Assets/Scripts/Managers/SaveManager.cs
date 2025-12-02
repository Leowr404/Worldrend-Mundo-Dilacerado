using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    [Header("Configuração")]
    public int totalSlots = 3; // Número de slots permitidos

    [Header("Referências de sistemas")]
    public InventorySaver inventorySaver;   // arraste no Inspector
    public Transform player;                // arraste o Player no Inspector
    public Transform cameraTransform;       // arraste a câmera se quiser salvar rotação

    // Tempo total de jogo
    private float playtimeCounter = 0f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        // Vamos contar tempo de jogo continuamente
        playtimeCounter += Time.deltaTime;
    }

    // ======================================================
    // SALVAR SLOT
    // ======================================================
    public void SaveToSlot(int slotIndex)
    {
        SaveData data = new SaveData();

        // SALVAR POSIÇÃO DO PLAYER
        if (player != null)
        {
            data.playerPosition = player.position;
        }

        // SALVAR ROTAÇÃO DA CÂMERA (opcional)
        if (cameraTransform != null)
        {
            data.cameraRotation = cameraTransform.rotation;
        }

        // SALVAR INVENTÁRIO
        if (inventorySaver != null)
        {
            data.inventory = inventorySaver.SaveInventory();
        }

        // TEMPO TOTAL DE JOGO
        data.playTimeSeconds = Mathf.FloorToInt(playtimeCounter);

        // DATA E HORA
        string now = System.DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        data.saveDate = now;
        data.lastSaveDate = now;

        // SERIALIZAR
        string json = JsonUtility.ToJson(data, true);

        SaveSystem.Save(slotIndex, json);

        Debug.Log($"💾 Slot {slotIndex} salvo!");
    }

    // ======================================================
    // CARREGAR SLOT
    // ======================================================
    public void LoadFromSlot(int slotIndex)
    {
        string json = SaveSystem.Load(slotIndex);

        if (string.IsNullOrEmpty(json))
        {
            Debug.LogWarning($"⚠️ Nenhum save encontrado no slot {slotIndex}");
            return;
        }

        SaveData data = JsonUtility.FromJson<SaveData>(json);

        if (data == null)
        {
            Debug.LogError("SaveManager: Erro ao decodificar SaveData!");
            return;
        }

        // RESTAURAR POSIÇÃO DO PLAYER
        if (player != null)
        {
            player.position = data.playerPosition;
        }

        // RESTAURAR ROTAÇÃO DA CÂMERA
        if (cameraTransform != null)
        {
            cameraTransform.rotation = data.cameraRotation;
        }

        // RESTAURAR INVENTÁRIO
        if (inventorySaver != null)
        {
            inventorySaver.LoadInventory(data.inventory);
        }

        // RESTAURAR TEMPO DE JOGO
        playtimeCounter = data.playTimeSeconds;

        Debug.Log($"📂 Slot {slotIndex} carregado!");
    }

    // ======================================================
    // PEGAR SOMENTE METADADOS (para UI)
    // ======================================================
    public SaveData Peek(int slotIndex)
    {
        string json = SaveSystem.Load(slotIndex);
        if (string.IsNullOrEmpty(json))
            return null;

        return JsonUtility.FromJson<SaveData>(json);
    }

    // ======================================================
    // DELETAR SLOT
    // ======================================================
    public void DeleteSlot(int slotIndex)
    {
        SaveSystem.Delete(slotIndex);
        Debug.Log($"🗑️ Slot {slotIndex} deletado!");
    }

    // ======================================================
    // VERIFICAR SE UM SLOT POSSUI SAVE
    // ======================================================
    public bool SlotExists(int slotIndex)
    {
        return SaveSystem.Exists(slotIndex);
    }
}
