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
        Instance = this;
    }

    private void Update()
    {
        playtimeCounter += Time.deltaTime;
    }

    // ======================================================
    // SALVAR SLOT
    // ======================================================
    public void SaveToSlot(int slotIndex)
    {
        SaveData data = new SaveData();

        // SALVAR VIDA DO PLAYER
        HealthPlayer hp = player.GetComponent<HealthPlayer>();
        if (hp != null)
        {
            data.playerHealth = hp.CurrentHealth;
        }

        // SALVAR POSIÇÃO DO PLAYER
        if (player != null)
        {
            data.playerPosition = player.position;
        }

        // SALVAR ROTAÇÃO DA CÂMERA
        if (cameraTransform != null)
        {
            data.cameraRotation = cameraTransform.rotation;
        }

        // SALVAR INVENTÁRIO
        if (inventorySaver != null)
        {
            data.inventory = inventorySaver.SaveInventory();
        }

        // TEMPO DE JOGO
        data.playTimeSeconds = Mathf.FloorToInt(playtimeCounter);

        // DATA E HORA DO SAVE
        string now = System.DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        data.saveDate = now;
        data.lastSaveDate = now;

        // SERIALIZAR PARA JSON
        string json = JsonUtility.ToJson(data, true);

        // SALVAR EM ARQUIVO
        SaveSystem.Save(slotIndex, json);

        //Debug.Log($"💾 Slot {slotIndex} salvo!");
    }

    // ======================================================
    // CARREGAR SLOT
    // ======================================================
    public void LoadFromSlot(int slotIndex)
    {
        string json = SaveSystem.Load(slotIndex);

        if (string.IsNullOrEmpty(json))
        {
            //Debug.LogWarning($"⚠️ Nenhum save encontrado no slot {slotIndex}");
            return;
        }

        SaveData data = JsonUtility.FromJson<SaveData>(json);

        if (data == null)
        {
           // Debug.LogError("SaveManager: Erro ao decodificar SaveData!");
            return;
        }

        // RESTAURAR VIDA DO PLAYER
        HealthPlayer hp = player.GetComponent<HealthPlayer>();
        if (hp != null)
        {
            hp.SetHealth(data.playerHealth);
        }
        // ==========================  
        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false; // 🔥 DESATIVAR PARA PERMITIR TELEPORTE
        // ==========================  

        // RESTAURAR POSIÇÃO DO PLAYER
        if (player != null)
        {
            player.position = data.playerPosition;
        }

        // ==========================  
        if (cc != null) cc.enabled = true;  // 🔥 REATIVAR APÓS TELEPORTE
        // ==========================  

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

        //Debug.Log($"📂 Slot {slotIndex} carregado!");
    }

    // ======================================================
    // PEGAR METADADOS (para UI)
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
        //Debug.Log($"🗑️ Slot {slotIndex} deletado!");
    }

    // ======================================================
    // VERIFICAR SE EXISTE SAVE
    // ======================================================
    public bool SlotExists(int slotIndex)
    {
        return SaveSystem.Exists(slotIndex);
    }
}
