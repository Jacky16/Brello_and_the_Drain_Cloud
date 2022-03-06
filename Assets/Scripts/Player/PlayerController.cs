using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterController characterController;
    private Animator animator;

    //Variables para almacenar los ID's de las animaciones

    private int isWalkingHash;
    private int isRunningHash;
    private int isJumpingHash;
    private int attackHash;
    private int isGlidingHash;
    private int isGroundedHash;

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
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningHash = Animator.StringToHash("isRunning");
        isJumpingHash = Animator.StringToHash("isJumping");
        isGlidingHash = Animator.StringToHash("isGliding");
        isGroundedHash = Animator.StringToHash("isGrounded");

        attackHash = Animator.StringToHash("attack");

        SetUpJumpvariables();
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

        //Grounded animator
        animator.SetBool(isGroundedHash, characterController.isGrounded);

        //Glading animator
        animator.SetBool(isGlidingHash, canGlade && isGladePressed && !characterController.isGrounded);

        //Grounded
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
        //Glading
        else if (canGlade && isGladePressed && !characterController.isGrounded)
        {
            float previousYVelocity = currentMovement.y;
            float newYVelocity = currentMovement.y + (gravity / gladeForce * Time.deltaTime);
            float nextYVelocity = (previousYVelocity + newYVelocity) * .5f;

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
        }
        //Normal Gravity
        else
        {
            float previousYVelocity = currentMovement.y;
            float newYVelocity = currentMovement.y + (gravity * Time.deltaTime);
            float nextYVelocity = (previousYVelocity + newYVelocity) * .5f;

            currentMovement.y = nextYVelocity;
            currentRunMovement.y = nextYVelocity;
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

    public void HandleAttack()
    {
        animator.SetTrigger(attackHash);
    }

    private void SetUpJumpvariables()
    {
        float timeToApex = maxJumpTime / 2;

        gravity = (-2 * maxJumpHeight) / Mathf.Pow(timeToApex, 2);
        initialJumpVelocity = (2 * maxJumpHeight) / timeToApex;
    }

    #region Inputs setters

    public void SetGladePressed(bool _value)
    {
        isGladePressed = _value;
    }

    public void SetJumPressed(bool _value)
    {
        isJumPressed = _value;
    }

    public void SetAxis(Vector2 _value)
    {
        axis = _value;
    }

    public void SetMovementPressed(bool _value)
    {
        isMovementPressed = _value;
    }

    public void SetRunPressed(bool _value)
    {
        isRunPressed = _value;
    }

    #endregion Inputs setters
}