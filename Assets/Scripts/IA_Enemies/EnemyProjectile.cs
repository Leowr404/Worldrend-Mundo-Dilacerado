using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [Header("Configuração do Dano")]
    public int damage = 10;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            HealthPlayer hp = other.GetComponentInParent<HealthPlayer>();

            if (hp != null)
            {
                hp.TakeDamage(damage);
            }

            Destroy(gameObject);
        }
    }
}
