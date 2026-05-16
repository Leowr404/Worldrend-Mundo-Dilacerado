using UnityEngine;

public class EnemyAnimationEvents : MonoBehaviour
{
    public HitboxTrigger hitbox;

    void EnableHitbox() => hitbox?.Enable();
    void DisableHitbox() => hitbox?.Disable();
}
