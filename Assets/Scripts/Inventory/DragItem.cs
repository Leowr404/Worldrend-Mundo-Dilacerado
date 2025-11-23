using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class DragItem : MonoBehaviour
{
    public static DragItem Instance { get; private set; }

    [Header("Config")]
    public Canvas rootCanvas;
    public Vector2 offset = new Vector2(15f, -15f);

    private GameObject iconGO;
    private Image iconImage;
    private RectTransform iconRect;

    [HideInInspector] 
    public InventorySlot sourceSlot;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        iconGO = new GameObject("DragIcon");
        iconGO.transform.SetParent(rootCanvas.transform, false);

        iconImage = iconGO.AddComponent<Image>();
        iconImage.raycastTarget = false;

        iconRect = iconGO.GetComponent<RectTransform>();
        iconRect.sizeDelta = new Vector2(48, 48);

        iconGO.SetActive(false);
    }

    private void Update()
    {
        if (!iconGO.activeSelf) return;

        Vector2 mousePos = Mouse.current.position.ReadValue();
        iconRect.position = mousePos + offset;
    }

    public void BeginDrag(Sprite sprite, InventorySlot slot)
    {
        sourceSlot = slot;
        iconImage.sprite = sprite;
        iconGO.SetActive(true);
    }

    public void EndDrag()
    {
        sourceSlot = null;
        iconGO.SetActive(false);
        iconImage.sprite = null;
    }
}
