using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject dialoguePanel;
    public TMP_Text speakerNameText;
    public TMP_Text dialogueText;
    public Transform optionsContainer;
    public GameObject optionButtonPrefab;

    [Header("Typing Effect")]
    public float charInterval = 0.03f;
    public float jumpPower = 90f;
    public float jumpDuration = 0.15f;

    private Queue<string> sentences;
    private Sequence typingSequence;
    private Quest pendingQuest;

    private void Awake()
    {
        sentences = new Queue<string>();
        dialoguePanel.SetActive(false);
    }

    public void StartDialogue(DialogueData data)
    {
        dialoguePanel.SetActive(true);
        speakerNameText.text = data.npcName;

        dialoguePanel.transform.localScale = Vector3.zero;
        dialoguePanel.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);

        sentences.Clear();

        // Escolhe fala de acordo com quest
        if (data.quest == null)
            foreach (string l in data.lines) sentences.Enqueue(l);
        else if (!QuestManager.Instance.HasQuest(data.quest) && !data.quest.isCompleted)
            foreach (string l in data.beforeQuest) sentences.Enqueue(l);
        else if (!data.quest.isCompleted)
            foreach (string l in data.duringQuest) sentences.Enqueue(l);
        else
            foreach (string l in data.afterQuest) sentences.Enqueue(l);

        pendingQuest = data.quest;

        // Remove botões antigos
        foreach (Transform child in optionsContainer)
            Destroy(child.gameObject);

        // Cria botões novos
        if (data.options != null && data.options.Count > 0)
        {
            foreach (DialogueOption opt in data.options)
            {
                GameObject btnObj = Instantiate(optionButtonPrefab, optionsContainer);
                TMP_Text btnText = btnObj.GetComponentInChildren<TMP_Text>();
                btnText.text = opt.optionText;

                Button btn = btnObj.GetComponent<Button>();
                btn.onClick.AddListener(() =>
                {
                    sentences.Clear();
                    foreach (string resp in opt.response)
                        sentences.Enqueue(resp);

                    if (opt.acceptQuest && pendingQuest != null)
                        QuestManager.Instance.AddQuest(pendingQuest);

                    foreach (Transform c in optionsContainer)
                        Destroy(c.gameObject);

                    DisplayNextSentence();
                });
            }
        }

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
        dialogueText.text = sentence;
        dialogueText.maxVisibleCharacters = 0;

        if (typingSequence != null) typingSequence.Kill();
        typingSequence = DOTween.Sequence();

        for (int i = 0; i < sentence.Length; i++)
        {
            int index = i;
            typingSequence.AppendCallback(() =>
            {
                dialogueText.maxVisibleCharacters = index + 1;
                dialogueText.ForceMeshUpdate();

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

                foreach (Transform child in optionsContainer)
                    Destroy(child.gameObject);

                InputManager.Instance.SwitchToPlayer();
            });
    }

    public bool IsDialogueActive()
    {
        return dialoguePanel.activeSelf;
    }
}
