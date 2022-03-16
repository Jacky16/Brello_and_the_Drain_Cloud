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

        //Dash
        playerInput.CharacterControls.Dash.started += OnDash;

        //Attack
        playerInput.CharacterControls.Attack.started += OnAttack;

        //Open Umbrella
        playerInput.CharacterControls.OpenUmbrella.started += OnUmbrella;
        playerInput.CharacterControls.OpenUmbrella.canceled += OnUmbrella;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
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

    private void OnDash(InputAction.CallbackContext ctx)
    {
        playerController.HandleDash();
    }

    private void OnAttack(InputAction.CallbackContext ctx)
    {
        playerController.HandleAttack();
    }

    private void OnUmbrella(InputAction.CallbackContext ctx)
    {
        //Abrir en el aire
        //playerController.SetGladePressed(ctx.ReadValueAsButton());

        //Abrir en el suelo
        playerController.OpenUmbrellaManager(ctx.ReadValueAsButton());
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