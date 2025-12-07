using UnityEngine;
using UnityEngine.AI;

public class FSM_Enemy_Combate : StateMachineBehaviour
{
    private GameObject Player;
    public float TempoDisparo = 2;
    public GameObject Projetil;
    private float contadorDisparo;

    //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       if (GameObject.Find ("Player"))
        {
            Player = GameObject.Find("Player");
        }
    }

    //OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       if (Player !=null)
        {
            contadorDisparo +=Time.deltaTime;  //contador de disparo

            if(contadorDisparo >=TempoDisparo)
            {
                GameObject novoProjetil = GameObject.Instantiate(
                    Projetil,
                    animator.transform.GetChild(0).transform.position,
                    animator.transform.GetChild(0).transform.rotation
                );

                novoProjetil.transform.GetComponent<Rigidbody>().AddForce(novoProjetil.transform.forward * 1000 * Time.deltaTime,ForceMode.VelocityChange);
                GameObject.Destroy(novoProjetil, 3);

                contadorDisparo = 0f;           
            }
            animator.transform.GetComponent<Animator>().SetFloat("distancia", Vector3.Distance(animator.transform.position,Player.transform.position));
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
