using UnityEngine;

public enum ItemType
{
    Consumable,
    Weapon,
    Armor,
    Misc
}

[CreateAssetMenu(fileName = "NewItem", menuName = "Game/Item", order = 0)]
public class Objects : ScriptableObject
{
    [Header("Dados básicos")]
    public string itemName;
    public int itemId;
    public Sprite itemSprite;
    public bool isStackable = true;

    [TextArea]
    public string descricaoItem;

    [Header("Tipo do item")]
    public ItemType itemType = ItemType.Misc;

    [Header("Consumível")]
    public int healAmount = 0;

    [Header("Equipamento")]
    public int attackBonus = 0;
    public int defenseBonus = 0;
   
    [Header("Buff de Stamina")]
    public int staminaBonus = 0;
    public float staminaDuration = 0f;
}