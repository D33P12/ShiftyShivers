using UnityEngine;
using Cinemachine;
using Unity.VisualScripting;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Rigidbody playerRigidbody;
    [SerializeField] private float walkSpeed = 10f;
    [SerializeField] private float rotationSpeed = 10f;
    
    [SerializeField] private CinemachineBrain cinemachineBrain;
    private Vector3 _movementDirection;
    private void Start()
    {
        //  Cursor.lockState = CursorLockMode.Locked;
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
    }
    private void HandleMovement()
    {
        if (_movementDirection != Vector3.zero)
        {
            
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
                playerTransform.rotation = Quaternion.Slerp(playerTransform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
        else
        {
            playerRigidbody.velocity = new Vector3(0f, playerRigidbody.velocity.y, 0f);
        }
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
