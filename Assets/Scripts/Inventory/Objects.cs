using UnityEngine;

public enum ItemType { Material, Consumable, Equipment }
public enum EquipSlotType { Helmet, Chest, Glove, Boot, Weapon }

[CreateAssetMenu(fileName = "NewItem", menuName = "Game/Item", order = 0)]
public class Objects : ScriptableObject
{
    [Header("Básico")]
    public string itemName;
    public int itemId;
    public Sprite itemSprite;
    public bool isStackable = true;
    [TextArea] public string descricaoItem;

    [Header("Tipo do item")]
    public ItemType itemType = ItemType.Material;

    [Header("Consumível (cura ao usar)")]
    public int healthRestore;
    public int staminaRestore;

    [Header("Equipável (atributos rolados ao coletar)")]
    public EquipSlotType equipSlot;
    public int minDefense;
    public int maxDefense;
    public int minAttack;
    public int maxAttack;

    [Header("Modelo 3D (só arma — aparece na mão ao equipar)")]
    public GameObject worldModel;
}
