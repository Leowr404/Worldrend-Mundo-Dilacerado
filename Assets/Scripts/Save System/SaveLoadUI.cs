using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class SaveLoadUI : MonoBehaviour
{
    public static SaveLoadUI Instance { get; private set; }

    [Header("UI")]
    public GameObject panel;
    public SaveSlotUI[] slotUIs;   // Arraste os 3 slots
    public Button saveButton;
    public Button loadButton;
    public Button deleteButton;

    private int selectedSlot = -1;

    private void Awake()
    {
        Instance = this;
        panel.SetActive(false);
        // Liga eventos dos botões
        saveButton.onClick.AddListener(OnSavePressed);
        loadButton.onClick.AddListener(OnLoadPressed);
        deleteButton.onClick.AddListener(OnDeletePressed);
    }

    public void ClearSelection()
    {
        selectedSlot = -1;
        UpdateButtons ();
    }
    // ============================================================
    // ABRIR / FECHAR UI
    // ============================================================

    public void Open()
    {
        Debug.Log("abriu");
        panel.SetActive(true);
        RefreshAll();
        selectedSlot = -1;
        UpdateButtons();
    }

    public void Close()
    {
        panel.SetActive(false);
    }

    // ============================================================
    // SELEÇÃO DE SLOT
    // ============================================================

    public void SelectSlot(int index)
    {
        selectedSlot = index;
        UpdateButtons();
    }

    private void UpdateButtons()
    {
        bool hasSlot = selectedSlot != -1;

        saveButton.interactable = hasSlot;
        loadButton.interactable = hasSlot && SaveManager.Instance.SlotExists(selectedSlot);
        deleteButton.interactable = hasSlot && SaveManager.Instance.SlotExists(selectedSlot);
    }

    // Atualizar os 3 slots visuais
    public void RefreshAll()
    {
        foreach (var slot in slotUIs)
            slot.Refresh();
    }

    // ============================================================
    // BOTÕES
    // ============================================================

    private void OnSavePressed()
    {
        if (selectedSlot == -1) return;

        SaveManager.Instance.SaveToSlot(selectedSlot);
        RefreshAll();
        UpdateButtons();
    }

    private void OnLoadPressed()
    {
        if (selectedSlot == -1) return;

        SaveManager.Instance.LoadFromSlot(selectedSlot);
    }

    private void OnDeletePressed()
    {
        if (selectedSlot == -1) return;

        SaveManager.Instance.DeleteSlot(selectedSlot);
        RefreshAll();
        UpdateButtons();
    }
}
