using UnityEngine;

[CreateAssetMenu(fileName = "New Object", menuName = "Inventory Object/Create New")]
public class Objects : ScriptableObject
{
    [Header("Informações do Item")]
    public string itemName;
    [TextArea(2, 5)] public string descricaoItem;
    public Sprite itemSprite;

    [Header("Configurações")]
    public bool isStackable; // 🟩 Define se o item é empilhável
}
