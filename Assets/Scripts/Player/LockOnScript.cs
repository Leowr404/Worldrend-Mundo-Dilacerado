using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class LockOnScript : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public CinemachineCamera vcamExplore;
    public CinemachineCamera vcamLockOn;
    public CinemachineTargetGroup targetGroup;

    [Header("Input")]
    public InputActionReference lockAction;
    public InputActionReference switchAction;

    [Header("Config")]
    public float searchRadius = 20f;
    public LayerMask enemyMask;

    private Transform currentEnemy;
    private List<Transform> nearbyEnemies = new List<Transform>();

    void Start()
    {
        if (targetGroup == null || player == null)
        {
            Debug.LogWarning("⚠️ LockOnScript: targetGroup ou player não atribuídos no Inspector!");
            return;
        }

        targetGroup.Targets.Clear();
        targetGroup.AddMember(player, 1f, 0.8f);
    }

    void Update()
    {
        // Impede erro se referências faltarem
        if (player == null || targetGroup == null || vcamExplore == null || vcamLockOn == null)
            return;

        if (lockAction == null || switchAction == null)
            return;

        // Atualiza a lista de inimigos a cada frame
        UpdateNearbyEnemies();

        // Se não houver inimigos próximos, cancela lock-on
        if (currentEnemy != null && (nearbyEnemies.Count == 0 || !nearbyEnemies.Contains(currentEnemy)))
        {
            ClearLock();
        }

        if (lockAction.action.WasPressedThisFrame())
        {
            if (currentEnemy == null) TryLockNearest();
            else ClearLock();
        }

        if (currentEnemy != null && switchAction.action.WasPressedThisFrame())
        {
            SwitchTarget();
        }
    }

    void UpdateNearbyEnemies()
    {
        if (player == null) return;

        nearbyEnemies = Physics.OverlapSphere(player.position, searchRadius, enemyMask)
                               .Select(c => c.transform).ToList();
    }

    void TryLockNearest()
    {
        if (nearbyEnemies.Count == 0 || player == null) return;

        Transform nearest = nearbyEnemies
            .OrderBy(e => Vector3.Distance(player.position, e.position))
            .FirstOrDefault();

        if (nearest != null) SetLock(nearest);
    }

    void SetLock(Transform enemy)
    {
        if (enemy == null || targetGroup == null || vcamExplore == null || vcamLockOn == null)
            return;

        currentEnemy = enemy;

        targetGroup.Targets.Clear();
        targetGroup.AddMember(player, 1f, 0.8f);
        targetGroup.AddMember(enemy, 1f, 0.8f);

        vcamLockOn.Priority = vcamExplore.Priority + 1;
    }

    void ClearLock()
    {
        if (targetGroup == null || vcamExplore == null || vcamLockOn == null)
            return;

        currentEnemy = null;

        targetGroup.Targets.Clear();
        targetGroup.AddMember(player, 1f, 0.8f);

        vcamLockOn.Priority = vcamExplore.Priority - 1;
    }

    void SwitchTarget()
    {
        if (nearbyEnemies.Count <= 1 || player == null) return;

        Transform nearestOther = nearbyEnemies
            .Where(e => e != currentEnemy)
            .OrderBy(e => Vector3.Distance(player.position, e.position))
            .FirstOrDefault();

        if (nearestOther != null) SetLock(nearestOther);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, searchRadius);
    }
}
