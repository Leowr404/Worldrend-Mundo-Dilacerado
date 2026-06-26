using System.Collections.Generic;
using UnityEngine;

// Spawna inimigos numa área e os mantém preenchidos:
// quando um morre, respawna outro após um tempo até atingir maxEnemies.
public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn")]
    public GameObject enemyPrefab;
    public int maxEnemies = 5;
    public float spawnRadius = 5f;     // raio onde os inimigos aparecem
    public float respawnDelay = 8f;    // segundos até repor um inimigo morto

    [Header("Patrulha")]
    public Transform waypointArea;     // área de waypoints que estes inimigos patrulham

    private List<GameObject> aliveEnemies = new List<GameObject>();
    private float respawnTimer;

    void Start()
    {
        for (int i = 0; i < maxEnemies; i++)
            SpawnOne();
    }

    void Update()
    {
        // remove referências de inimigos destruídos (mortos)
        aliveEnemies.RemoveAll(e => e == null);

        if (aliveEnemies.Count < maxEnemies)
        {
            respawnTimer -= Time.deltaTime;
            if (respawnTimer <= 0f)
            {
                SpawnOne();
                respawnTimer = respawnDelay;
            }
        }
    }

    void SpawnOne()
    {
        if (enemyPrefab == null) return;

        Vector2 circle = Random.insideUnitCircle * spawnRadius;
        Vector3 pos = transform.position + new Vector3(circle.x, 0f, circle.y);

        GameObject enemy = Instantiate(enemyPrefab, pos, Quaternion.identity);

        // define a área de patrulha deste inimigo
        EnemyPatrolArea patrol = enemy.GetComponent<EnemyPatrolArea>();
        if (patrol == null) patrol = enemy.AddComponent<EnemyPatrolArea>();
        patrol.waypointArea = waypointArea;

        aliveEnemies.Add(enemy);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);

        if (waypointArea != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, waypointArea.position);
        }
    }
}
