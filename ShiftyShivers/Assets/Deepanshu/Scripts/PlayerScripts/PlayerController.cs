using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using TMPro;
using Unity.VisualScripting;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Rigidbody playerRigidbody;
    [SerializeField] private float walkSpeed = 10f;
    [SerializeField] private float rotationSpeed = 10f;
    
    [SerializeField] private MeshRenderer playerBodyMeshRenderer;
    [SerializeField] private List<GameObject> placeholderPrefabs;
    
    [SerializeField] private List<EnemyAIBase> nearbyEnemies;
    [SerializeField] private float idleAlertThreshold = 5f;
    [SerializeField] private List<HidingZone> hidingZone;
    
    [SerializeField] private TextMeshProUGUI alertCountdownText; 
    private float alertTimer = 0f;
    
    private GameObject placeholderObject;
    private bool isMoving;
    private float idleTimer = 0f;
    
    [SerializeField] private CinemachineBrain cinemachineBrain;
    
    private Vector3 _movementDirection;
    private void Start()
    {
        //  Cursor.lockState = CursorLockMode.Locked;
        isMoving = true;
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
    private void OnMove(Vector2 inputValue)
    {
        _movementDirection = new Vector3(inputValue.x, 0, inputValue.y);
        
        if (_movementDirection != Vector3.zero && !isMoving)
        {
            TogglePlayerBody(true);
            if (placeholderObject != null)
            {
                Destroy(placeholderObject);
            }
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
            
            Transform activeCameraTransform = GetActiveCameraTransform();

            if (activeCameraTransform != null)
            {
                Vector3 cameraForward = activeCameraTransform.forward;
                Vector3 cameraRight = activeCameraTransform.right;

                cameraForward.y = 0;
                cameraRight.y = 0;
                cameraForward.Normalize();
                cameraRight.Normalize();

                Vector3 adjustedMovement = cameraRight * _movementDirection.x + cameraForward * _movementDirection.z;
                Vector3 velocity = adjustedMovement * walkSpeed;
                playerRigidbody.velocity = new Vector3(velocity.x, playerRigidbody.velocity.y, velocity.z);

                Quaternion targetRotation = Quaternion.LookRotation(adjustedMovement);
                targetRotation = Quaternion.Euler(0f, targetRotation.eulerAngles.y, 0f);
                playerTransform.rotation = Quaternion.Slerp(playerTransform.rotation, targetRotation,
                    rotationSpeed * Time.deltaTime);
            }
            TogglePlayerBody(true);
            if (placeholderObject != null)
            {
                Destroy(placeholderObject);
            }
            isMoving = true;
            idleTimer = 0f;
        }
        else if (isMoving)
        {
            playerRigidbody.velocity = new Vector3(0f, playerRigidbody.velocity.y, 0f);
            TogglePlayerBody(false);
            
            if (!IsPlayerHiding())
            {
                int randomIndex = Random.Range(0, placeholderPrefabs.Count);
                placeholderObject = Instantiate(placeholderPrefabs[randomIndex], playerTransform.position,
                    Quaternion.identity);
            }

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
    private bool IsPlayerHiding()
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
    private void TogglePlayerBody(bool isVisible)
    {
        playerBodyMeshRenderer.enabled = isVisible;
    }
    private Transform GetActiveCameraTransform()
    {
        if (cinemachineBrain.ActiveVirtualCamera != null)
        {
            CinemachineVirtualCamera activeVirtualCamera = cinemachineBrain.ActiveVirtualCamera as CinemachineVirtualCamera;
            if (activeVirtualCamera != null)
            {
                return activeVirtualCamera.transform;
            }
        }
        return null;
    }
}
