using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [Header("Configuração do Dano")]
    public int damage = 50;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStats hp = other.GetComponentInParent<PlayerStats>();

            if (hp != null)
            {
                hp.TakeDamage(damage);
            }
            
            Destroy(gameObject);
        }
    }
}
