using UnityEngine;
using UnityEngine.AI;

public class FSM_Enemy_Patrulha : StateMachineBehaviour
{
    public string WaypointArea_Name;
    private GameObject Player;
    private GameObject WaypointArea;
    private int WaypointArea_Count = 0, WaypointArea_Choice = 0;
    private float stuckTimer;

    //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       if (GameObject.Find ("Player"))
        {
            Player = GameObject.Find("Player");

            // 1º: área definida pelo spawner (por inimigo). 2º: fallback pelo nome (cena fixa).
            EnemyPatrolArea patrol = animator.GetComponentInParent<EnemyPatrolArea>();
            if (patrol != null && patrol.waypointArea != null)
                WaypointArea = patrol.waypointArea.gameObject;
            else if (GameObject.Find(WaypointArea_Name))
                WaypointArea = GameObject.Find(WaypointArea_Name);

            if (WaypointArea != null)
            {
                WaypointArea_Count = WaypointArea.transform.childCount;
                WaypointArea_Choice = Random.Range( 0, WaypointArea_Count);
            }
        }
    }

    //OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       if (Player != null && WaypointArea != null)
        {
            NavMeshAgent agent = animator.transform.GetComponentInParent<NavMeshAgent>();
            if (agent == null) return;

            agent.isStopped = false; // garante que não ficou travado de outro estado

            Transform target = WaypointArea.transform.GetChild(WaypointArea_Choice);
            agent.destination = target.position;

            // chegou ao destino (distância no caminho, não em linha reta)
            bool chegou = !agent.pathPending && agent.remainingDistance <= 2f;

            // travou: parado sem chegar (caminho bloqueado/inalcançável)
            if (!agent.pathPending && agent.velocity.sqrMagnitude < 0.05f && agent.remainingDistance > 2f)
                stuckTimer += Time.deltaTime;
            else
                stuckTimer = 0f;

            // chegou OU travou por 1.5s → escolhe outro waypoint
            if (chegou || stuckTimer > 1.5f)
            {
                WaypointArea_Choice = Random.Range(0, WaypointArea_Count);
                stuckTimer = 0f;
            }

            animator.SetFloat("distancia", Vector3.Distance(animator.transform.position, Player.transform.position));
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
