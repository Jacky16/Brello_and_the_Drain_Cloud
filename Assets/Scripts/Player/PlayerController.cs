using System.Collections;
using UnityEngine;
using DG.Tweening;
using System;

public class PlayerController : MonoBehaviour
{
    //References/Components
    private BrelloOpenManager brelloOpenManager;
    private Rigidbody rb;
    private Animator animator;
    private PlayerAudio playerAudio;


    //Variables para almacenar los ID's de las animaciones
    private int isGlidingHash;
    private int isGroundedHash;
    private int speedHash;
    private int fallSpeedHash;

    //Vector variables
    private Vector2 axis;
    private Vector3 camForward;
    private Vector3 camRight;
    private Vector3 camDir;

    //Variables bools
    private bool isMovementPressed;
    private bool isStartingToSwim;
    private bool isSwimming;
    private bool isGlading;
    private bool isJumping;

    [Header("Movement Settings")]
    [SerializeField] private float runSpeed = 20;
    [SerializeField] private float walkSpeed = 10;
    [SerializeField] private float acceleration = 1;
    [SerializeField] private float rotationSpeed = 15f;
    public enum MovementMode { ADD_FORCE,VELOCITY}
    [SerializeField] private MovementMode movementMode = MovementMode.VELOCITY;
    
    bool isUmbrellaOpen;
    private float currentSpeed = 0;
    private bool canMove = true;
    
    [Header("Glading Settings")]
    [SerializeField] private float gladingSpeed = 10;
    [SerializeField] private float gladingGravity = 100;
    [SerializeField] private float velocityToGlade = 3;
    public bool canGlide;


    [Header("Ground Checker settings")]
    [SerializeField] Transform posCheckerGround;
    [SerializeField] float radiusCheck = .25f;
    [SerializeField] LayerMask groundLayerMask;
    [SerializeField] Transform wallCheckPos;
    [SerializeField] float radiusCheckWall = .25f;
    bool isGrounded;
    bool isWallForward = false;


    [Header("Jump Settings")]
    [SerializeField] float jumpForce = 10;

    //Swiming variables
    [Header("Swimimg Settings")]
    [SerializeField] float offsetTweenY;
    [SerializeField] float time = 1;
    [SerializeField] GameObject splashParticle;
    Transform pivotSwiming;
    Vector3 currentTorrentDirection;
    private Tween tweenSwiming;

    [Header("Attack Settings")]
    [SerializeField] LayerMask attackLayerMask;
    [SerializeField] Transform pivotAttack;
    [SerializeField] Vector3 sizeCubeAttack;
    [SerializeField] int damage;
    [SerializeField] float forceForward = 1000;
    [SerializeField] float forceUp = 1000;
    [SerializeField] float timeToAttack = .25f;
    float timeToAttackTimer;
    public bool canAttack;


    int noOfClicks = 0;
    const string nameFirstAttack = "Armature_Idle_head";
    const string nameSecondAttack= "Armature_head_patada";
    const string nameThirdAttack = "Armature_spin";

    //Audio variables
    private bool isGlidePlaying = false;

    private void Awake()
    {
        canGlide = true;
        canAttack = true;
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        brelloOpenManager = GetComponent<BrelloOpenManager>();
        playerAudio = GetComponent<PlayerAudio>();

        SetAnimatorsHashes();
    }

    private void Update()
    {
        Checkers();
        SwimingManager();
        MovementManager();
        
        HandleRotation();
        HandleAnimation();
    }
    private void FixedUpdate()
    {
        ForceTorrent();
        Movement();
        GladeManager();
    }

    #region Main movement functions

    private void MovementManager()
    {
        if (isMovementPressed)
        {
            if (canMove)
            {
                if (isGlading)
                    currentSpeed = Mathf.Lerp(currentSpeed, gladingSpeed, acceleration * Time.deltaTime);

                 else if (isUmbrellaOpen)
                    currentSpeed = Mathf.Lerp(currentSpeed, walkSpeed, acceleration * Time.deltaTime);

                 else
                   currentSpeed = Mathf.Lerp(currentSpeed, runSpeed, acceleration * Time.deltaTime);
            }
        }
        else
            currentSpeed = Mathf.Lerp(currentSpeed, 0, acceleration * Time.deltaTime);

    }
    private void Movement()
    {
        Vector3 dir;
        dir = CamDirection() * currentSpeed;
        if (canMove)
        {           
            if (movementMode == MovementMode.VELOCITY)
            {
                //Comprueba si esta chocando con el muero para cancelar la velocidad del 
                if (isWallForward)
                    dir.z = rb.velocity.z;

                dir.y = rb.velocity.y;
                rb.velocity = dir;
            }
            else
            {
                dir.y = 0;
                rb.AddForce(dir, ForceMode.Acceleration);
            }               
        }
        else
            currentSpeed = 0;
    }
    void ForceTorrent()
    {
        if (isSwimming && canMove)
        {
            currentTorrentDirection.y = 0;
            rb.AddForce(currentTorrentDirection, ForceMode.Acceleration);
        }
    }
    public void HandleJump()
    {  
        if((isGrounded || isSwimming) && canMove && !isUmbrellaOpen)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            isJumping = true;
            rb.AddForce(Vector3.up * jumpForce * 10, ForceMode.Impulse);
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
        isGrounded = Physics.CheckSphere(posCheckerGround.position, radiusCheck, groundLayerMask) && !isSwimming;
        isGlading = !isSwimming && isUmbrellaOpen && !isGrounded;
        isWallForward = Physics.CheckSphere(wallCheckPos.position, radiusCheckWall, groundLayerMask);
        if (isGrounded)
        {         
            isJumping = false;
        }
        
    }
    private void GladeManager()
    {
        if (isGlading && !isSwimming)
        {
            rb.AddForce(Vector3.down * gladingGravity, ForceMode.Force);
            
        }  
    }
    private Vector3 CamDirection()
    {
        camForward = Camera.main.transform.forward.normalized;
        camRight = Camera.main.transform.right.normalized;
        camDir = (axis.x * camRight + axis.y * camForward);
        camDir.y = 0;
        return camDir.normalized;
    }
    public void BlockMovement()
    {
        canMove = false;
    }
    public void EnableMovement()
    {
        canMove = true;
    }

    public void ChangeTypeofMovement(MovementMode _movementMode,                                   
                                    bool _resetToDefault = false,
                                    float timeToDefault = .5f)
    {
        movementMode = _movementMode;
        if(_resetToDefault)
            StartCoroutine(ChangeTypeofMovementCoroutine(timeToDefault));
    }

    IEnumerator ChangeTypeofMovementCoroutine(float timeToDefault)
    {
        yield return new WaitForSeconds(timeToDefault);
        movementMode = MovementMode.VELOCITY;
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
        if (canAttack && canMove && Time.time > timeToAttack)
        {
            timeToAttackTimer = Time.time + timeToAttack;
            noOfClicks++;
        }

        if (noOfClicks == 1)
            animator.SetInteger("currentAttack", 1);
    }
    
    bool CheckState(string nameState)
    {        
        return animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == nameState;
    }

    void CheckCombo()
    {
        canAttack = false;
       
        if (CheckState(nameFirstAttack) && noOfClicks <= 1)
        {
            animator.SetInteger("currentAttack", 0);
            canAttack = true;
            movementMode = MovementMode.VELOCITY;
            noOfClicks = 0;
        }
        //Ataque 2
        if (CheckState(nameFirstAttack) && noOfClicks >= 2)
        {
            animator.SetInteger("currentAttack", 2);
            canAttack = true;
        }
        if(CheckState(nameSecondAttack) && noOfClicks <= 2)
        {
            animator.SetInteger("currentAttack", 0);
            canAttack = true;
            noOfClicks = 0;
            movementMode = MovementMode.VELOCITY;
        }
        if(CheckState(nameSecondAttack) && noOfClicks >= 3) {
            movementMode = MovementMode.VELOCITY;
            animator.SetInteger("currentAttack", 3);
            canAttack = true;
        }
        //Ataque 3
        if(CheckState(nameThirdAttack) && noOfClicks >= 3)
        {
            animator.SetInteger("currentAttack", 0);
            canAttack = true;
            noOfClicks = 0;
            movementMode = MovementMode.VELOCITY;
        }
    }

    //Se ejecuta en los eventos de animacion
    void AddForceForward()
    {
        movementMode = MovementMode.ADD_FORCE;
        rb.AddForce(CamDirection() * forceForward, ForceMode.Force);
        rb.AddForce(Vector3.up * forceUp, ForceMode.Force);
    }
    //Se ejecuta en los eventos de animacion
    private void Attack()
    {
        Collider[] colliders = Physics.OverlapBox(pivotAttack.position, sizeCubeAttack, Quaternion.identity, attackLayerMask);
        foreach (Collider collider in colliders)
        {
            if (collider.TryGetComponent(out Health _health))
            {
                if (!collider.CompareTag("Player") && !collider.CompareTag("Pyra"))
                {
                    _health.DoDamage(damage);
                }

                print(collider.tag);
            }
        }
    }

    #endregion Attack functions

    #region Swiming functions

    private void OnSwiming(Collider other)
    {
        if (other.CompareTag("Water") && !isSwimming)
        {         
            currentTorrentDirection = other.GetComponent<WaterTorrent>().GetTorrentDir();
   
            rb.useGravity = false;

            isJumping = false;
            isSwimming = true;
            isGlading = false;
            isStartingToSwim = true;

            movementMode = MovementMode.ADD_FORCE;

            animator.SetBool("isSwiming", true);
            
            if(splashParticle)
                Instantiate(splashParticle, transform.position, splashParticle.transform.rotation);

            rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;

            pivotSwiming = other.transform.GetChild(0).transform;
            tweenSwiming = transform.DOLocalMoveY(pivotSwiming.position.y, 2).SetEase(Ease.OutElastic).OnComplete(() =>
            {
                isStartingToSwim = false;
            });

            AkSoundEngine.PostEvent("WaterSplash_Brello", WwiseManager.instance.gameObject);
        }
    }

    private void OnOutSwiming(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            currentTorrentDirection = Vector3.zero;

            rb.useGravity = true;

            isSwimming = false;

            animator.SetBool("isSwiming", false);

            brelloOpenManager.SetOpen(false);

            movementMode = MovementMode.VELOCITY;
      
            //Matamos a la animacion por si se sale antes, que no se quede flotando
            tweenSwiming.Kill();
        }
    }

    private void SwimingManager()
    {
        if (isSwimming)
        {
            rb.useGravity = false;
            brelloOpenManager.SetOpen(true);
           
            //Aplicar anclaje en el pivote del agua
            if(!isJumping)
                rb.position = new Vector3(rb.position.x, pivotSwiming.position.y, rb.position.z);
        }
    }

    public void HandleSwimingJump()
    {
        if (isSwimming && !WaterPlatformManager.singletone.IsPyraInPlatform())
        {
            tweenSwiming.Kill();

            rb.constraints = RigidbodyConstraints.FreezeRotation;
            rb.useGravity = true;
            HandleJump();
        }
    }
    #endregion Swiming functions

    public void OpenUmbrellaManager(bool _value)
    {
        //Parar el impulso de cuando planeas
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        
        if (canGlide && canMove)
        {
            isUmbrellaOpen = _value;
            brelloOpenManager.SetOpen(isUmbrellaOpen);

            
            rb.useGravity = !_value;
            
            if(!_value)
                movementMode = MovementMode.VELOCITY;
            else
                movementMode = MovementMode.ADD_FORCE;

            //Audio de apertura de paraguas
            if (_value && !isSwimming)
            {
                playerAudio.PlayOpen();
            }
            else if (!_value && !isSwimming)
            {
                playerAudio.PlayClose();
                isGlidePlaying = false;
                playerAudio.StopGlide();
            }
            else if (isGlidePlaying && isSwimming)
            {
                isGlidePlaying = false;
                playerAudio.StopGlide();
            }
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
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (isGrounded) Gizmos.color = Color.green;
        Gizmos.DrawSphere(posCheckerGround.position, radiusCheck);

        Gizmos.DrawWireCube(pivotAttack.position,sizeCubeAttack);

        //Draw Sphere wall check
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(wallCheckPos.position, radiusCheckWall);
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

    public bool IsStartingToSwim()
    {
        return isStartingToSwim;
    }

    public bool IsMoving()
    {
        return isMovementPressed;
    }
    public bool IsGrounded()
    {
        return isGrounded;
    }
    public bool IsGlading()
    {
        return isGlading;
    }
    #endregion Getters

    #region Init functions

    private void SetAnimatorsHashes()
    {
        isGlidingHash = Animator.StringToHash("isGliding");
        isGroundedHash = Animator.StringToHash("isGrounded");
        speedHash = Animator.StringToHash("speed");
        fallSpeedHash = Animator.StringToHash("fallSpeed");
    }

    #endregion Init functions

    #region Audio



    #endregion
}