using UnityEngine;

// Parede invisível que bloqueia o player até uma quest ser completada.
// Liga/desliga o Collider (nunca o GameObject) pra sobreviver a save/load.
[RequireComponent(typeof(Collider))]
public class QuestBarrier : MonoBehaviour
{
    [Header("Quest que libera a passagem")]
    public Quest requiredQuest;

    private Collider barrierCollider;

    private void Awake()
    {
        barrierCollider = GetComponent<Collider>();
    }

    private void Start()
    {
        Refresh();
        if (QuestManager.Instance != null)
            QuestManager.Instance.OnQuestCompleted += OnQuestCompleted;
    }

    private void OnDestroy()
    {
        if (QuestManager.Instance != null)
            QuestManager.Instance.OnQuestCompleted -= OnQuestCompleted;
    }

    private void OnQuestCompleted(Quest quest)
    {
        if (quest == requiredQuest) Refresh();
    }

    // Lê o estado real da quest e liga/desliga o bloqueio.
    // Chamado no Start, ao completar a quest, e pelo SaveManager após carregar.
    public void Refresh()
    {
        if (requiredQuest == null || QuestManager.Instance == null) return;

        bool done = QuestManager.Instance.IsCompleted(requiredQuest);
        if (barrierCollider != null) barrierCollider.enabled = !done;
    }
}
