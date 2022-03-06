using System;
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
    private int isJumpingHash;
    private int attackHash;

    private Vector2 axis;
    private Vector3 currentMovement;
    private Vector3 currentRunMovement;
    private Vector3 camForward;
    private Vector3 camRight;

    private bool isMovementPressed;
    private bool isRunPressed;
    private bool isJumPressed;
    private bool isGladePressed;

    [Header("Movement Settings")]
    [SerializeField] private float runSpeed = 3;

    [SerializeField] private float walkSpeed = 1;
    [SerializeField] private float rotationSpeed = 15f;

    [Header("Gravity Settings")]
    [SerializeField] private float groundGravity = .05f;

    [SerializeField] private float fallMultiplier = 2;

    private float gravity = -9.8f;

    [Header("Glade Settings")]
    [SerializeField] private float gladeForce = 4;

    [SerializeField] private float velocityToGlade = 0;

    [Header("Jump Settings")]
    [SerializeField] private float maxJumpHeight = 1.0f;

    [SerializeField] private float maxJumpTime = 0.5f;
    private float initialJumpVelocity;
    private bool isJumping;
    private bool isJumpAnimating;

    private void Awake()
    {
        playerInput = new PlayerInput();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningHash = Animator.StringToHash("isRunning");
        isJumpingHash = Animator.StringToHash("isJumping");
        attackHash = Animator.StringToHash("attack");

        //Player inputs callbacks

        //Movement
        playerInput.CharacterControls.Move.started += OnMovementInput;
        playerInput.CharacterControls.Move.performed += OnMovementInput;
        playerInput.CharacterControls.Move.canceled += OnMovementInput;

        //Run
        playerInput.CharacterControls.Run.started += OnRun;
        playerInput.CharacterControls.Run.canceled += OnRun;

        //Jump
        playerInput.CharacterControls.Jump.started += OnJump;
        playerInput.CharacterControls.Jump.canceled += OnJump;

        //Glade
        playerInput.CharacterControls.Glade.started += OnGlade;
        playerInput.CharacterControls.Glade.canceled += OnGlade;

        //Attack
        playerInput.CharacterControls.Attack.started += OnAttack;
        SetUpJumpvariables();
    }

    private void OnGlade(InputAction.CallbackContext ctx)
    {
        isGladePressed = ctx.ReadValueAsButton();
    }

    private void OnJump(InputAction.CallbackContext ctx)
    {
        isJumPressed = ctx.ReadValueAsButton();
    }

    private void OnMovementInput(InputAction.CallbackContext ctx)
    {
        axis = ctx.ReadValue<Vector2>();

        ////Walk
        //currentMovement.x = axis.x * walkSpeed;
        //currentMovement.z = axis.y * walkSpeed;
        ////Run
        //currentRunMovement.x = axis.x * runSpeed;
        //currentRunMovement.z = axis.y * runSpeed;

        isMovementPressed = currentMovement.x != 0 || currentMovement.z != 0;
    }

    private void OnRun(InputAction.CallbackContext ctx)
    {
        isRunPressed = ctx.ReadValueAsButton();
    }

    private void OnAttack(InputAction.CallbackContext ctx)
    {
        HandleAttack();
    }

    private void Update()
    {
        //Calcular la direccion de la camara
        CamDirection();
        HandleRotation();
        HandleAnimation();
        if (isRunPressed)
        {
            Vector3 dir = (axis.x * camRight + axis.y * camForward) * runSpeed;
            dir.y = currentRunMovement.y;
            isMovementPressed = dir.x != 0 || dir.z != 0;

            characterController.Move(dir * Time.deltaTime);
        }
        else
        {
            Vector3 dir = (axis.x * camRight + axis.y * camForward) * walkSpeed;
            dir.y = currentRunMovement.y;
            isMovementPressed = dir.x != 0 || dir.z != 0;

            characterController.Move(dir * Time.deltaTime);
        }

        HandleGravity();
        HandleJump();
    }

    private void HandleJump()
    {
        if (characterController.isGrounded && !isJumping && isJumPressed)
        {
            animator.SetBool(isJumpingHash, true);
            isJumping = true;
            isJumpAnimating = true;
            currentMovement.y = initialJumpVelocity;
            currentRunMovement.y = initialJumpVelocity;
        }
        else if (characterController.isGrounded && isJumping && !isJumPressed)
        {
            isJumping = false;
        }
    }

    private void HandleRotation()
    {
        Vector3 positionToLookAt = (axis.x * camRight + axis.y * camForward);

        //Setear la rotacion en la cual va a girar

        positionToLookAt.y = 0.0f;

        //Obtenemos la rotacion actual del Player
        Quaternion currentRotation = transform.rotation;

        if (isMovementPressed)
        {
            //Hacemos una interpolaci�n en la rotacion que va a girar cuando el player se mueve
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void HandleGravity()
    {
        bool canGlade = currentMovement.y < velocityToGlade;
        bool isFalling = currentMovement.y <= 0 || !isJumPressed;
        animator.SetBool("isGrounded", characterController.isGrounded);
        if (characterController.isGrounded)
        {
            if (isJumpAnimating)
            {
                animator.SetBool(isJumpingHash, false);
                isJumpAnimating = false;
            }

            currentMovement.y = groundGravity;
            currentRunMovement.y = groundGravity;
        }
        //Glade
        else if (canGlade && isGladePressed && !characterController.isGrounded)
        {
            Debug.Log("Glanding");
            float previousYVelocity = currentMovement.y;
            float newYVelocity = currentMovement.y + (gravity / gladeForce * Time.deltaTime);
            float nextYVelocity = (previousYVelocity + newYVelocity) * .5f;
            animator.SetBool("isGliding", true);

            currentMovement.y = nextYVelocity;
            currentRunMovement.y = nextYVelocity;
        }
        //Falling
        else if (isFalling)
        {
            float previousYVelocity = currentMovement.y;
            float newYVelocity = currentMovement.y + (gravity * fallMultiplier * Time.deltaTime);
            float nextYVelocity = (previousYVelocity + newYVelocity) * .5f;
            currentMovement.y = nextYVelocity;
            currentRunMovement.y = nextYVelocity;
            animator.SetBool("isGliding", false);
        }
        //Normal Gravity
        else
        {
            float previousYVelocity = currentMovement.y;
            float newYVelocity = currentMovement.y + (gravity * Time.deltaTime);
            float nextYVelocity = (previousYVelocity + newYVelocity) * .5f;
            currentMovement.y = nextYVelocity;
            currentRunMovement.y = nextYVelocity;
            animator.SetBool("isGliding", false);
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
        if ((isRunPressed && isRunPressed) && !isRunning)
            animator.SetBool(isRunningHash, true);
        else if ((!isRunPressed || !isRunning) && isRunning)
            animator.SetBool(isRunningHash, false);
    }

    private void CamDirection()
    {
        camForward = Camera.main.transform.forward.normalized;
        camRight = Camera.main.transform.right.normalized;
    }

    private void HandleAttack()
    {
        animator.SetTrigger(attackHash);
    }

    private void SetUpJumpvariables()
    {
        float timeToApex = maxJumpTime / 2;
        gravity = (-2 * maxJumpHeight) / Mathf.Pow(timeToApex, 2);

        initialJumpVelocity = (2 * maxJumpHeight) / timeToApex;
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