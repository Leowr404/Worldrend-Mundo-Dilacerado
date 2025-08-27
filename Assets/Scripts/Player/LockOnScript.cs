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
    public float searchFOV = 80f;
    public LayerMask enemyMask;

    private Transform currentEnemy;

    void Start()
    {
        targetGroup.Targets.Clear();

        // adiciona o player
        targetGroup.AddMember(player, 1f, 0.8f);
    }

    void Update()
    {
        if (lockAction.action.WasPressedThisFrame())
        {
            if (currentEnemy == null) TryLockNearest();
            else ClearLock();
        }

        if (currentEnemy != null)
        {
            float dir = switchAction.action.ReadValue<float>();
            if (Mathf.Abs(dir) > 0.5f) SwitchTarget(dir > 0 ? 1 : -1);
        }
    }

    void TryLockNearest()
    {
        var cam = Camera.main;
        if (!cam) return;

        var enemies = Physics.OverlapSphere(player.position, searchRadius, enemyMask)
                             .Select(c => c.transform).ToArray();

        Transform best = null;
        float bestScore = float.MinValue;

        foreach (var e in enemies)
        {
            Vector3 to = e.position - cam.transform.position;
            float dist = to.magnitude;
            float angle = Vector3.Angle(cam.transform.forward, to);
            if (angle > searchFOV * 0.5f) continue;

            float score = (1f / dist) + (1f - angle / (searchFOV * 0.5f));
            if (score > bestScore) { bestScore = score; best = e; }
        }

        if (best != null) SetLock(best);
    }

    void SetLock(Transform enemy)
    {
        currentEnemy = enemy;

        // garante que o player sempre está no grupo
        if (!targetGroup.Targets.Any(t => t.Object == player))
            targetGroup.AddMember(player, 1f, 0.8f);

        // adiciona inimigo se ainda não estiver
        if (!targetGroup.Targets.Any(t => t.Object == enemy))
            targetGroup.AddMember(enemy, 1f, 0.8f);

        // ativa câmera lock
        vcamLockOn.Priority = vcamExplore.Priority + 1;
    }

    void ClearLock()
    {
        if (currentEnemy != null)
        {
            targetGroup.RemoveMember(currentEnemy);
            currentEnemy = null;
        }

        // volta pra câmera normal
        vcamLockOn.Priority = vcamExplore.Priority - 1;
    }

    void SwitchTarget(int dir)
    {
        var cam = Camera.main;
        if (!cam || currentEnemy == null) return;

        var enemies = Physics.OverlapSphere(player.position, searchRadius, enemyMask)
                             .Select(c => c.transform).Where(e => e != currentEnemy).ToArray();

        Transform best = null;
        float bestX = dir > 0 ? float.NegativeInfinity : float.PositiveInfinity;

        foreach (var e in enemies)
        {
            Vector3 vp = cam.WorldToViewportPoint(e.position);
            if (vp.z < 0) continue;

            if (dir > 0 && vp.x > bestX) { bestX = vp.x; best = e; }
            if (dir < 0 && vp.x < bestX) { bestX = vp.x; best = e; }
        }

        if (best != null) SetLock(best);
    }
}
