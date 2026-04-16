using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.EventSystems;

public class ItemMenuUI : MonoBehaviour
{
    public static ItemMenuUI Instance;

    [Header("UI")]
    public GameObject panel;
    public Button useButton;
    public Button descriptionButton;
    public Button deleteButton;

    [Header("Offset do Menu")]
    public Vector2 menuOffset = new Vector2(150f, -20f);

    private InventorySlot currentSlot;

    private void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    private void Update()
    {
        if (!panel.activeSelf) return;

        // Fechar ao clicar fora
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();

            if (!RectTransformUtility.RectangleContainsScreenPoint(
                panel.GetComponent<RectTransform>(),
                mousePos,
                null))
            {
                Close();
            }
        }
    }

    public void OpenMenu(InventorySlot slot, Vector2 mousePos, Vector2 offset)
    {
        currentSlot = slot;

        panel.SetActive(true);

        RectTransform rect = panel.GetComponent<RectTransform>();
        rect.position = mousePos + offset;
    }

    public void Close()
    {
        panel.SetActive(false);
        currentSlot = null;
    }

    // ============================
    // BOTÕES
    // ============================

    public void OnUseItem()
    {
        Debug.Log("Usou o item: " + currentSlot.currentItem.itemName);
        Close();
    }

    public void OnShowDescription()
    {
        TooltipUI.Instance.ShowTooltip(
            currentSlot.currentItem.itemName,
            currentSlot.currentItem.descricaoItem
        );
        Close();
    }
    public void OnDeleteItem()
    {
        //  Agora usa confirmação!
        DeleteItemConfirmPanel.Instance.OpenConfirm(currentSlot);
        Close();
    }
}
