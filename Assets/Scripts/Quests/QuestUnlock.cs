using UnityEngine;

// Troca objetos quando uma quest é completada.
// Ex: NPC (antes) some e a Loja (depois) aparece no lugar.
// IMPORTANTE: este script fica num objeto PAI que está sempre ativo;
// ele liga/desliga os FILHOS. Assim sobrevive a save/load.
public class QuestUnlock : MonoBehaviour
{
    [Header("Quest que desbloqueia")]
    public Quest requiredQuest;

    [Header("Objetos")]
    public GameObject beforeObject; // ativo ANTES de completar (ex: NPC)
    public GameObject afterObject;  // ativo DEPOIS de completar (ex: Loja)

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

    private bool pendingSwap;

    private void OnQuestCompleted(Quest quest)
    {
        // não troca já — espera o diálogo fechar (senão trava o painel)
        if (quest == requiredQuest) pendingSwap = true;
    }

    private void Update()
    {
        if (pendingSwap && (UiManager.Instance == null || !UiManager.Instance.IsDialogueActive()))
        {
            pendingSwap = false;
            Refresh();
        }
    }

    // Lê o estado real da quest e troca os objetos.
    // Chamado no Start, ao completar a quest, e pelo SaveManager após carregar.
    public void Refresh()
    {
        if (requiredQuest == null || QuestManager.Instance == null) return;

        bool done = QuestManager.Instance.IsCompleted(requiredQuest);
        if (beforeObject != null) beforeObject.SetActive(!done);
        if (afterObject != null) afterObject.SetActive(done);
    }
}
