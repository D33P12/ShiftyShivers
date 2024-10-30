using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public enum TreatState
{
    IDLE, DEATH, WONDER, RUNAWAY

}
public class TastyTreat : MonoBehaviour
{
    public TreatState CurrentState => state;
    private NavMeshAgent agent;
    private float idleTimer = 0f;
    private TreatState state;
    private float runawayTimer = 0f;
    
    [SerializeField] private float runawayDuration = 5f;
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float fieldOfView = 60f;
    [SerializeField] private Transform player;
    [SerializeField] private List<Transform> patrolPoints;
    [SerializeField] private Animator anim;
    
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        ChangeState(TreatState.IDLE);
        UpdateAnimationState();
    }

    private void Update()
    {
        switch (state)
        {
            case TreatState.IDLE:
                Idle();
                break;
            case TreatState.DEATH:
                EatenState();
                break;
            case TreatState.WONDER:
                WonderState();
                break;
            case TreatState.RUNAWAY:
                RunawayState();
                break;
        }
        DetectPlayer();
    }

    private void ChangeState(TreatState newState)
    {
        state = newState;
        idleTimer = 0f;
        runawayTimer = 0f;
        UpdateAnimationState();
    }

    private void Idle()
    {
        idleTimer += Time.deltaTime;
        if (agent.velocity != Vector3.zero)
        {
            agent.velocity = Vector3.zero;
        }
        if (idleTimer > 2f)
        {
            ChangeState(TreatState.WONDER);
        }
    }

    internal void EatenState()
    {
        anim.SetBool("isIdling", false);
        anim.SetBool("isWalking", false);
        anim.SetBool("isRunning", false);
        anim.SetBool("isDead", true);
        if (agent != null)
        {
            agent.isStopped = true;        
            agent.enabled = false; 
        }
        this.enabled = false; 
    }

    private void WonderState()
    {
        if (!agent.hasPath || agent.remainingDistance < 0.5f)
        {
            int randomPoint = Random.Range(0, patrolPoints.Count);
            agent.SetDestination(patrolPoints[randomPoint].position);
        }
    }

    private void RunawayState()
    {
        Vector3 directionAwayFromPlayer = (transform.position - player.position).normalized;
        Vector3 randomRunawayPoint = transform.position + directionAwayFromPlayer * detectionRange;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomRunawayPoint, out hit, detectionRange, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    private void DetectPlayer()
    {
        if (player.GetComponent<PlayerController>().IsPlayerHiding()  || !player.GetComponent<PlayerController>().IsVisible)
        {
            return;
        }
        
        Vector3 directionToPlayer = player.position - transform.position;
        float angle = Vector3.Angle(directionToPlayer, transform.forward);

        if (directionToPlayer.magnitude <= detectionRange && angle <= fieldOfView / 2)
        {
            if (!Physics.Raycast(transform.position, directionToPlayer.normalized, directionToPlayer.magnitude, LayerMask.GetMask("Obstacle")))
            {
                ChangeState(TreatState.RUNAWAY);
            }
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector3 leftBoundary = Quaternion.Euler(0, -fieldOfView / 2, 0) * transform.forward * detectionRange;
        Vector3 rightBoundary = Quaternion.Euler(0, fieldOfView / 2, 0) * transform.forward * detectionRange;

        Gizmos.DrawRay(transform.position, leftBoundary);
        Gizmos.DrawRay(transform.position, rightBoundary);
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
    private void UpdateAnimationState()
    {
        if (anim != null)
        {
            anim.SetBool("isIdling", state == TreatState.IDLE);
            anim.SetBool("isWalking", state == TreatState.WONDER);
            anim.SetBool("isRunning", state == TreatState.RUNAWAY);
           // anim.SetBool("isAttacking", state == TreatState.DEATH);
        }
    }
}
