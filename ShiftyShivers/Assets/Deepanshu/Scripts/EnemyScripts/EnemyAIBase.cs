using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

enum AIState
{
    IDLE, PATROL, CHASE ,DEATH
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
    
    private UnityEngine.AI.NavMeshAgent agent;
    private float idleTimer = 0f;
    private int currentPatrolPoint = 0;
    private AIState state;

    void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>(); 
        currentPatrolPoint = 0;
        ChangeState(AIState.IDLE); 
    }

    void Update()
    {
        if (state == AIState.DEATH) return;
        
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
        }
    }

    void ChangeState(AIState newState)
    {
        state = newState;
        idleTimer = 0f; 
    }

    void CheckForPlayer()
    {
        bool playerIsStationary = playerObject.GetComponent<PlayerController>().IsPlayerStationary();

        if (!IsPlayerInAnyHidingZone() && IsPlayerInCone() && DistanceCheck(transform.position, playerObject.transform.position, playerDistance))
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
    
    bool IsPlayerInCone()
    {
        if (playerObject == null) return false;

        Vector3 directionToPlayer = playerObject.transform.position - transform.position;
        directionToPlayer.y = 0; 

        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        return angleToPlayer < fieldOfView / 2f; 
    }


    bool DistanceCheck(Vector3 position1, Vector3 position2, float distance)
    {
        float currentDistance = Vector3.Distance(position1, position2);
        return currentDistance < distance;
    }

    void Idle()
    {
        idleTimer += Time.deltaTime;
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

    void Chase()
    {
        if (DistanceCheck(transform.position, playerObject.transform.position, playerDistance))
        {
            SetDestination(playerObject.transform.position);
            FacePlayer(); 
            idleTimer = 0f; 
        }
        else
        {
            idleTimer += Time.deltaTime;
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
        if (agent != null)
        {
            agent.isStopped = true;
            agent.enabled = false;
        }
        Destroy(gameObject, 2f);
    }
}
