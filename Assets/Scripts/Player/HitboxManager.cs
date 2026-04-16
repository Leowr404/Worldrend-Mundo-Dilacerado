using UnityEngine;
using System.Collections.Generic;

public class HitboxManager : MonoBehaviour
{
    public static HitboxManager Instance { get; private set; }
    private PlayerStats playerStats;

    [Header("Hitboxes")]
    public HitboxTrigger lightHitbox;
    public HitboxTrigger heavyHitbox;

    [Header("Gizmos")]
    public Color lightHitboxColor = new Color(0f, 1f, 0f, 0.3f);
    public Color heavyHitboxColor = new Color(1f, 0f, 0f, 0.3f);
    public float hitboxDuration = 0.15f;

    private HashSet<GameObject> alreadyHit = new HashSet<GameObject>();
    private float timer;
    private bool isActive = false;
    private int currentDamage;

    // ── NOVO ──
    public enum AttackType { Light, Heavy }
    private AttackType currentAttackType;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DisableAll();
        playerStats = GetComponentInParent<PlayerStats>();
    }

    void Update()
    {
        // Mantém seu input direto pra testar — trocar por Animation Events depois
        if (InputManager.Instance.Attack)
            ActivateHitbox(lightHitbox, playerStats.attackPower, AttackType.Light);

        if (InputManager.Instance.Attack2)
            ActivateHitbox(heavyHitbox, playerStats.attackPower * 2, AttackType.Heavy); // ← dano maior

        if (!isActive) return;
        timer -= Time.deltaTime;
        if (timer <= 0) DeactivateAll();
    }

    // ── Sobrecarga com AttackType opcional (não quebra chamadas existentes) ──
    public void ActivateHitbox(HitboxTrigger hitbox, int damage, AttackType attackType = AttackType.Light)
    {
        DeactivateAll();
        alreadyHit.Clear();
        currentDamage = damage;
        currentAttackType = attackType; // ← salva o tipo
        hitbox.Enable();
        isActive = true;
        timer = hitboxDuration;
    }

    public void DeactivateAll()
    {
        isActive = false;
        DisableAll();
        alreadyHit.Clear();
    }

    void DisableAll()
    {
        if (lightHitbox) lightHitbox.Disable();
        if (heavyHitbox) heavyHitbox.Disable();
    }

    public void RegisterHit(GameObject target)
    {
        if (!isActive) return;
        if (alreadyHit.Contains(target)) return;

        if (target.TryGetComponent<EnemyStats>(out var enemy))
        {
            enemy.TakeDamage(currentDamage);
            alreadyHit.Add(target);

            // ── NOVO: knockback + ragdoll só no heavy ──
            if (currentAttackType == AttackType.Heavy)
            {
                if (target.TryGetComponent<EnemyHitHeavy>(out var reaction))
                    reaction.TakeHeavyHit(transform.position);
            }
        }
        else if (target.TryGetComponent<Destructible>(out var obj))
        {
            obj.TakeDamage(currentDamage);
            alreadyHit.Add(target);
        }
    }

    void OnDrawGizmos()
    {
        DrawHitboxGizmo(lightHitbox, lightHitboxColor);
        DrawHitboxGizmo(heavyHitbox, heavyHitboxColor);
    }

    void DrawHitboxGizmo(HitboxTrigger hitbox, Color color)
    {
        if (hitbox == null) return;
        BoxCollider box = hitbox.GetComponent<BoxCollider>();
        if (box == null) return;
        Matrix4x4 oldMatrix = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(
            hitbox.transform.position,
            hitbox.transform.rotation,
            hitbox.transform.lossyScale
        );
        if (box.enabled)
        {
            Gizmos.color = color;
            Gizmos.DrawCube(box.center, box.size);
        }
        Gizmos.color = new Color(color.r, color.g, color.b, 1f);
        Gizmos.DrawWireCube(box.center, box.size);
        Gizmos.matrix = oldMatrix;
    }
}