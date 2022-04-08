using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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

        //Dash
        playerInput.CharacterControls.Dash.started += OnDash;

        //Attack
        playerInput.CharacterControls.Attack.started += OnAttack;

        //Open Umbrella
        playerInput.CharacterControls.OpenUmbrella.started += OnUmbrella;
        playerInput.CharacterControls.OpenUmbrella.canceled += OnUmbrella;

        //Input Mouse
        playerInput.CharacterControls.CameraMovement.performed += OnMouseMovement;
        playerInput.CharacterControls.CameraMovement.canceled += OnMouseMovement;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            AkSoundEngine.PostEvent("StopBackgroundMusic_Level1", WwiseManager.instance.gameObject);
            SceneManager.LoadScene("MainMenu");
        }
    }

    private void OnJump(InputAction.CallbackContext ctx)
    {
        playerController.HandleJump();
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

    private void OnMouseMovement(InputAction.CallbackContext ctx)
    {
        Vector2 mouseAxis = ctx.ReadValue<Vector2>();
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