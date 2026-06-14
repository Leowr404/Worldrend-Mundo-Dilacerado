using UnityEngine;

// Mostra a arma equipada. Guardada fica no backSocket (costas),
// em combate vai pro handSocket (mão). A troca é feita por Animation Events.
public class WeaponHolder : MonoBehaviour
{
    [Header("Sockets")]
    public Transform handSocket; // mão (em combate)
    public Transform backSocket; // costas (guardada)

    private GameObject currentWeapon;
    private Objects currentItem;

    public bool HasWeapon => currentItem != null;

    // Chamado pelo EquipmentManager ao equipar/desequipar.
    public void EquipWeapon(Objects item)
    {
        if (item == currentItem) return;

        if (currentWeapon != null) Destroy(currentWeapon);
        currentWeapon = null;
        currentItem = item;

        if (item == null || item.worldModel == null) return;

        AttachTo(backSocket); // nasce guardada nas costas
    }

    // Animation Event no meio da anim Draw (mão pega a espada das costas)
    public void ShowWeapon() => AttachTo(handSocket);

    // Animation Event no meio da anim Sheath (guarda a espada nas costas)
    public void HideWeapon() => AttachTo(backSocket);

    private void AttachTo(Transform socket)
    {
        if (currentItem == null || socket == null) return;

        if (currentWeapon == null)
            currentWeapon = Instantiate(currentItem.worldModel, socket);
        else
            currentWeapon.transform.SetParent(socket);

        currentWeapon.transform.localPosition = Vector3.zero;
        currentWeapon.transform.localRotation = Quaternion.identity;
    }
}
