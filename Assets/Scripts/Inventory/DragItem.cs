using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

/// <summary>
/// Gerencia o visual do item sendo arrastado (singleton).
/// </summary>
public class DragItem : MonoBehaviour
{
    public static DragItem Instance { get; private set; }

    [Header("Config")]
    public Canvas rootCanvas;            // arraste seu Canvas aqui no Inspector
    public Vector2 offset = new Vector2(15f, -15f);

    private GameObject iconGO;
    private Image iconImage;
    private RectTransform iconRect;
    [HideInInspector] public InventorySlot sourceSlot; // slot de origem enquanto arrasta

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        // cria o ícone de arrasto dinamicamente
        iconGO = new GameObject("DragIcon");
        iconGO.transform.SetParent(rootCanvas.transform, false);
        iconImage = iconGO.AddComponent<Image>();
        iconImage.raycastTarget = false; // não bloquear raycasts
        iconRect = iconGO.GetComponent<RectTransform>();
        iconRect.sizeDelta = new Vector2(48, 48);
        iconGO.SetActive(false);
    }

    private void Update()
    {
        if (!iconGO.activeSelf) return;

        // pega posição do mouse usando novo Input System
        Vector2 mousePos = Mouse.current.position.ReadValue();
        iconRect.position = (Vector3)mousePos + (Vector3)offset;
    }

    public void BeginDrag(Sprite sprite, InventorySlot fromSlot)
    {
        if (sprite == null || fromSlot == null) return;
        sourceSlot = fromSlot;
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
