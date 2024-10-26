using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
   public event Action<Vector2> onMove;
   public event Action<bool> onOptionmenu;
   public event Action<bool> onInteract;
   
    private Inputs inputs;
    private Vector2 moveInput;
    
    private void OnEnable()
    {
        SetupInput();
        EnableInput();
    }

    private void OnDisable()
    {
        DisableInput();
    }

    private void SetupInput()
    {
        inputs = new Inputs();
        
        inputs.PlayerMovement.Move.performed += OnMove;
        inputs.PlayerInteract.OptionMenu.performed += OnOptionmenu;
        inputs.PlayerInteract.Interact.performed += OnInteract;
    }

    private void EnableInput()
    {
        inputs.PlayerMovement.Enable();
        inputs.PlayerInteract.Enable();
    }

    private void DisableInput()
    {
        inputs.PlayerMovement.Disable();
        inputs.PlayerInteract.Disable();
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        onMove?.Invoke(moveInput);
    }
    private void OnOptionmenu(InputAction.CallbackContext context)
    {
        bool isInteracting = context.ReadValueAsButton();
        onOptionmenu?.Invoke(isInteracting);
    }
    private void OnInteract(InputAction.CallbackContext context)
    {
        bool isInteracting = context.ReadValueAsButton();
        onInteract?.Invoke(isInteracting);
    }
    
    private void Update()
    {
        if (onMove != null)
        {
            onMove(moveInput);
        }
    }


    
}
