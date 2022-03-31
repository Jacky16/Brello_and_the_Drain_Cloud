using System.Collections;
using UnityEngine;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{
    //References/Components

    private BrelloOpenManager brelloOpenManager;
    private CharacterController characterController;
    private Animator animator;

    private PlayerAudio playerAudio;

    //Variables para almacenar los ID's de las animaciones

    private int isJumpingHash;
    private int attackHash;
    private int isGlidingHash;
    private int isGroundedHash;
    private int speedHash;
    private int numAttackHash;
    private int fallSpeedHash;

    private Vector2 axis;
    private Vector3 currentGravity;
    private Vector3 camForward;
    private Vector3 camRight;
    private Vector3 camDir;

    private bool isMovementPressed;
    private bool isJumPressed;
    private bool isSwimming;

    [Header("Speed Settings")]
    [Header("Movement Settings")]
    [SerializeField] private float runSpeed = 10;
    [SerializeField] private float walkSpeed = 10;
    [SerializeField] private float gladingSpeed = 20;
    [SerializeField] private float dashSpeed = 5;
    [Space]
    [SerializeField] private float acceleration = 1;
    [SerializeField] private float rotationSpeed = 15f;
    bool isUmbrellaOpen;
    private float currentSpeed = 0;
    private float dashTime = 0.25f;
    private bool canMove = true;

    [Header("Gravity Settings")]
    [SerializeField] private float groundGravity = .05f;

    [SerializeField] private float fallMultiplier = 2;

    private float gravity = -9.8f;
    
    [Header("Glade Settings")]
    [SerializeField] private float divideGravityGlade = 4; 

    [SerializeField] private float velocityToGlade = 0;

    private bool canGlade;

    [Header("Jump Settings")]
    [SerializeField] private float maxJumpHeight = 1.0f;

    [SerializeField] private float maxJumpTime = 0.5f;
    private float initialJumpVelocity;
    private bool isJumping;
    private bool isJumpAnimating;

    //Attack variables
    [Header("Attack Settings")]

    [SerializeField] private int damage = 1;
    [SerializeField] private float timeBtwAttacks = 0.25f;
    [SerializeField] float timeResetComboAttack = 1;
    [SerializeField] private Vector3 sizeCubeAttack;
    [SerializeField] private Transform pivotAttack;
    private int currentAttack = 0;
    private int numAttacks = 2;
    float nextAttack = 0;
    float nextResetAttack = 0;

    //Swiming variables
    [Header("Tween Settings")]
    [SerializeField] float offsetTweenY;
    [SerializeField] float time = 1;
    private Tween tweenSwiming;
    [SerializeField] GameObject splashParticle;
 

    //Air movement variables

    private bool isAirMoving;
    private Tween tweenAirMovement;


    //Audio variables

    private bool isGlidePlaying = false;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        brelloOpenManager = GetComponent<BrelloOpenManager>();

        playerAudio = GetComponent<PlayerAudio>();

        SetAnimatorsHashes();
        SetUpJumpvariables();
    }

    private void Update()
    {
        CamDirection();
        HandleRotation();
        HandleAnimation();

        MovementManager();
        HandleGravity();
        HandleJump();
        SwimingManager();
        AirMovementManager();
    }

    #region Main movement functions

    private void MovementManager()
    {
        Movement();

        Vector3 currDir = CanMoveManager();

        characterController.Move(currDir * Time.deltaTime);
    }

    private Vector3 CanMoveManager()
    {
        Vector3 currDir = Vector3.zero;
        bool canGlade = currentGravity.y < velocityToGlade;

        if (canMove)
        {
            currDir = camDir * currentSpeed;
        }
        else
        {
            currDir.x = 0;
            currDir.z = 0;
        }
        currDir.y = currentGravity.y;
        if (isUmbrellaOpen && canGlade)
        {
            //currDir.y = currentGravity.y / divideGravityGlade;
        }
        return currDir;
    }

    private void Movement()
    {
        //Acceleration
        if (isMovementPressed)
        {
            //Paraguas abierto
            if (isUmbrellaOpen && currentGravity.y > 0)
            {
                currentSpeed = Mathf.Lerp(currentSpeed, walkSpeed, acceleration * Time.deltaTime);
            }
            //Planeando
            else if (isUmbrellaOpen && currentGravity.y < velocityToGlade)
            {
                currentSpeed = Mathf.Lerp(currentSpeed, gladingSpeed, acceleration * Time.deltaTime);
            }
            //Correr
            else if(!isUmbrellaOpen)
            {
                currentSpeed = Mathf.Lerp(currentSpeed, runSpeed, acceleration * Time.deltaTime);
            }

        }
        else
            currentSpeed = Mathf.Lerp(currentSpeed, 0, acceleration * Time.deltaTime);
    }

    private void HandleJump()
    {
        if (characterController.isGrounded && !isJumping && isJumPressed)
        {
            playerAudio.PlayFootstep();
            Jump();
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
        canGlade = currentGravity.y < velocityToGlade;
        bool isFalling = currentGravity.y < 0 || !isJumPressed && !characterController.isGrounded;

        //Fall speed animator
        animator.SetFloat(fallSpeedHash, currentGravity.y);

        //Grounded animator
        animator.SetBool(isGroundedHash, characterController.isGrounded);

        //Glading animator
        animator.SetBool(isGlidingHash, isFalling && isUmbrellaOpen);

        //Grounded
        if (characterController.isGrounded)
        {
            if (isJumpAnimating)
            {
                animator.SetBool(isJumpingHash, false);
                isJumpAnimating = false;
            }
            currentGravity.y = groundGravity;

            if (isGlidePlaying)
            {
                playerAudio.StopGlide();
                isGlidePlaying = false;
            }
        }
        //Glading
        else if (canGlade && isUmbrellaOpen && !characterController.isGrounded)
        {
            float previousYVelocity = currentGravity.y;
            float newYVelocity = currentGravity.y + (divideGravityGlade * Time.deltaTime);
            float nextYVelocity = (previousYVelocity + newYVelocity) * .5f;
            currentGravity.y = nextYVelocity;

            brelloOpenManager.SetOpen(true);

            if (!isGlidePlaying)
            {
                playerAudio.PlayStartGlide();
                isGlidePlaying = true;
            }
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
        camDir.y = 0;
    }

    public void BlockMovement()
    {
        canMove = false;
    }

    public void EnableMovement()
    {
        canMove = true;
    }

    private void Jump()
    {
        if (!canMove) return;
        animator.SetBool(isJumpingHash, true);

        isSwimming = false;
        isJumping = true;
        isJumpAnimating = true;

        currentGravity.y = initialJumpVelocity;
    }

    #endregion Main movement functions

    #region Dash functions

    public void HandleDash()
    {
        bool canGlade = currentGravity.y < velocityToGlade;
        if (!isSwimming && !isUmbrellaOpen)
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
        if (Time.time >= nextAttack && !isSwimming)
        {
            nextAttack = Time.time + timeBtwAttacks;
            DoAttackAnimation();

            playerAudio.PlayAttack();
        }
    }

    private void DoAttackAnimation()
    {
        animator.SetTrigger(attackHash);

        if (Time.time >= nextResetAttack)
        {
           
            nextResetAttack = Time.time + timeResetComboAttack;
            currentAttack = 0;
        }
        if (currentAttack == numAttacks)
        {
            currentAttack = 0;
        }

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
                if (!collider.CompareTag("Player") && !collider.CompareTag("Pyra"))
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
            isJumping = false;
            isJumPressed = false;
            animator.SetBool("isSwiming", true);

            Instantiate(splashParticle, transform.position, splashParticle.transform.rotation);

            Transform pivotWater = other.transform.GetChild(0).transform;
            tweenSwiming = transform.DOLocalMoveY(pivotWater.position.y, 2).SetEase(Ease.OutElastic);

            AkSoundEngine.PostEvent("WaterSplash_Brello", WwiseManager.instance.gameObject);
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
            tweenSwiming.Kill();
        }
    }

    private void SwimingManager()
    {
        if (isSwimming && isJumPressed && !isJumping)
        {
            tweenSwiming.Kill();
        
            if (!WaterPlatformManager.singletone.IsPyraInPlatform() && !characterController.isGrounded)
            {
                Jump();
            }
        }
        else if (isSwimming)
        {
            currentGravity.y = 0;
            brelloOpenManager.SetOpen(true);
        }
    }

    #endregion Swiming functions

    #region Air Movement Functions

    //Funcion que se ejecuta en el update, y bloquea la gravedad a 0
    private void AirMovementManager()
    {
        if (isAirMoving)
        {
            currentGravity.y = 0;

        }
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

    public void OpenUmbrellaManager(bool _value)
    {
        isUmbrellaOpen = _value;
        brelloOpenManager.SetOpen(isUmbrellaOpen);

        //Audio de apertura de paraguas
        if (_value)
        {
            playerAudio.PlayOpen();

        }
        else
        {
            playerAudio.PlayClose();
            isGlidePlaying = false;
            playerAudio.StopGlide();
        }
        
    }

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

    public bool IsMoving()
    {
        return isMovementPressed;
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
        fallSpeedHash = Animator.StringToHash("fallSpeed");
    }

    #endregion Init functions

    #region Audio



    #endregion
}