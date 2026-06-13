using UnityEngine;

// Soma a defesa/ataque de todas as peças equipadas e aplica no PlayerStats.
public class EquipmentManager : MonoBehaviour
{
    public static EquipmentManager Instance;

    [Header("Slots de equipamento (capacete/peito/luva/bota/arma)")]
    public EquipmentSlot[] equipmentSlots;

    [Header("Referências")]
    public PlayerStats player;
    public WeaponHolder weaponHolder; // troca o modelo 3D da arma na mão
    public InventorySlot[] inventorySlots; // pra devolver peça desequipada

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (player == null) player = FindAnyObjectByType<PlayerStats>();
        RecalculateBonus();
    }

    // Recalcula o bônus total e aplica no player
    public void RecalculateBonus()
    {
        int totalDef = 0;
        int totalAtk = 0;

        foreach (var slot in equipmentSlots)
        {
            if (slot != null && slot.currentItem != null)
            {
                totalDef += slot.rolledDefense;
                totalAtk += slot.rolledAttack;
            }
        }

        if (player != null)
            player.ApplyEquipmentBonus(totalDef, totalAtk);

        UpdateWeapon();
    }

    // Acha o slot de arma e atualiza o modelo 3D na mão
    private void UpdateWeapon()
    {
        if (weaponHolder == null) return;

        foreach (var slot in equipmentSlots)
        {
            if (slot != null && slot.slotType == EquipSlotType.Weapon)
            {
                weaponHolder.EquipWeapon(slot.currentItem); // null = remove
                return;
            }
        }

        weaponHolder.EquipWeapon(null);
    }

    // Coloca a peça desequipada no primeiro slot livre do inventário
    public bool ReturnToInventory(Objects item, int def, int atk)
    {
        foreach (var slot in inventorySlots)
        {
            if (slot.currentItem == null)
            {
                slot.SetItem(item, 1, false, def, atk);
                return true;
            }
        }

        UiManager.Notify("Inventário cheio!");
        return false;
    }
}
