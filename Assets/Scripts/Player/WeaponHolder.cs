using UnityEngine;

// Mostra o modelo 3D da arma equipada na mão do player.
public class WeaponHolder : MonoBehaviour
{
    [Header("Onde a arma aparece (osso/empty da mão)")]
    public Transform handSocket;

    private GameObject currentWeapon;
    private Objects currentItem;
    private bool weaponVisible = false; // começa guardada (fora de combate)

    public bool HasWeapon => currentItem != null;

    // item == null => remove a arma da mão
    public void EquipWeapon(Objects item)
    {
        if (item == currentItem) return; // já é a mesma arma

        if (currentWeapon != null) Destroy(currentWeapon);
        currentWeapon = null;
        currentItem = null;

        if (item == null || item.worldModel == null || handSocket == null) return;

        currentWeapon = Instantiate(item.worldModel, handSocket);
        currentWeapon.transform.localPosition = Vector3.zero;
        currentWeapon.transform.localRotation = Quaternion.identity;
        currentItem = item;

        // respeita o estado atual: se está fora de combate, nasce escondida
        currentWeapon.SetActive(weaponVisible);
    }

    // chamado por Animation Event no meio da anim Draw (mão pega a espada)
    public void ShowWeapon()
    {
        weaponVisible = true;
        if (currentWeapon != null) currentWeapon.SetActive(true);
    }

    // chamado por Animation Event no meio da anim Sheath (guarda a espada)
    public void HideWeapon()
    {
        weaponVisible = false;
        if (currentWeapon != null) currentWeapon.SetActive(false);
    }
}
