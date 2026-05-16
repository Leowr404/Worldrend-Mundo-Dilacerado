using UnityEngine;

public class EnemyAnimationEvents : MonoBehaviour
{
    public EnemyHitboxTrigger hitbox;

    void EnableHitbox() => hitbox?.Enable();
    void DisableHitbox() => hitbox?.Disable();
}
