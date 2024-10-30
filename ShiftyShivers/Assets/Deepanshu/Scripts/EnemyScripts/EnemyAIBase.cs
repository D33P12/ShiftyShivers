using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum AIState
{
    IDLE, PATROL, CHASE ,DEATH, ATTACK

}

public class EnemyAIBase : MonoBehaviour
{
    [SerializeField] GameObject playerObject;
    [SerializeField] float playerDistance = 10f;
    [SerializeField] float patrolRadius = 3f;
    [SerializeField] List<Transform> patrolPoints = new List<Transform>();
    [SerializeField] float idleDelay = 3f; 
    
    [SerializeField] float fieldOfView = 45f;
    [SerializeField] int coneResolution = 10;
    
    [SerializeField] private float idleAlertThreshold = 5f; 
    private float alertTimer = 0f;
    [SerializeField] List<EnemyAIBase> nearbyEnemies;
    
    [SerializeField] List<HidingZone> hidingZone;
    
    [SerializeField] public float damage;
    public PlayerHealthScript playerHealth;
    [SerializeField] float attackRange = 2f;
    [SerializeField] float attackCooldown = 1f;
    [SerializeField] float attackTimer = 0f;
    
    public AIState CurrentState => state;
    private UnityEngine.AI.NavMeshAgent agent;
    private float idleTimer = 0f;
    private int currentPatrolPoint = 0;
    private AIState state;
    
   [SerializeField] private Animator anim;


    void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>(); 
        currentPatrolPoint = 0;
        ChangeState(AIState.IDLE); 
        anim = GetComponent<Animator>();
        UpdateAnimationState();

    }

    void Update()
    {
        
        CheckForPlayer();

        switch (state)
        {
            case AIState.IDLE:
                Idle();
                break;
            case AIState.PATROL:
                Patrol();
                break;
            case AIState.CHASE:
                Chase();
                break;
            case AIState.ATTACK:
                Attack();
                break;
            case AIState.DEATH:
                DeathState();
                break;
        }
    }

    void ChangeState(AIState newState)
    {
        state = newState;
        idleTimer = 0f; 
        attackTimer = 0f;
        UpdateAnimationState();

    }

    void CheckForPlayer()
    {
        bool playerIsStationary = playerObject.GetComponent<PlayerController>().IsPlayerStationary();

        if (!IsPlayerInAnyHidingZone() && IsPlayerInCone() && DistanceCheck(transform.position, playerObject.transform.position, playerDistance) && state != AIState.ATTACK)
        {
            if (playerIsStationary)
            {
                alertTimer += Time.deltaTime;
                if (alertTimer >= idleAlertThreshold)
                {
                    ChangeState(AIState.CHASE);
                    AlertNearbyEnemies(playerObject.transform.position);
                    alertTimer = 0f;
                }
            }
            else
            {
                ChangeState(AIState.CHASE);
                AlertNearbyEnemies(playerObject.transform.position);
                alertTimer = 0f;
            }
        }
        else if (state == AIState.CHASE)
        {
            idleTimer += Time.deltaTime; 
            if (idleTimer >= idleDelay)
            {
                ChangeState(AIState.IDLE); 
            }
        }
        else
        {
            alertTimer = 0f;
        }
    }
    private bool IsPlayerInAnyHidingZone()
    {
        foreach (HidingZone zone in hidingZone)
        {
            if (zone.playerIsHiding)
            {
                return true;
            }
        }
        return false;
    }
    void AlertNearbyEnemies(Vector3 lastKnownPosition)
    {
        foreach (EnemyAIBase enemy in nearbyEnemies)
        { 
            enemy.ReceiveAlert(lastKnownPosition);
        }
    }

    public void ReceiveAlert(Vector3 lastKnownPosition)
    {
        if (state != AIState.CHASE)  
        {
            ChangeState(AIState.CHASE);
            SetDestination(lastKnownPosition);
        }
    }
    
    public bool IsPlayerInCone()
    {
        if (playerObject == null) return false;

        Vector3 directionToPlayer = playerObject.transform.position - transform.position;
        directionToPlayer.y = 0; 

        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        if (angleToPlayer < fieldOfView / 2f)
        {
            Ray ray = new Ray(transform.position, directionToPlayer.normalized);
            RaycastHit hit;

            int layerMask = LayerMask.GetMask("Obstacle");

            if (Physics.Raycast(ray, out hit, playerDistance, layerMask))
            {
                if (hit.transform != playerObject.transform)
                {
                    return false;
                }
            }
            return true;
        }

        return false;
    }


    bool DistanceCheck(Vector3 position1, Vector3 position2, float distance)
    {
        float currentDistance = Vector3.Distance(position1, position2);
        return currentDistance < distance;
    }

    void Idle()
    {
        idleTimer += Time.deltaTime;
        
        if (agent.velocity != Vector3.zero)
        {
            agent.velocity = Vector3.zero;
        }
        
        if (idleTimer >= idleDelay)
        {
            ChangeState(AIState.PATROL);
        }
    }

    void Patrol()
    {
        if (patrolPoints.Count == 0)
            return;

        if ((transform.position - patrolPoints[currentPatrolPoint].position).magnitude < patrolRadius)
        {
            currentPatrolPoint = GetNextPatrolPoint();
        }

        SetDestination(patrolPoints[currentPatrolPoint]);
    }

    int GetNextPatrolPoint()
    {
        return (currentPatrolPoint + 1) % patrolPoints.Count;
    }

    public void Chase()
    {
        if (DistanceCheck(transform.position, playerObject.transform.position, attackRange))
        {
            ChangeState(AIState.ATTACK);
        }
        else if (DistanceCheck(transform.position, playerObject.transform.position, playerDistance))
        {
            if (IsPlayerInCone())
            {
                SetDestination(playerObject.transform.position);
            }
            else
            {
                idleTimer += Time.deltaTime;
                if (idleTimer >= idleDelay)
                {
                    ChangeState(AIState.IDLE);
                }
            }
        }
        else
        {
            idleTimer += Time.deltaTime;
            if (idleTimer >= idleDelay)
            {
                ChangeState(AIState.IDLE);
            }
        }

        FacePlayer();
    }
    void Attack()
    {
        if (DistanceCheck(transform.position, playerObject.transform.position, attackRange))
        {
            attackTimer += Time.deltaTime;
            if (attackTimer >= attackCooldown)
            {
                PerformAttack();
                attackTimer = 0f;
            }
        }
        else
        {
            ChangeState(AIState.CHASE);
        }
        FacePlayer();
    }
    void PerformAttack()
    {
        if (playerHealth != null)
        {
           GameManager.phealth -= damage;
        }
    }


    void SetDestination(Transform destinationTransform)
    {
        if (agent != null)
        {
            agent.SetDestination(destinationTransform.position);
        }
    }

    void SetDestination(Vector3 position)
    {
        if (agent != null)
        {
            agent.SetDestination(position);
        }
    }

    private void FacePlayer()
    {
        if (playerObject == null) return;

        Vector3 directionToPlayer = playerObject.transform.position - transform.position;
        directionToPlayer.y = 0; 

        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 forwardDirection = transform.forward * playerDistance;

        Vector3 leftBoundary = Quaternion.Euler(0, -fieldOfView / 2f, 0) * forwardDirection;
        Vector3 rightBoundary = Quaternion.Euler(0, fieldOfView / 2f, 0) * forwardDirection;

        Gizmos.DrawRay(transform.position, leftBoundary);
        Gizmos.DrawRay(transform.position, rightBoundary);
        Gizmos.DrawWireSphere(transform.position, playerDistance); 
    }
    public void DeathState()
    {
        anim.SetBool("isIdling", false);
        anim.SetBool("isWalking", false);
        anim.SetBool("isRunning", false);
        anim.SetBool("isAttacking", false);
        anim.SetBool("isDead", true);
        
        if (agent != null)
        {
            agent.isStopped = true;        
            agent.enabled = false; 
        }
        this.enabled = false; 
    }
    private void UpdateAnimationState()
    {
        if (anim != null)
        {
            anim.SetBool("isIdling", state == AIState.IDLE);
            anim.SetBool("isWalking", state == AIState.PATROL);
            anim.SetBool("isRunning", state == AIState.CHASE);
            anim.SetBool("isAttacking", state == AIState.ATTACK);
            anim.SetBool("isDead", state == AIState.IDLE);
        }
    }

}
