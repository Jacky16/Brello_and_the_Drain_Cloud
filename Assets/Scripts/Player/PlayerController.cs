using System.Collections;
using UnityEngine;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{
    //References/Components

    private BrelloOpenManager brelloOpenManager;
    private Rigidbody rb;
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
    private Vector3 moveDirection;

    private bool isMovementPressed;
    private bool isJumPressed;
    private bool isSwimming;
    private bool canGlade;
    private bool isGlading;

    [Header("Movement Settings")]
    [SerializeField] private float runSpeed = 20;
    [SerializeField] private float walkSpeed = 10;
    [SerializeField] private float acceleration = 1;
    [SerializeField] private float rotationSpeed = 15f;
    bool isUmbrellaOpen;
    private float currentSpeed = 0;
    private bool canMove = true;

    [Header("Ground Checker settings")]
    [SerializeField] Transform posCheckerGround;
    [SerializeField] float radiusCheck = .25f;
    [SerializeField] LayerMask groundLayerMask;
    bool isGrounded;

    [Header("Jump Settings")]
    [SerializeField] float jumpForce = 10;

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
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        brelloOpenManager = GetComponent<BrelloOpenManager>();
        playerAudio = GetComponent<PlayerAudio>();

        SetAnimatorsHashes();
    }

    private void Update()
    {
        Checkers();

        MovementManager();
        
        HandleRotation();
        HandleAnimation();
    }
    private void FixedUpdate()
    {
        Movement();
        GladeManager();
    }

    #region Main movement functions

    private void MovementManager()
    {
        if (isMovementPressed)
        {
            if(isUmbrellaOpen)
                currentSpeed = Mathf.Lerp(currentSpeed, walkSpeed, acceleration * Time.deltaTime);
            else
            currentSpeed = Mathf.Lerp(currentSpeed, runSpeed, acceleration * Time.deltaTime);
        }
        else
        {
            currentSpeed = Mathf.Lerp(currentSpeed, 0, acceleration * Time.deltaTime);
        }
        if (isGrounded)
            rb.useGravity = false;
        else
            rb.useGravity = true;
    }
    private void Movement()
    {
        Vector3 dir = CamDirection() * currentSpeed;
        dir.y = rb.velocity.y;
        rb.velocity = dir;       
        
    }
    public void HandleJump()
    {
        if (isGrounded)
            rb.AddForce(Vector3.up * jumpForce * 10, ForceMode.Impulse);
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
                rb.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
           
        }
    }
    private void HandleAnimation()
    {
        animator.SetFloat(fallSpeedHash, rb.velocity.y);
        animator.SetBool(isGroundedHash, isGrounded);
        animator.SetFloat(speedHash, currentSpeed);
        animator.SetBool(isGlidingHash, isGlading);
            
    }
    private void Checkers()
    {

        isGrounded = Physics.CheckSphere(posCheckerGround.position, radiusCheck, groundLayerMask);

        isGlading = !isGrounded && isUmbrellaOpen;
        
    }

    private void GladeManager()
    {
        if (isGlading)
        {
            rb.useGravity = false;
            rb.AddForce(Vector3.down * -Physics.gravity.y / 2, ForceMode.Force);
        }
        else
        {
            rb.useGravity = true;
        }
    }

    private Vector3 CamDirection()
    {
        camForward = Camera.main.transform.forward.normalized;
        camRight = Camera.main.transform.right.normalized;
        camDir = (axis.x * camRight + axis.y * camForward);
        camDir.y = 0;
        return camDir;
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
        
    }

    #endregion Dash functions

    #region Attack functions

    public void HandleAttack()
    {
        
    }

    private void DoAttackAnimation()
    {
        animator.SetTrigger(attackHash); 
    }

    private void Attack()
    {
        //Collider[] colliders = Physics.OverlapBox(pivotAttack.position, sizeCubeAttack, Quaternion.identity);
        //foreach (Collider collider in colliders)
        //{
        //    if (collider.TryGetComponent(out Health _health))
        //    {
        //        if (!collider.CompareTag("Player") && !collider.CompareTag("Pyra"))
        //        {
        //            _health.DoDamage(damage);
        //        }
        //    }
        //}
    }

    #endregion Attack functions

    #region Swiming functions

    private void OnSwiming(Collider other)
    {
        if (other.CompareTag("Water") && !isSwimming)
        {
            isSwimming = true;
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
        if (isSwimming && isJumPressed)
        {
            tweenSwiming.Kill();
        
            if (!WaterPlatformManager.singletone.IsPyraInPlatform())
            {
                HandleJump();
            }
        }
        else if (isSwimming)
        {
            currentGravity.y = 0;
            brelloOpenManager.SetOpen(true);
        }
    }

    #endregion Swiming functions

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
    }

    private void OnTriggerExit(Collider other)
    {
        OnOutSwiming(other);

    }

    private void OnDrawGizmosSelected()
    {
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (isGrounded) Gizmos.color = Color.green;
        Gizmos.DrawSphere(posCheckerGround.position, radiusCheck);

    }

    #region Inputs setters

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