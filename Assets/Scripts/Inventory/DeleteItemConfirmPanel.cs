using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DeleteItemConfirmPanel : MonoBehaviour
{
    public static DeleteItemConfirmPanel Instance;

    [Header("UI")]
    public GameObject panel;
    public TMP_Text messageText;
    public Button yesButton;
    public Button noButton;

    private InventorySlot storedSlot; // slot que será deletado

    private void Awake()
    {
        Instance = this;

        if (panel != null)
            panel.SetActive(false);
    }

    // ============================================
    // 📌 MÉTODO QUE ESTAVA FALTANDO
    // ============================================
    public void OpenConfirm(InventorySlot slot)
    {
        storedSlot = slot;

        panel.SetActive(true);

        if (messageText != null)
            messageText.text = $"Deseja excluir <b>{slot.currentItem.itemName}</b>?";
    }

    // ============================================
    public void ConfirmDelete()
    {
        if (storedSlot != null)
        {
            storedSlot.DeleteItemConfirmed();
        }

        Close();
    }

    public void Close()
    {
        panel.SetActive(false);
        storedSlot = null;
    }
}
