using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    [Header("Configuração do item")]
    public string itemID;             // deve bater com objective.targetID na Quest
    public string itemName = "Item";
    public int amount = 1;
    public bool destroyOnCollect = true;

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.collider.CompareTag("Player")) return;

        // Reporta progresso pelo QuestManager
        QuestManager.Instance.ReportCollect(itemID, amount);

        UiManager.Notify(itemName + " coletado!");

        if (destroyOnCollect) Destroy(gameObject);
    }
}