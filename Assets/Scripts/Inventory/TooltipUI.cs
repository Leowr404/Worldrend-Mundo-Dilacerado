using UnityEngine;
using TMPro;
using UnityEngine.InputSystem; // <- IMPORTANTE

public class TooltipUI : MonoBehaviour
{
    public static TooltipUI Instance;

    [Header("Referências")]
    public GameObject tooltipPanel;
    public TMP_Text tooltipText;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        tooltipPanel.SetActive(false);
    }

    private void Update()
    {
        if (tooltipPanel.activeSelf)
        {
            // Mouse do novo Input System
            Vector2 pos = Mouse.current.position.ReadValue();

            Vector2 offset = new Vector2(15f, -15f);

            tooltipPanel.transform.position = pos + offset;
        }
    }

    public void Show(string text)
    {
        tooltipText.text = text;
        tooltipPanel.SetActive(true);
    }

    public void Hide()
    {
        tooltipPanel.SetActive(false);
    }
}
