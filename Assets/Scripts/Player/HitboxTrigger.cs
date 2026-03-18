using UnityEngine;

[RequireComponent(typeof(Collider))]
public class HitboxTrigger : MonoBehaviour
{
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
        Debug.Log($"HitboxTrigger colidiu com: {other.gameObject.name} | Tag: {other.tag}");

        if (other.CompareTag("Player")) return;

        Debug.Log($"Registrando hit em: {other.gameObject.name}");
        HitboxManager.Instance?.RegisterHit(other.gameObject);
    }
}