using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
   public event Action<Vector2> onMove;
   
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
        
    }

    private void EnableInput()
    {
        inputs.PlayerMovement.Enable();

    }

    private void DisableInput()
    {
        inputs.PlayerMovement.Disable();
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        onMove?.Invoke(moveInput);
    }
    
    private void Update()
    {
        if (onMove != null)
        {
            onMove(moveInput);

        }
    }


    
}
