using UnityEngine;
using UnityEngine.AI;

public class FSM_Enemy_Patrulha : StateMachineBehaviour
{
    public string WaypointArea_Name;
    private GameObject Player;
    private GameObject WaypointArea;
    private int WaypointArea_Count = 0, WaypointArea_Choice = 0;

    //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       if (GameObject.Find ("Player"))
        {
            Player = GameObject.Find("Player");
            if(GameObject.Find(WaypointArea_Name))
            {
                WaypointArea = GameObject.Find(WaypointArea_Name);
                WaypointArea_Count = WaypointArea.transform.childCount;
                WaypointArea_Choice = Random.Range( 0, WaypointArea_Count);
                animator.transform.GetChild(0).transform.Rotate( -90, 0, 0);
            }
        }
    }

    //OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       if (Player !=null && WaypointArea != null)
        {
            animator.transform.GetComponent<NavMeshAgent>().destination = WaypointArea.transform.GetChild(WaypointArea_Choice).transform.position;
           
            if (Vector3.Distance(animator.transform.position,WaypointArea.transform.GetChild(WaypointArea_Choice).transform.position)<2f)
            {
                WaypointArea_Choice = Random.Range( 0, WaypointArea_Count);
            }
            animator.transform.GetComponent<Animator>().SetFloat("distancia", Vector3.Distance(animator.transform.position, Player.transform.position));
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
