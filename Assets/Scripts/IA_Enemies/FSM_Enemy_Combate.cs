using UnityEngine;
using UnityEngine.AI;

public class FSM_Enemy_Combate : StateMachineBehaviour
{
    private GameObject Player;
    public float attackRange = 1.8f;
    public float attackCooldown = 1.5f;
    private float cooldownTimer;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Player = GameObject.Find("Player");
        cooldownTimer = 0f;

    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (Player == null) return;

        NavMeshAgent agent = animator.transform.GetComponentInParent<NavMeshAgent>();
        float dist = Vector3.Distance(animator.transform.position, Player.transform.position);

        animator.SetFloat("distancia", dist);
        agent.destination = Player.transform.position;

        if (dist <= attackRange)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0f)
            {
                animator.SetTrigger("Ataque");
                cooldownTimer = attackCooldown;
            }
        }
    }
}
