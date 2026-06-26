using UnityEngine;

// Marca qual área de waypoints este inimigo deve patrulhar.
// O EnemySpawner preenche isso ao spawnar; o FSM_Enemy_Patrulha lê daqui.
public class EnemyPatrolArea : MonoBehaviour
{
    public Transform waypointArea; // objeto pai dos waypoints
}
