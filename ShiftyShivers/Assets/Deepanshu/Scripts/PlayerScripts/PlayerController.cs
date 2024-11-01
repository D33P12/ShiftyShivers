using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Rigidbody playerRigidbody;
    [SerializeField] private float walkSpeed = 10f;
    [SerializeField] private float rotationSpeed = 10f;
    
    [SerializeField] private GameObject DefaultPlayerVisual;
    [SerializeField] private Transform transformationHolder;
    [SerializeField] private List<GameObject> placeholderPrefabs;
    
    [SerializeField] private List<EnemyAIBase> nearbyEnemies;
    [SerializeField] private float idleAlertThreshold = 5f;
    [SerializeField] private List<HidingZone> hidingZone;
    
    [SerializeField] private TextMeshProUGUI alertCountdownText; 
    private float alertTimer = 0f;
    
    [SerializeField] Animator animator;
    
    private GameObject placeholderObject;
    public bool IsVisible => DefaultPlayerVisual.activeSelf;
    private bool isMoving;
    private float idleTimer = 0f;
    private bool disablePlaceholder = false;
    
    private Vector3 _movementDirection;
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        isMoving = true;
        animator = GetComponent<Animator>();
    }
    private void OnEnable()
    {
        inputManager.onMove += OnMove;
    }
    private void OnDisable()
    {
        inputManager.onMove -= OnMove;
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }
    private void Update()
    {
        UpdateAnimation();
    }

    private void OnMove(Vector2 inputValue)
    {
        _movementDirection = new Vector3(inputValue.x, 0, inputValue.y);
        
        if (_movementDirection != Vector3.zero && !isMoving)
        {
            if (!IsEnemyInChaseState())
            {
                TogglePlayerBody(true);
                if (placeholderObject != null)
                {
                    Destroy(placeholderObject);
                }
            }
            SoundManager.Instance.PlayAudio(AudioType.PLAYERWALK);
            isMoving = true;
            idleTimer = 0f;
        }
    }
    public bool IsPlayerStationary()
    {
        return _movementDirection == Vector3.zero;
    }
    private void HandleMovement()
    {
        if (_movementDirection != Vector3.zero)
        {
            alertTimer = 0f;
            alertCountdownText.text = string.Empty;
            
           

            Vector3 velocity = _movementDirection * walkSpeed;
        
            playerRigidbody.velocity = new Vector3(velocity.x, playerRigidbody.velocity.y, velocity.z);
        
            if (_movementDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(_movementDirection);
                targetRotation = Quaternion.Euler(0f, targetRotation.eulerAngles.y, 0f);
                playerTransform.rotation = Quaternion.Slerp(playerTransform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            
            }

            if (!IsEnemyInChaseState())
            {
                TogglePlayerBody(true);
                if (placeholderObject != null)
                {
                    Destroy(placeholderObject);
                }
            }
            isMoving = true;
            idleTimer = 0f;
        }
        else if (isMoving)
        {
            playerRigidbody.velocity = new Vector3(0f, playerRigidbody.velocity.y, 0f);
            if (!IsEnemyInChaseState())
            {
                TogglePlayerBody(false);

                if (!IsPlayerHiding())
                {
                    int randomIndex = Random.Range(0, placeholderPrefabs.Count);
                    placeholderObject = Instantiate(placeholderPrefabs[randomIndex], transformationHolder.position,
                        Quaternion.identity);
                    placeholderObject.transform.SetParent(transformationHolder);
                }
            }
            SoundManager.Instance.PlayAudio(AudioType.PLAYERIDLE);
            isMoving = false;
        }
        else if(!IsPlayerHiding())
        {
            alertTimer += Time.deltaTime;
            float timeRemaining = Mathf.Max(idleAlertThreshold - alertTimer, 0f);
            
            if (timeRemaining > 0f)
            {
                alertCountdownText.text = $"Enemies alerting in: {timeRemaining:F1}s";  // Display countdown format if you wanna change it...
            }
            else
            {
                alertCountdownText.text = " ";
                AlertNearbyEnemies();
                alertTimer = 0f;  
            }
        }
    }
    internal bool IsPlayerHiding()
    {
        foreach (var zone in hidingZone)
        {
            if (zone.playerIsHiding)
            {
                return true;
            }
        }
        return false;
    }
    private bool IsEnemyInChaseState()
    {
        foreach (EnemyAIBase enemy in nearbyEnemies)
        {
            if (enemy != null && enemy.CurrentState == AIState.CHASE)
            {
                return true;
            }
        }
        return false;
    }
    private void AlertNearbyEnemies()
    {
        foreach (EnemyAIBase enemy in nearbyEnemies)
        {
            enemy.ReceiveAlert(playerTransform.position);
        }
        foreach (EnemyAIBase enemy in nearbyEnemies)
        {
            enemy.ReceiveAlert(playerTransform.position);
        }
    }
    public void TogglePlayerBody(bool isVisible)
    {
        DefaultPlayerVisual.SetActive(isVisible);
    }
    private void UpdateAnimation()
    {
        Vector3 localVelocity = playerTransform.InverseTransformDirection(playerRigidbody.velocity);

        float forwardSpeed = localVelocity.x;
        float sideSpeed = localVelocity.z;
        
        sideSpeed = Mathf.Clamp(sideSpeed, -1f, 1f);
        
        animator.SetFloat("Y", forwardSpeed);
        animator.SetFloat("X", sideSpeed);
    }

}
