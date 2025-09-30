using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [Header("Diálogo do NPC")]
    public string npcName;
    [TextArea(2, 5)]
    public List<string> lines;

    [Header("UI de interação")]
    public GameObject interactionIcon; // Ex: "Pressione E"

    private bool playerInRange = false;
    private UiManager uiManager;

    void Start()
    {
        uiManager = FindAnyObjectByType<UiManager>();
        interactionIcon.SetActive(false);
    }

    void Update()
    {
        if (playerInRange && InputManager.Instance.Interact)
        {
            Debug.Log("Dialogo Iniciado");
            InputManager.Instance.SwitchToUI();
            uiManager.StartDialogue(npcName, lines);
        }

        if (uiManager.IsDialogueActive() && InputManager.Instance.AdvanceDialogue)
        {
            uiManager.DisplayNextSentence();
            Debug.Log("Dialogo Terminado");
            
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (interactionIcon != null) interactionIcon.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (interactionIcon != null) interactionIcon.SetActive(false);
            uiManager.EndDialogue();
        }
    }

    // Gizmo para debug
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 0, 0.3f);
        Collider col = GetComponent<Collider>();
        if (col is SphereCollider sphere)
            Gizmos.DrawSphere(transform.position, sphere.radius);
        else if (col is BoxCollider box)
            Gizmos.DrawCube(transform.position, box.bounds.size);
    }
}
