using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private DialogueData dialogue;
    [SerializeField] private string npcID;
    [SerializeField] private GameObject interactionIcon;

    private bool playerInRange;

    private void Start()
    {
        if (interactionIcon) interactionIcon.SetActive(false);
    }

    private void Update()
    {
        if (!playerInRange) return;

        if (InputManager.Instance.Interact && !UiManager.Instance.IsDialogueActive())
        {
            AudioManager.instancia.PlayNPCTalk();
            InputManager.Instance.SwitchToUI();
            Cursor.lockState = CursorLockMode.None;

            // Entrega primeiro — assim o diálogo já vê a quest como completa
            QuestManager.Instance.TryDeliverQuest(npcID);

            // Só reporta TalkToNPC se não tem quest para entregar aqui
            if (!QuestManager.Instance.HasQuestToDeliver(npcID))
                QuestManager.Instance.ReportTalkToNPC(npcID);

            UiManager.Instance.StartDialogue(dialogue, npcID);
        }

        if (UiManager.Instance.IsActiveDialogue(dialogue) && InputManager.Instance.AdvanceDialogue)
        {
            Cursor.lockState = CursorLockMode.None;
            UiManager.Instance.DisplayNextSentence();
            AudioManager.instancia.PlayNPCTalk();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = true;
        if (interactionIcon) interactionIcon.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = false;
        if (interactionIcon) interactionIcon.SetActive(false);
        if (UiManager.Instance.IsActiveDialogue(dialogue))
            UiManager.Instance.EndDialogue();
    }
}