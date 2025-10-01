using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject dialoguePanel;
    public TMP_Text speakerNameText;
    public TMP_Text dialogueText;

    [Header("Typing Effect")]
    public float charInterval = 0.03f;   // tempo entre letras
    public float jumpPower = 90f;        // altura do pulo
    public float jumpDuration = 0.15f;   // duração do pulo

    private Queue<string> sentences;
    private Sequence typingSequence;

    private void Awake()
    {
        sentences = new Queue<string>();
        dialoguePanel.SetActive(false);
    }

    public void StartDialogue(string speaker, List<string> lines)
    {
        dialoguePanel.SetActive(true);
        speakerNameText.text = speaker;

        // animação de abertura
        dialoguePanel.transform.localScale = Vector3.zero;
        dialoguePanel.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);

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

        // reset
        dialogueText.text = sentence;
        dialogueText.maxVisibleCharacters = 0;

        if (typingSequence != null) typingSequence.Kill();

        typingSequence = DOTween.Sequence();

        for (int i = 0; i < sentence.Length; i++)
        {
            int index = i;

            typingSequence.AppendCallback(() =>
            {
                // mostra a letra nova
                dialogueText.maxVisibleCharacters = index + 1;

                // força TMP a atualizar o mesh imediatamente
                dialogueText.ForceMeshUpdate();

                // recria o animator sempre com o mesh atualizado
                var animator = new DOTweenTMPAnimator(dialogueText);

                if (index < animator.textInfo.characterCount)
                {
                    animator
                        .DOOffsetChar(index, new Vector3(0, jumpPower, 0), jumpDuration)
                        .SetEase(Ease.OutQuad)
                        .SetLoops(2, LoopType.Yoyo);
                }
            });

            typingSequence.AppendInterval(charInterval);
        }
    }

    public void EndDialogue()
    {
        if (typingSequence != null) typingSequence.Kill();

        dialoguePanel.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                dialoguePanel.SetActive(false);
                speakerNameText.text = "";
                dialogueText.text = "";

                // volta para Player map
                InputManager.Instance.SwitchToPlayer();
            });
    }

    public bool IsDialogueActive()
    {
        return dialoguePanel.activeSelf;
    }
}
