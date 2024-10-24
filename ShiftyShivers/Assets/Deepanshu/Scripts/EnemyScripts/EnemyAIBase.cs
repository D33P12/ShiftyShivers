using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

enum AIState
{
    IDLE, PATROL, CHASE
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
        if (IsPlayerInCone() && DistanceCheck(transform.position, playerObject.transform.position, playerDistance))
        {
            ChangeState(AIState.CHASE); 
        }
        else if (state == AIState.CHASE)
        {
            idleTimer += Time.deltaTime; 
            if (idleTimer >= idleDelay)
            {
                ChangeState(AIState.IDLE); 
            }
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

}
