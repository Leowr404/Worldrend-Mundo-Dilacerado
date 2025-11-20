using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    [Header("Configuração do item")]
    public string questName;  // Nome exato da quest no ScriptableObject
    public int amount = 1;    // Quanto esse item adiciona no progresso
    public bool destroyOnCollect = true; // se o item some ao coletar
    [Header("Configuração do item")]
    public string itemName = "Item Sem Nome";

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.collider.CompareTag("Player")) return;

        // Procura a quest correspondente
        Quest quest = QuestManager.Instance.activeQuests.Find(q => q.questName == questName);

        if (quest != null && !quest.isCompleted)
        {
            quest.AddProgress(amount);
            Debug.Log($"🧺 Coletado +{amount} para a quest: {quest.questName} ({quest.collectedItems}/{quest.requiredItems})");

            // Verifica se completou
            if (quest.isCompleted)
            {
                Debug.Log($"✅ Quest '{quest.questName}' completada!");
                QuestManager.Instance.CompleteQuest(quest);
            }

            // some com o item se configurado
            if (destroyOnCollect) Destroy(gameObject);
            {
             NotificationUI.Show(itemName + " coletado!");
            }
        }
    }
}
