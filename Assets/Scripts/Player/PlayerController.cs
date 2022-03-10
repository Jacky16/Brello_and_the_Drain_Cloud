using System.Collections;
using UnityEngine;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{
    private CharacterController characterController;
    private Animator animator;

    //Variables para almacenar los ID's de las animaciones

    private int isJumpingHash;
    private int attackHash;
    private int isGlidingHash;
    private int isGroundedHash;
    private int speedHash;
    private int numAttackHash;

    private Vector2 axis;
    private Vector3 currentGravity;
    private Vector3 currentRunMovement;
    private Vector3 camForward;
    private Vector3 camRight;
    private Vector3 camDir;

    private bool isMovementPressed;
    private bool isJumPressed;
    private bool isGladePressed;
    private bool isSwimming;

    [Header("Movement Settings")]
    [SerializeField] private float speed = 3;

    [SerializeField] private float dashSpeed = 5;
    [SerializeField] private float acceleration = 1;
    [SerializeField] private float rotationSpeed = 15f;
    private float currentSpeed = 0;
    private float dashTime = 0.25f;

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

    //Attack
    private float timeBtwAttacks = 0.25f;

    private int currentAttack = 0;
    private int numAttacks = 3;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        SetAnimatorsHashes();
        SetUpJumpvariables();
    }

    private void Update()
    {
        CamDirection();
        HandleRotation();
        HandleAnimation();

        Movement();
        HandleGravity();
        HandleJump();
        SwimingManager();
    }


    #region Main movement functions
    private void Movement()
    {
        //Acceleration
        if (isMovementPressed)
            currentSpeed = Mathf.Lerp(currentSpeed, speed, acceleration * Time.deltaTime);
        else
            currentSpeed = Mathf.Lerp(currentSpeed, 0, acceleration * Time.deltaTime);

        Vector3 currDir = camDir * currentSpeed;

        currDir.y = currentGravity.y;

        characterController.Move(currDir * Time.deltaTime);
    }

    private void HandleJump()
    {
        if (characterController.isGrounded && !isJumping && isJumPressed)
        {
            animator.SetBool(isJumpingHash, true);
            isJumping = true;
            isJumpAnimating = true;
            currentGravity.y = initialJumpVelocity;
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
        bool canGlade = currentGravity.y < velocityToGlade;
        bool isFalling = currentGravity.y < 0 || !isJumPressed && !characterController.isGrounded;

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

            currentGravity.y = groundGravity;
        }
        //Glading
        else if (canGlade && isGladePressed && !characterController.isGrounded)
        {
            float previousYVelocity = currentGravity.y;
            float newYVelocity = currentGravity.y + (gravity / gladeForce * Time.deltaTime);
            float nextYVelocity = (previousYVelocity + newYVelocity) * .5f;

            currentGravity.y = nextYVelocity;
        }
        //Falling
        else if (isFalling)
        {
            float previousYVelocity = currentGravity.y;
            float newYVelocity = currentGravity.y + (gravity * fallMultiplier * Time.deltaTime);
            float nextYVelocity = (previousYVelocity + newYVelocity) * .5f;

            currentGravity.y = nextYVelocity;
        }
        //Normal Gravity
        else
        {
            float previousYVelocity = currentGravity.y;
            float newYVelocity = currentGravity.y + (gravity * Time.deltaTime);
            float nextYVelocity = (previousYVelocity + newYVelocity) * .5f;

            currentGravity.y = nextYVelocity;
        }
    }

    private void HandleAnimation()
    {
        animator.SetFloat(speedHash, currentSpeed);
    }

    private void CamDirection()
    {
        camForward = Camera.main.transform.forward.normalized;
        camRight = Camera.main.transform.right.normalized;
        camDir = (axis.x * camRight + axis.y * camForward);
    }

    #endregion


    #region Dash functions
    public void HandleDash()
    {
        StartCoroutine(Dash());
    }

    private IEnumerator Dash()
    {
        float startTime = Time.time;
        while (Time.time < startTime + dashTime)
        {
            characterController.Move(camDir * dashSpeed * Time.deltaTime);

            yield return null;
        }
    }

    #endregion

    #region Attack functions
    public void HandleAttack()
    {
        animator.SetTrigger(attackHash);

        DoAttack();
    }
    private void DoAttack()
    {
        if (currentAttack == numAttacks)
            currentAttack = 0;

        animator.SetInteger(numAttackHash, currentAttack);
        currentAttack++;
    }
    #endregion

    #region Swiming functions
    private void OnSwiming(Collider other)
    {
        if (other.CompareTag("Water") && !isSwimming)
        {
            isSwimming = true;
            animator.SetBool("isSwiming", true);
            Transform pivotWater = other.transform.GetChild(0).transform;
            transform.DOMoveY(pivotWater.position.y, 2).SetEase(Ease.OutElastic);
        }
    }
    private void OutSwiming(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            Debug.Log("Fuera del agua");
            animator.SetBool("isSwiming", false);
            isSwimming = false;
        }
    }
    private void SwimingManager()
    {
        if (isSwimming)
            currentGravity.y = 0;
    }
    #endregion

    private void OnTriggerEnter(Collider other)
    {
        OnSwiming(other);
    }

    private void OnTriggerExit(Collider other)
    {
        OutSwiming(other);
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

    #endregion Inputs setters

    #region Init functions
    private void SetUpJumpvariables()
    {
        float timeToApex = maxJumpTime / 2;

        gravity = (-2 * maxJumpHeight) / Mathf.Pow(timeToApex, 2);
        initialJumpVelocity = (2 * maxJumpHeight) / timeToApex;
    }

    private void SetAnimatorsHashes()
    {
        isJumpingHash = Animator.StringToHash("isJumping");
        isGlidingHash = Animator.StringToHash("isGliding");
        isGroundedHash = Animator.StringToHash("isGrounded");
        speedHash = Animator.StringToHash("speed");
        attackHash = Animator.StringToHash("attack");
        numAttackHash = Animator.StringToHash("numAttack");
    }
    #endregion
}