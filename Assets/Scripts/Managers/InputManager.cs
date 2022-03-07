using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private PlayerInput playerInput;
    private PlayerController playerController;

    private void Awake()
    {
        playerInput = new PlayerInput();
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        //Movement
        playerInput.CharacterControls.Move.started += OnMovementInput;
        playerInput.CharacterControls.Move.performed += OnMovementInput;
        playerInput.CharacterControls.Move.canceled += OnMovementInput;

        //Jump
        playerInput.CharacterControls.Jump.started += OnJump;
        playerInput.CharacterControls.Jump.canceled += OnJump;

        //Glade
        playerInput.CharacterControls.Glade.started += OnGlade;
        playerInput.CharacterControls.Glade.canceled += OnGlade;

        //Attack
        playerInput.CharacterControls.Attack.started += OnAttack;
    }

    private void OnGlade(InputAction.CallbackContext ctx)
    {
        playerController.SetGladePressed(ctx.ReadValueAsButton());
    }

    private void OnJump(InputAction.CallbackContext ctx)
    {
        playerController.SetJumPressed(ctx.ReadValueAsButton());
    }

    private void OnMovementInput(InputAction.CallbackContext ctx)
    {
        Vector2 axis = ctx.ReadValue<Vector2>();

        playerController.SetAxis(axis);
        playerController.SetMovementPressed(axis.x != 0 || axis.y != 0);
    }

    private void OnAttack(InputAction.CallbackContext ctx)
    {
        playerController.HandleAttack();
    }

    private void OnEnable()
    {
        playerInput.Enable();
    }

    private void OnDisable()
    {
        playerInput.Disable();
    }
}