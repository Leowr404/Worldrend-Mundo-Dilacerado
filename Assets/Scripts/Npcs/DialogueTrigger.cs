using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public DialogueData dialogue;
    public GameObject interactionIcon;

    private UiManager uiManager;
    private bool playerInRange;

    void Start()
    {
        uiManager = FindAnyObjectByType<UiManager>();
        if (interactionIcon) interactionIcon.SetActive(false);
    }

    void Update()
    {
        if (playerInRange && InputManager.Instance.Interact && !uiManager.IsDialogueActive())
        {
            InputManager.Instance.SwitchToUI();
            Cursor.lockState = CursorLockMode.None;
            uiManager.StartDialogue(dialogue);
        }

        if (uiManager.IsDialogueActive() && InputManager.Instance.AdvanceDialogue)
        {
            uiManager.DisplayNextSentence();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (interactionIcon) interactionIcon.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (interactionIcon) interactionIcon.SetActive(false);
            uiManager.EndDialogue();
        }
    }
}
