using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class DragItem : MonoBehaviour
{
    public static DragItem Instance;

    public Image dragIcon;
    public RectTransform dragRect;

    [HideInInspector] public InventorySlot sourceSlot;

    private void Awake()
    {
        Instance = this;
        dragIcon.enabled = false;
    }

    private void Update()
    {
        if (dragIcon.enabled)
        {
            Vector2 pos = Mouse.current.position.ReadValue();
            dragRect.position = pos;
        }
    }

    public void BeginDrag(Sprite icon, InventorySlot slot)
    {
        sourceSlot = slot;
        dragIcon.sprite = icon;
        dragIcon.enabled = true;
    }

    public void EndDrag()
    {
        dragIcon.enabled = false;
        sourceSlot = null;
    }
}
