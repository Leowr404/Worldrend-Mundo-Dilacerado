using UnityEngine;

[RequireComponent(typeof(Collider))]
public class EnemyHitboxTrigger : MonoBehaviour
{
    public int damage = 10;

    private Collider col;

    void Awake()
    {
        col = GetComponent<Collider>();
        col.isTrigger = true;
        col.enabled = false;
    }

    public void Enable() => col.enabled = true;
    public void Disable() => col.enabled = false;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerStats stats = other.GetComponent<PlayerStats>();
        if (stats != null) stats.TakeDamage(damage);
    }
}
