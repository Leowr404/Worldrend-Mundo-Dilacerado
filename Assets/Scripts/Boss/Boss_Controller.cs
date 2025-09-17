using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Boss_Controller : MonoBehaviour
{
    private NavMeshAgent enemy;
    public Transform point;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemy = GetComponent<NavMeshAgent>();
       
        
    }

    // Update is called once per frame
    void Update()
    {
        enemy.SetDestination(point.position);
        
    }
}
