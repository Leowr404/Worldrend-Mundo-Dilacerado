using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class TooltipUI : MonoBehaviour
{
    public static TooltipUI Instance;

    [Header("Referências")]
    public GameObject tooltipObject;
    public RectTransform tooltipRect;
    public TMP_Text titleText;
    public TMP_Text descriptionText;

    [Header("Offset (px)")]
    public Vector2 offset = new Vector2(30f, 0f); // Tooltip 30px à direita do mouse

    private void Awake()
    {
        Instance = this;

        if (tooltipObject != null)
            tooltipObject.SetActive(false);
    }

    private void Update()
    {
        if (tooltipObject != null && tooltipObject.activeSelf)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            tooltipRect.position = mousePos + offset;
        }
    }

    public void ShowTooltip(string title, string description)
    {
        if (titleText != null) titleText.text = title;
        if (descriptionText != null) descriptionText.text = description;

        tooltipObject.SetActive(true);
    }

    public void HideTooltip()
    {
        if (tooltipObject != null)
            tooltipObject.SetActive(false);
    }
}
