using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    private WeaponHolder weaponHolder;

    void Awake()
    {
        weaponHolder = GetComponentInParent<WeaponHolder>();
    }

    void EnableHitbox()
    {
        var hbm = HitboxManager.Instance;
        if (hbm == null) { Debug.LogWarning("HitboxManager.Instance null!"); return; }

        PlayerStats stats = GetComponentInParent<PlayerStats>();
        hbm.ActivateHitbox(hbm.lightHitbox, stats != null ? stats.attackPower : 10, HitboxManager.AttackType.Light);
    }

    void DisableHitbox() => HitboxManager.Instance?.DeactivateAll();

    // chamados por Animation Event nas anims Draw / Sheath
    void ShowWeapon() => weaponHolder?.ShowWeapon();
    void HideWeapon() => weaponHolder?.HideWeapon();
}
