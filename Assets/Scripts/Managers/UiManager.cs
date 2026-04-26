using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    public static UiManager Instance { get; private set; }

    [Header("Diálogo")]
    public GameObject dialoguePanel;
    public TMP_Text speakerNameText;
    public TMP_Text dialogueText;
    public Transform optionsContainer;
    public GameObject optionButtonPrefab;

    [Header("Quest HUD")]
    public GameObject questHUD;
    public TMP_Text questHUDName;
    public TMP_Text questHUDProgress;

    [Header("Journal")]
    public GameObject journalPanel;
    public Transform journalActiveContainer;
    public Transform journalCompletedContainer;
    public GameObject journalEntryPrefab;

    [Header("Notificações")]
    public GameObject notificationPrefab;
    public Transform notificationParent;
    public float notificationShowTime = 2f;
    public float notificationMoveUp = 30f;
    public float notificationFade = 0.5f;

    [Header("Inventário / Stats")]
    public GameObject playerStatsPanel;
    public GameObject backgroundPlayer;
    public GameObject playerInv;
    public GameObject playerStats;

    [Header("Typing Effect")]
    public float charInterval = 0.03f;
    public float jumpPower = 90f;
    public float jumpDuration = 0.15f;

    private Queue<string> sentences = new Queue<string>();
    private Sequence typingSequence;
    private bool isTyping;
    private bool playerStatsTab;

    private DialogueData currentDialogue;
    private string currentNPCID;
    private Quest pendingQuest;

    private List<RectTransform> activeNotifications = new List<RectTransform>();

    private System.Action<Quest> _onQuestAdded;
    private System.Action<Quest> _onObjectiveCompleted;
    private System.Action<Quest> _onQuestCompleted;

    // ─────────────────────────────────────────────────────
    //  LIFECYCLE
    // ─────────────────────────────────────────────────────

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        dialoguePanel.SetActive(false);
        playerStatsPanel.SetActive(false);
        backgroundPlayer.SetActive(false);
        playerInv.SetActive(false);
        playerStats.SetActive(false);
        questHUD.SetActive(false);
        journalPanel.SetActive(false);
    }

    private void OnEnable()
    {
        _onQuestAdded = OnQuestAdded;
        _onObjectiveCompleted = OnObjectiveCompleted;
        _onQuestCompleted = OnQuestCompleted;

        QuestManager.Instance.OnQuestAdded += _onQuestAdded;
        QuestManager.Instance.OnObjectiveCompleted += _onObjectiveCompleted;
        QuestManager.Instance.OnQuestCompleted += _onQuestCompleted;
    }

    private void OnDisable()
    {
        QuestManager.Instance.OnQuestAdded -= _onQuestAdded;
        QuestManager.Instance.OnObjectiveCompleted -= _onObjectiveCompleted;
        QuestManager.Instance.OnQuestCompleted -= _onQuestCompleted;
    }

    private void Update()
    {
        if (InputManager.Instance.OpenInventory && !playerStatsTab)
            ToggleInventory(true);
        if (InputManager.Instance.CloseInventory && playerStatsTab)
            ToggleInventory(false);
    }

    // ─────────────────────────────────────────────────────
    //  EVENTOS DO QUESTMANAGER
    // ─────────────────────────────────────────────────────

    private void OnQuestAdded(Quest quest)
    {
        RefreshQuestHUD();
        RefreshJournal();
    }

    private void OnObjectiveCompleted(Quest quest)
    {
        RefreshQuestHUD();
    }

    private void OnQuestCompleted(Quest quest)
    {
        RefreshQuestHUD();
        RefreshJournal();
    }

    // ─────────────────────────────────────────────────────
    //  NOTIFICAÇÕES
    // ─────────────────────────────────────────────────────

    public static void Notify(string msg) => Instance.ShowNotification(msg);

    public void ShowNotification(string msg)
    {
        GameObject notif = Instantiate(notificationPrefab, notificationParent);
        RectTransform rect = notif.GetComponent<RectTransform>();
        TMP_Text text = notif.GetComponent<TMP_Text>();

        text.text = msg;
        text.alpha = 0f;
        activeNotifications.Add(rect);

        // Remove o loop de mover para cima — o VerticalLayoutGroup cuida disso
        Sequence seq = DOTween.Sequence();
        seq.Append(text.DOFade(1f, notificationFade));
        seq.AppendInterval(notificationShowTime);
        seq.Append(text.DOFade(0f, notificationFade));
        seq.AppendCallback(() =>
        {
            activeNotifications.Remove(rect);
            Destroy(notif);
        });
    }

    // ─────────────────────────────────────────────────────
    //  QUEST HUD
    // ─────────────────────────────────────────────────────

    private void RefreshQuestHUD()
    {
        var quests = QuestManager.Instance.activeQuests;
        if (quests.Count == 0) { questHUD.SetActive(false); return; }

        var q = quests[0];
        questHUD.SetActive(true);
        questHUDName.text = q.questName;

        var obj = q.objective;
        questHUDProgress.text = q.isReadyToDeliver
            ? "Entregue ao NPC!"
            : $"{obj.description} ({obj.currentAmount}/{obj.requiredAmount})";
    }

    // ─────────────────────────────────────────────────────
    //  JOURNAL
    // ─────────────────────────────────────────────────────

    public void ToggleJournal(bool open)
    {
        journalPanel.SetActive(open);
        if (open) RefreshJournal();
    }

    private void RefreshJournal()
    {
        foreach (Transform t in journalActiveContainer) Destroy(t.gameObject);
        foreach (Transform t in journalCompletedContainer) Destroy(t.gameObject);

        foreach (var q in QuestManager.Instance.activeQuests)
            SpawnJournalEntry(q, journalActiveContainer, false);

        foreach (var q in QuestManager.Instance.completedQuests)
            SpawnJournalEntry(q, journalCompletedContainer, true);
    }

    private void SpawnJournalEntry(Quest q, Transform container, bool completed)
    {
        var entry = Instantiate(journalEntryPrefab, container);
        var label = entry.GetComponentInChildren<TMP_Text>();
        var obj = q.objective;
        string status = completed ? "✓" : $"{obj.currentAmount}/{obj.requiredAmount}";
        label.text = $"{q.questName}\n<size=80%>{obj.description} {status}</size>";
    }

    // ─────────────────────────────────────────────────────
    //  INVENTÁRIO / STATS
    // ─────────────────────────────────────────────────────

    private void ToggleInventory(bool open)
    {
        playerStatsTab = open;
        playerStatsPanel.SetActive(open);
        backgroundPlayer.SetActive(open);
        playerInv.SetActive(open);
        playerStats.SetActive(false);
        if (open) InputManager.Instance.SwitchToUI();
        else InputManager.Instance.SwitchToPlayer();
    }

    public void OpenStats() { playerStats.SetActive(true); playerInv.SetActive(false); }
    public void CloseStats() { playerStats.SetActive(false); playerInv.SetActive(true); }

    // ─────────────────────────────────────────────────────
    //  DIÁLOGO
    // ─────────────────────────────────────────────────────

    public void StartDialogue(DialogueData data, string npcID = "")
    {
        if (data == null) return;

        currentDialogue = data;
        currentNPCID = npcID;
        pendingQuest = data.quest;
        sentences.Clear();

        if (data.quest != null)
        {
            bool has = QuestManager.Instance.HasQuest(data.quest);
            bool completed = QuestManager.Instance.IsCompleted(data.quest);

            if (completed) EnqueueLines(data.afterQuest);  // já entregou
            else if (!has) EnqueueLines(data.beforeQuest); // nunca aceitou
            else EnqueueLines(data.duringQuest); // em andamento
        }
        else
        {
            EnqueueLines(data.lines);
        }

        if (sentences.Count == 0) EnqueueLines(data.lines);

        dialoguePanel.SetActive(true);
        speakerNameText.text = data.npcName;
        dialoguePanel.transform.localScale = Vector3.zero;
        dialoguePanel.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);

        DisplayNextSentence();
    }

    private void EnqueueLines(List<DialogueLine> lines)
    {
        if (lines == null) return;
        foreach (var l in lines)
            if (!string.IsNullOrEmpty(l.text)) sentences.Enqueue(l.text);
    }

    public void DisplayNextSentence()
    {
        if (isTyping) { SkipTyping(); return; }

        if (sentences.Count == 0)
        {
            bool temQuest = currentDialogue?.quest != null;
            bool naoAceitou = temQuest
                           && !QuestManager.Instance.HasQuest(currentDialogue.quest)
                           && !QuestManager.Instance.IsCompleted(currentDialogue.quest);

            if (naoAceitou) { ShowQuestButtons(); return; }

            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        dialogueText.text = sentence;
        dialogueText.maxVisibleCharacters = 0;

        if (typingSequence != null) typingSequence.Kill();
        typingSequence = DOTween.Sequence();
        isTyping = true;

        for (int i = 0; i < sentence.Length; i++)
        {
            int index = i;
            typingSequence.AppendCallback(() =>
            {
                dialogueText.maxVisibleCharacters = index + 1;
                dialogueText.ForceMeshUpdate();
                var anim = new DOTweenTMPAnimator(dialogueText);
                if (index < anim.textInfo.characterCount)
                    anim.DOOffsetChar(index, new Vector3(0, jumpPower, 0), jumpDuration)
                        .SetEase(Ease.OutQuad).SetLoops(2, LoopType.Yoyo);
            });
            typingSequence.AppendInterval(charInterval);
        }
        typingSequence.OnComplete(() => isTyping = false);
    }

    private void SkipTyping()
    {
        typingSequence?.Kill();
        dialogueText.maxVisibleCharacters = dialogueText.text.Length;
        isTyping = false;
    }

    private void ShowQuestButtons()
    {
        foreach (Transform c in optionsContainer) Destroy(c.gameObject);

        SpawnButton("Aceitar quest", () =>
        {
            QuestManager.Instance.AddQuest(pendingQuest);
            foreach (Transform c in optionsContainer) Destroy(c.gameObject);
            DisplayNextSentence();
        });

        SpawnButton("Recusar", () =>
        {
            foreach (Transform c in optionsContainer) Destroy(c.gameObject);
            EndDialogue();
        });
    }

    private void SpawnButton(string label, UnityEngine.Events.UnityAction callback)
    {
        var btn = Instantiate(optionButtonPrefab, optionsContainer);
        btn.GetComponentInChildren<TMP_Text>().text = label;
        btn.transform.localScale = Vector3.zero;
        btn.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
        btn.GetComponent<Button>().onClick.AddListener(callback);
    }

    public void EndDialogue()
    {
        if (typingSequence != null) typingSequence.Kill();
        isTyping = false;

        dialoguePanel.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                dialoguePanel.SetActive(false);
                speakerNameText.text = "";
                dialogueText.text = "";
                currentDialogue = null;
                currentNPCID = "";
                pendingQuest = null;
                Cursor.lockState = CursorLockMode.Locked;
                InputManager.Instance.SwitchToPlayer();
            });
    }

    public bool IsActiveDialogue(DialogueData data) => dialoguePanel.activeSelf && currentDialogue == data;
    public bool IsDialogueActive() => dialoguePanel.activeSelf;
}