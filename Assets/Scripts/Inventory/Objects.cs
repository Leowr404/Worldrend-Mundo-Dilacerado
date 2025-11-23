using UnityEngine;

[CreateAssetMenu(fileName = "New Object", menuName = "Inventory Object/Create New")]
public class Objects : ScriptableObject
{
    public string itemName;
    [TextArea] public string descricaoItem;
    public Sprite itemSprite;
    public bool isStackable;
}
