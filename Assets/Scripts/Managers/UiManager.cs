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

    private DialogueData currentDialogue;
    private Quest pendingQuest;

    private void Awake()
    {
        sentences = new Queue<string>();
        dialoguePanel.SetActive(false);
    }

    public void StartDialogue(DialogueData data)
    {
        if (data == null)
        {
            Debug.LogError("❌ DialogueData não atribuído no NPC!");
            return;
        }

        currentDialogue = data;
        pendingQuest = data.quest;

        dialoguePanel.SetActive(true);
        speakerNameText.text = data.npcName;

        dialoguePanel.transform.localScale = Vector3.zero;
        dialoguePanel.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);

        sentences.Clear();

        // 🔍 Lógica de qual fala usar
        if (data.quest != null)
        {
            bool hasQuest = QuestManager.Instance.HasQuest(data.quest);
            bool isCompleted = data.quest.isCompleted;

            if (!hasQuest)
                EnqueueLines(data.beforeQuest);  // antes de aceitar
            else if (hasQuest && !isCompleted)
                EnqueueLines(data.duringQuest); // em progresso
            else
                EnqueueLines(data.afterQuest);  // depois de completa
        }
        else
        {
            EnqueueLines(data.lines);
        }

        DisplayNextSentence();
    }

    private void EnqueueLines(List<string> lines)
    {
        sentences.Clear();
        foreach (string line in lines)
            sentences.Enqueue(line);
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            // Só mostra opções (ex: aceitar quest) se ainda não foi aceita
            if (currentDialogue != null && currentDialogue.options != null &&
                currentDialogue.options.Count > 0 &&
                (currentDialogue.quest == null || !QuestManager.Instance.HasQuest(currentDialogue.quest)))
            {
                ShowOptions(currentDialogue.options);
                return;
            }

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

    private void ShowOptions(List<DialogueOption> options)
    {
        foreach (Transform c in optionsContainer)
            Destroy(c.gameObject);

        foreach (DialogueOption opt in options)
        {
            GameObject btnObj = Instantiate(optionButtonPrefab, optionsContainer);
            TMP_Text btnText = btnObj.GetComponentInChildren<TMP_Text>();
            btnText.text = opt.optionText;

            btnObj.transform.localScale = Vector3.zero;
            btnObj.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);

            Button btn = btnObj.GetComponent<Button>();
            btn.onClick.AddListener(() =>
            {
                // Mostra respostas
                EnqueueLines(opt.response);

                // Se aceitar a quest, adiciona e marca como ativa
                if (opt.acceptQuest && pendingQuest != null)
                {
                    QuestManager.Instance.AddQuest(pendingQuest);
                }

                // Remove opções e continua
                foreach (Transform c in optionsContainer)
                    Destroy(c.gameObject);

                DisplayNextSentence();
            });
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

                InputManager.Instance.SwitchToPlayer();
            });
    }

    public bool IsDialogueActive() => dialoguePanel.activeSelf;
}
