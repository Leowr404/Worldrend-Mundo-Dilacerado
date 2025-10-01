using UnityEngine;
using UnityEngine.UIElements;

public class Boss_Ia : MonoBehaviour
{
    public Transform player;       // Arraste o Player aqui no Inspector
    private UnityEngine.AI.NavMeshAgent agent;
    public float Velocidade = 5f;

    private Vector3 Point_Inicial;

    private GameObject position_inicial;

    

    void Awake()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        agent.speed = Velocidade;
       

    }

    void Start()
    {
        Point_Inicial = transform.position;
        
    }




    void Update()
    {
        if (player != null)
        {
            // Define o destino do Boss como a posição atual do Player
            agent.SetDestination(player.position);
            agent.stoppingDistance = 3f;
            

        }

    }
    
     

 
}