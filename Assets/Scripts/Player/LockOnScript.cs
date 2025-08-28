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
        targetGroup.Targets.Clear();
        targetGroup.AddMember(player, 1f, 0.8f);
    }

    void Update()
    {
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
        nearbyEnemies = Physics.OverlapSphere(player.position, searchRadius, enemyMask)
                               .Select(c => c.transform).ToList();
    }

    void TryLockNearest()
    {
        if (nearbyEnemies.Count == 0) return;

        // pega o inimigo mais próximo
        Transform nearest = nearbyEnemies.OrderBy(e => Vector3.Distance(player.position, e.position)).FirstOrDefault();
        if (nearest != null) SetLock(nearest);
    }

    void SetLock(Transform enemy)
    {
        currentEnemy = enemy;

        targetGroup.Targets.Clear();
        targetGroup.AddMember(player, 1f, 0.8f);
        targetGroup.AddMember(enemy, 1f, 0.8f);

        vcamLockOn.Priority = vcamExplore.Priority + 1;
    }

    void ClearLock()
    {
        currentEnemy = null;

        targetGroup.Targets.Clear();
        targetGroup.AddMember(player, 1f, 0.8f);

        vcamLockOn.Priority = vcamExplore.Priority - 1;
    }

    void SwitchTarget()
    {
        if (nearbyEnemies.Count <= 1) return;

        // escolhe o inimigo mais próximo que NÃO seja o atual
        Transform nearestOther = nearbyEnemies
            .Where(e => e != currentEnemy)
            .OrderBy(e => Vector3.Distance(player.position, e.position))
            .FirstOrDefault();

        if (nearestOther != null) SetLock(nearestOther);
    }

    // 🔹 Gizmo para visualizar a área no editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, searchRadius);
    }
}
