using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private PlayerInput playerInput;
    private CharacterController characterController;
    private Animator animator;

    //Variables para almacenar los ID's de las animaciones
    private int isWalkingHash;

    private int isRunningHash;

    private Vector2 currentMovementInput;
    private Vector3 currentMovement;
    private Vector3 currentRunMovement;

    private bool isMovementPressed;
    private bool isRunPressed;
    [SerializeField] private float rotationFactorPerFrame = 15f;
    private float runSpeed = 3;

    private void Awake()
    {
        playerInput = new PlayerInput();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningHash = Animator.StringToHash("isRunning");

        //Player inputs callbacks
        playerInput.CharacterControls.Move.started += OnMovementInput;
        playerInput.CharacterControls.Move.performed += OnMovementInput;
        playerInput.CharacterControls.Move.canceled += OnMovementInput;

        playerInput.CharacterControls.Run.started += OnRun;
        playerInput.CharacterControls.Run.performed += OnRun;
        playerInput.CharacterControls.Run.canceled += OnRun;
    }

    private void OnMovementInput(InputAction.CallbackContext ctx)
    {
        currentMovementInput = ctx.ReadValue<Vector2>();

        //Walk
        currentMovement.x = currentMovementInput.x;
        currentMovement.z = currentMovementInput.y;
        //Run
        currentRunMovement.x = currentMovementInput.x * runSpeed;
        currentRunMovement.z = currentMovementInput.y * runSpeed;

        isMovementPressed = currentMovement.x != 0 || currentMovement.z != 0;
    }

    private void OnRun(InputAction.CallbackContext ctx)
    {
        isRunPressed = ctx.ReadValueAsButton();
    }

    private void Update()
    {
        HandleGravity();
        HandleRotation();
        HandleAnimation();
        if (isRunPressed)
        {
            characterController.Move(currentRunMovement * Time.deltaTime);
        }
        else
        {
            characterController.Move(currentMovement * Time.deltaTime);
        }
    }

    private void HandleRotation()
    {
        Vector3 positionToLookAt;

        //Setear la rotacion en la cual va a girar
        positionToLookAt.x = currentMovement.x;
        positionToLookAt.y = 0.0f;
        positionToLookAt.z = currentMovement.z;

        //Obtenemos la rotacion actual del Player
        Quaternion currentRotation = transform.rotation;

        if (isMovementPressed)
        {
            //Hacemos una interpolación en la rotacion que va a girar cuando el player se mueve
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationFactorPerFrame * Time.deltaTime);
        }
    }

    private void HandleGravity()
    {
        if (characterController.isGrounded)
        {
            float groundedGravity = -.05f;
            currentMovement.y = groundedGravity;
            currentRunMovement.y = groundedGravity;
        }
        else
        {
            float gravity = -9.8f;
            currentMovement.y += gravity;
            currentRunMovement.y += gravity;
        }
    }

    private void HandleAnimation()
    {
        bool isWalking = animator.GetBool(isWalkingHash);
        bool isRunning = animator.GetBool(isRunningHash);

        //Walk Check
        if (isMovementPressed && !isWalking)
            animator.SetBool("isWalking", true);
        else if (!isMovementPressed && isWalking)
            animator.SetBool("isWalking", false);

        //Running Check
        if ((isMovementPressed && isRunPressed) && !isRunning)
            animator.SetBool(isRunningHash, true);
        else if ((!isMovementPressed || !isRunning) && isRunning)
            animator.SetBool(isRunningHash, false);
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