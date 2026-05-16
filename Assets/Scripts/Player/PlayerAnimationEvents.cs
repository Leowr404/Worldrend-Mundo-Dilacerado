using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    void EnableHitbox()
    {
        var hbm = HitboxManager.Instance;
        if (hbm == null) { Debug.LogWarning("HitboxManager.Instance null!"); return; }

        PlayerStats stats = GetComponentInParent<PlayerStats>();
        hbm.ActivateHitbox(hbm.lightHitbox, stats != null ? stats.attackPower : 10, HitboxManager.AttackType.Light);
    }

    void DisableHitbox() => HitboxManager.Instance?.DeactivateAll();
}
