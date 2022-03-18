using System.Collections;
using UnityEngine;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{
    //References/Components

    private BrelloOpenManager brelloOpenManager;
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
    private bool canMove = true;

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

    //Attack variables
    [Header("Attack Settings")]
    [SerializeField] private Transform pivotAttack;

    [SerializeField] private int damage = 1;
    [SerializeField] private Vector3 sizeCubeAttack;
    private float timeBtwAttacks = 0.25f;
    private int currentAttack = 0;
    private int numAttacks = 2;

    //Swiming variables

    private Tween swimingTwee;

    //Air movement variables

    private bool isAirMoving;
    private Tween tweenAirMovement;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        brelloOpenManager = GetComponent<BrelloOpenManager>();

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
        AirMovementManager();
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
        if (canMove)
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
        if (canMove)
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

            brelloOpenManager.SetOpen(true);
        }
        //Falling
        else if (isFalling && !characterController.isGrounded && !isSwimming)
        {
            float previousYVelocity = currentGravity.y;
            float newYVelocity = currentGravity.y + (gravity * fallMultiplier * Time.deltaTime);
            float nextYVelocity = (previousYVelocity + newYVelocity) * .5f;
            currentGravity.y = nextYVelocity;
        }
        //Normal Gravity
        else
        {
            if (isSwimming) return;
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

    public void BlockMovement()
    {
        canMove = false;
    }

    public void EnableMovement()
    {
        canMove = true;
    }

    #endregion Main movement functions

    #region Dash functions

    public void HandleDash()
    {
        if (!isSwimming)
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

    #endregion Dash functions

    #region Attack functions

    public void HandleAttack()
    {
        animator.SetTrigger(attackHash);

        DoAttackAnimation();
    }

    private void DoAttackAnimation()
    {
        if (currentAttack == numAttacks)
            currentAttack = 0;

        animator.SetInteger(numAttackHash, currentAttack);
        currentAttack++;
    }

    private void Attack()
    {
        Collider[] colliders = Physics.OverlapBox(pivotAttack.position, sizeCubeAttack, Quaternion.identity);
        foreach (Collider collider in colliders)
        {
            if (collider.TryGetComponent(out Health _health))
            {
                if (!collider.CompareTag("Player"))
                {
                    _health.DoDamage(damage);
                }
            }
        }
    }

    #endregion Attack functions

    #region Swiming functions

    private void OnSwiming(Collider other)
    {
        if (other.CompareTag("Water") && !isSwimming)
        {
            isSwimming = true;

            animator.SetBool("isSwiming", true);

            Transform pivotWater = other.transform.GetChild(0).transform;
            swimingTwee = transform.DOMoveY(pivotWater.position.y, 2).SetEase(Ease.OutElastic);
        }
    }

    private void OnOutSwiming(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            isSwimming = false;

            animator.SetBool("isSwiming", false);

            brelloOpenManager.SetOpen(false);

            //Matamos a la animacion por si se sale antes, que no se quede flotando
            swimingTwee.Kill();
        }
    }

    private void SwimingManager()
    {
        if (isSwimming)
        {
            currentGravity.y = 0;
            brelloOpenManager.SetOpen(true);
        }
    }

    #endregion Swiming functions

    public void OpenUmbrellaManager(bool _value)
    {
        isGladePressed = _value;
        brelloOpenManager.SetOpen(isGladePressed);
    }

    #region Air Movement Functions

    //Funcion que se ejecuta en el update, y bloquea la gravedad a 0
    private void AirMovementManager()
    {
        if (isAirMoving)
            currentGravity.y = 0;
    }

    //Funcion que se llama cuando entra en contacto con la zona de aire
    private void OnAirMovement(Collider other)
    {
        if (other.CompareTag("Air") && !isAirMoving)
        {
            isAirMoving = true;
            Transform pivotAir = other.transform.GetChild(0).transform;

            tweenAirMovement = transform.DOMoveY(pivotAir.position.y, 1).SetEase(Ease.Linear);
        }
    }

    //Funcion que se llama cuando se va de la zona de aire
    private void OutMovementAir(Collider other)
    {
        if (other.CompareTag("Air"))
        {
            isAirMoving = false;
            tweenAirMovement.Kill();
        }
    }

    #endregion Air Movement Functions

    private void OnTriggerEnter(Collider other)
    {
        OnSwiming(other);
        OnAirMovement(other);
    }

    private void OnTriggerExit(Collider other)
    {
        OnOutSwiming(other);
        OutMovementAir(other);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(pivotAttack.position, sizeCubeAttack);
    }

    #region Inputs setters

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

    #region Getters

    public bool IsSwimming()
    {
        return isSwimming;
    }

    #endregion Getters

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

    #endregion Init functions
}