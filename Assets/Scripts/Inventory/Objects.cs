using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Game/Item", order = 0)]
public class Objects : ScriptableObject
{
    public string itemName;
    public int itemId;
    public Sprite itemSprite;    // <-- CAMPO QUE FALTAVA
    public bool isStackable = true;

    public string descricaoItem;
    // Pode adicionar mais atributos depois (raridade, descrição, etc.)
}
