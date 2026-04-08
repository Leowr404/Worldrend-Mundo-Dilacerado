using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class EnemyHitHeavy : MonoBehaviour
{
    AudioManager audioManager;
    [Header("Knockback")]
    public float knockbackForce = 12f;
    public float upwardForce = 4f;
    public float torqueForce = 8f;

    [Header("Ragdoll")]
    public float ragdollDuration = 1.8f;

    [Header("Som")]
    public AudioClip heavyHitSound;

    private Rigidbody rb;
    private AudioSource audioSource;
    private Animator animator;
    private bool isInRagdoll = false;
    private UnityEngine.AI.NavMeshAgent navAgent;

    void Awake()
    {
       // audioManager = AudioManager.instancia;
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        navAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        audioSource.spatialBlend = 1f;
        audioSource.maxDistance = 60f;
        audioSource.rolloffMode = AudioRolloffMode.Linear;

        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints.FreezeAll;
    }

    public void TakeHeavyHit(Vector3 attackerPosition)
    {
        if (isInRagdoll) return;
        StartCoroutine(HeavyHitSequence(attackerPosition));
    }

    private IEnumerator HeavyHitSequence(Vector3 attackerPosition)
    {
        isInRagdoll = true;

        if (navAgent != null) navAgent.enabled = false;

        if (AudioManager.instancia != null)
            AudioManager.instancia.PlaySFX(AudioManager.instancia.HeavyHit);
        //AudioManager.instancia.PlaySFX(AudioManager.instancia.Scream);
        if (heavyHitSound != null)
            audioSource.PlayOneShot(heavyHitSound);

        rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints.None;
        rb.useGravity = true;          // ← ativa
        rb.centerOfMass = new Vector3(0f, 1.5f, 0f);

        if (animator != null)
            animator.enabled = false;

        Vector3 dir = (transform.position - attackerPosition).normalized;
        dir.y = 0f;
        rb.AddForce(dir * knockbackForce, ForceMode.Impulse);
        rb.AddForce(Vector3.up * upwardForce, ForceMode.Impulse);

        Vector3 randomTorque = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f)
        ) * torqueForce;
        rb.AddTorque(randomTorque, ForceMode.Impulse);

        yield return new WaitForSeconds(ragdollDuration);

        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints.FreezeAll;
        rb.useGravity = false;         // ← desativa
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.centerOfMass = Vector3.zero;

        if (animator != null)
            animator.enabled = true;

        if (navAgent != null) navAgent.enabled = true;

        isInRagdoll = false;
    }
}