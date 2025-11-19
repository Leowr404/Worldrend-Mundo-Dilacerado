using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TooltipUI : MonoBehaviour
{
    public static TooltipUI Instance;

    [Header("Referências")]
    public GameObject tooltipObject;   // painel do tooltip
    public TMP_Text titleText;         // título do item
    public TMP_Text descriptionText;   // descrição do item

    private RectTransform rect;

    private void Awake()
    {
        Instance = this;
        rect = tooltipObject.GetComponent<RectTransform>();
    }

    private void Start()
    {
        tooltipObject.SetActive(false);
    }

    public void ShowTooltip(string itemName, string description, Vector3 nearSlotPosition)
    {
        if (tooltipObject == null)
        {
            Debug.LogError("TooltipObject não está atribuído no TooltipUI!");
            return;
        }

        // Define o conteúdo
        titleText.text = itemName;
        descriptionText.text = description;

        // Posiciona o tooltip ao lado do slot
        Vector3 offset = new Vector3(160f, 0f, 0f); // lado direito
        Vector3 finalPos = nearSlotPosition + offset;

        tooltipObject.transform.position = finalPos;

        tooltipObject.SetActive(true);
    }

    public void HideTooltip()
    {
        if (tooltipObject == null)
        {
            Debug.LogError("TooltipObject não atribuído, não foi possível esconder.");
            return;
        }

        tooltipObject.SetActive(false);
    }
}
