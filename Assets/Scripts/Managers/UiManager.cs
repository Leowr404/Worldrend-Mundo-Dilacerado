using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject dialoguePanel;   // Painel da UI
    public TMP_Text speakerNameText;   // Nome do personagem
    public TMP_Text dialogueText;      // Texto da fala

    [Header("Typing Effect")]
    public float typingSpeed = 0.5f;

    private Queue<string> sentences;
    private Coroutine typingCoroutine;

    private void Awake()
    {
        sentences = new Queue<string>();
        dialoguePanel.SetActive(false);
    }

    public void StartDialogue(string speaker, List<string> lines)
    {
        dialoguePanel.SetActive(true);
        speakerNameText.text = speaker;

        sentences.Clear();
        foreach (string line in lines)
            sentences.Enqueue(line);

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeSentence(sentence));
    }

    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    public void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        speakerNameText.text = "";
        dialogueText.text = "";
        InputManager.Instance.SwitchToPlayer();
        Debug.Log("Fim Do Dialogo");
    }

    public bool IsDialogueActive()
    {
        return dialoguePanel.activeSelf;
    }
}
