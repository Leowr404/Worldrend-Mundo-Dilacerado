using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class SaveSlotUI : MonoBehaviour, IPointerClickHandler
{
    [Header("Index")]
    public int slotIndex;  // definido no Inspector (0,1,2)

    [Header("UI")]
    public TMP_Text slotText;
    public TMP_Text dateText;
    public TMP_Text playtimeText;

    // ---------------------------------------------------------
    // Atualizar UI do slot
    // ---------------------------------------------------------
    public void Refresh()
    {
        // Nome do slot sempre aparece
        slotText.text = $"Slot {slotIndex + 1}";

        // Existe save nesse slot?
        bool exists = SaveManager.Instance.SlotExists(slotIndex);

        if (!exists)
        {
            dateText.text = "<color=grey>Vazio</color>";
            playtimeText.text = "";
            return;
        }

        // Carregar apenas metadados
        SaveData data = SaveManager.Instance.Peek(slotIndex);

        if (data == null)
        {
            dateText.text = "<color=grey>Vazio</color>";
            playtimeText.text = "";
            return;
        }

        // Exibe data do save
        dateText.text = $"Último save: {data.lastSaveDate}";

        // Exibe tempo de jogo formatado
        playtimeText.text = $"Tempo de jogo: {FormatTime(data.playTimeSeconds)}";
    }

    // ---------------------------------------------------------
    // Formatar HH:MM:SS
    // ---------------------------------------------------------
    private string FormatTime(int seconds)
    {
        int h = seconds / 3600;
        int m = (seconds % 3600) / 60;
        int s = seconds % 60;

        return $"{h:00}:{m:00}:{s:00}";
    }

    // ---------------------------------------------------------
    // Clique no slot → informa à UI principal
    // ---------------------------------------------------------
    public void OnPointerClick(PointerEventData eventData)
    {
        SaveLoadUI.Instance.SelectSlot(slotIndex);
    }
}
