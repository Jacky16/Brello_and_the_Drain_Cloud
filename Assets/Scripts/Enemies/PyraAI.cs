using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
public sealed class PyraAI : MonoBehaviour
{
    //Variables de Pyra.
    public NavMeshAgent agent;

    private PlayerController player;
    private Rigidbody rb;
    private PyraProtection pyraProtection;
    private PyraHealth pyraHealth;
    private Animator animator;
    CombatManager combatManager;

    //Variables de detección de objetos interactuables.
    public bool canChasePlayer = true;

    public bool isMovingToInteractuable;
    public bool isInteracting;

    [Header("Radio en el que detectará objetos interactuables.")]
    [SerializeField] private float detectionRadius;

    [Header("Layer(s) con las que interesa que interactue Pyra.")]
    [SerializeField] private LayerMask interactable;

    [Header("Layer de lluvia para comprobar los raycast.")]
    [SerializeField] private LayerMask rainMask;

    [SerializeField] private LayerMask whatIsGround;

    //Variables de objetos detectados.
    [SerializeField] private List<Interactable> detectedObjects;

    public Interactable currentInteractuable;

    //Variables de movimiento.
    public bool moveToPlatform = false, isInPlatform = false, isJumping = false;
    private bool playerIsHighEnough, pyraIsGliding, canArriveToBrello;
    [SerializeField] GameObject tpStartParticles;
    [SerializeField] GameObject tpFinishParticles;
    GameObject currentParticle;

    [Header("Jump settings")]
    [SerializeField] private float jumpPower;

    [SerializeField] private float minDistToTP;
    [SerializeField] float walkSpeed;
    [SerializeField] float runSpeed;
    [SerializeField] private float jumpDuration;
    [SerializeField] private float distanceToJump;
    [SerializeField] private Transform platform;
    [SerializeField] private Transform platformParent;

    private Vector3 posToJump;
    bool stayUnderBrello;

    private void Start()
    {
        combatManager = GameObject.FindGameObjectWithTag("Player").GetComponent<CombatManager>();
        pyraProtection = GameObject.FindGameObjectWithTag("Player").GetComponent<PyraProtection>();
        pyraHealth = GetComponent<PyraHealth>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        isInteracting = false;
        playerIsHighEnough = false;
        pyraIsGliding = false;
    }
    private void Update()
    {
        Debug.Log("Ahora no estoy en combate!!!");
        //Si en algun momento pyra no está en la navmesh, la tpeamos al punto mas cercano en ella.
        if (!agent.isOnNavMesh && !isInPlatform)
        {
            NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 10f, NavMesh.AllAreas);

            agent.Warp(hit.position);
        }

        AIManager();
        RotationManager();
        AnimationManager();
    }

    private void FixedUpdate()
    {
        Physics.Raycast(player.transform.position, -player.transform.up, out RaycastHit hit, 1000f, whatIsGround);

        playerIsHighEnough = Vector3.Distance(player.transform.position, hit.point) >= 5f ? true : false;

        if(Physics.CheckSphere(transform.position, detectionRadius, interactable))
        {
            Collider[] interactables = Physics.OverlapSphere(transform.position, detectionRadius, interactable);

            foreach(Collider interactable in interactables)
            {
                OnInteractuableCollision(interactable);
            }
        }
    }

    private void AIManager()
    {
        if (player.GetComponent<BrelloOpenManager>().GetIsOpen() && !player.IsSwimming())
        {
            stayUnderBrello = true;
            canChasePlayer = false;
            if (pyraHealth.GetIsProtected())
            {
                agent.speed = walkSpeed;
            }
        }
        else
        {
            canChasePlayer = true;
            stayUnderBrello = false;
            agent.speed = runSpeed;
        }

        if (combatManager.GetIsInCombat())
        {
            Debug.Log("Estoy en combate!!");
        }
        else
        {
            Debug.Log("No estoy en combate!!");
        }

        if(!player.IsSwimming() && player.IsGlading() && playerIsHighEnough && !pyraIsGliding 
            && Vector3.Distance(player.transform.position, transform.position) <= minDistToTP)
        {
            pyraIsGliding = true;
            canChasePlayer = false;
            stayUnderBrello = false;
            isMovingToInteractuable = false;

            if (isInteracting)
            {
                currentInteractuable.ResetInter();
            }

            transform.DOScale(0f, 0.2f).OnComplete(() =>
            {
                 currentParticle = Instantiate(tpStartParticles, transform.position, Quaternion.identity);
            });
        }
        else if(pyraIsGliding && player.IsGrounded())
        {
            pyraIsGliding = false;

            Physics.Raycast(player.transform.GetChild(0).position, Vector3.down, out RaycastHit hit, 100f, whatIsGround);

            NavMesh.SamplePosition(hit.point, out NavMeshHit navHit, 1f, NavMesh.AllAreas);

            agent.Warp(navHit.position);
            currentParticle.GetComponent<PyraBall>().posToFinish(navHit.position);  
        }
        else if(pyraIsGliding && player.IsSwimming())
        {
            pyraIsGliding = false;
            isInPlatform = true;

            agent.Warp(platform.position);

            platform.SetParent(null);
            transform.SetParent(platform);
            platform.SetParent(platformParent);

            animator.SetTrigger("SitDown");

            currentParticle.transform.SetParent(platformParent);

            currentParticle.GetComponent<PyraBall>().FinishInWater(platform.localPosition);
        }
        else if(canChasePlayer && !isInteracting && !pyraProtection.GetIsInRain() && !isInPlatform && !isMovingToInteractuable && !moveToPlatform)
        {
            Vector3 dir = player.transform.position - transform.position;
            float rayDistance = Vector3.Distance(transform.position, player.transform.position);
            if (!Physics.Raycast(transform.position, dir, rayDistance, rainMask))
            {
                Vector3 relativeDistance = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
                if (!(Vector3.Distance(transform.position, relativeDistance) <= agent.stoppingDistance))
                {
                    agent.destination = player.transform.position;
                }
                else
                {
                    agent.velocity = Vector3.zero;
                    agent.SetDestination(transform.position);
                }             
            }
            else
            {
                agent.velocity = Vector3.zero;
                agent.SetDestination(transform.position);
            }
        }
        else if(canChasePlayer && !isInteracting && pyraProtection.GetIsInRain() && !isInPlatform)
        {
            agent.SetDestination(transform.position);
        }
        else if(moveToPlatform)
        {
            if (!isJumping)
            {
                if (isInteracting)
                {
                    if (currentInteractuable)
                    {
                        currentInteractuable.ResetInter();
                        detectedObjects.Clear();
                        currentInteractuable = null;
                    }
                }
                //Este sistema de condiciones comprueba lo siguiente:
                //Si el player pulsa la E en el agua pero pyra está muy lejos de el, esta se acercará hasta que esté a 
                //la suficiente distancia como para saltar encima de el.
                if (Vector3.Distance(transform.position, player.transform.position) <= distanceToJump)
                {
                    if (player.IsSwimming())
                    {
                        isJumping = true;

                        //Desactivamos el agent para que podamos mover a pyra mediante rigidbody.
                        agent.enabled = false;
                        posToJump = platform.transform.position;
                        player.BlockMovement();
                        animator.SetTrigger("JumpToPlatform");
                    }
                    else
                    {
                        moveToPlatform = false;
                    }
                }
                else
                {
                    agent.SetDestination(player.transform.position);
                }
            }
        }
        else if(stayUnderBrello && (!pyraProtection.GetIsInRain() || pyraHealth.GetIsProtected()) && !player.IsSwimming())
        {
            agent.SetDestination(player.transform.GetChild(0).position);

            if (currentInteractuable)
            {
                isMovingToInteractuable = false;
                currentInteractuable.ResetInter();
                detectedObjects.Clear();
                currentInteractuable = null;
            }

        }
        else if(isMovingToInteractuable && !stayUnderBrello && !moveToPlatform && !isInPlatform && !isInteracting)
        {
            agent.SetDestination(currentInteractuable.transform.position);
        }
    }

    private void RotationManager()
    {
        if (isJumping)
        {
            Vector3 rotation = new Vector3(posToJump.x, transform.position.y, posToJump.z);
            transform.DOLookAt(rotation, 0.5f);
        }
        else if(stayUnderBrello && !player.IsSwimming())
        {
            Vector3 rotation = new Vector3(player.transform.GetChild(0).position.x, transform.position.y, player.transform.GetChild(0).position.z);
            transform.DOLookAt(rotation, 0.5f);
            
        }
        else if((isMovingToInteractuable || isInteracting || isInteracting && player.IsSwimming()) && !moveToPlatform)
        {
            Vector3 rotation = new Vector3(currentInteractuable.transform.position.x, transform.position.y, currentInteractuable.transform.position.z);
            transform.DOLookAt(rotation, 0.5f);
        }
        else if(!player.IsMoving())
        {
            Vector3 rotation = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
            transform.DOLookAt(rotation, 0.5f);
        }
        else
        {
            Vector3 rotation = new Vector3(agent.steeringTarget.x, transform.position.y, agent.steeringTarget.z);
            transform.DOLookAt(rotation, 0.5f);
        }
    }

    private void AnimationManager()
    {
        animator.SetBool("isWalking", agent.velocity != Vector3.zero);
        animator.SetFloat("Speed", agent.speed);
    }

    #region MOVEMENT_FUNCTIONS

    //Funcion para el salto desde tierra hasta plataforma (para el animator).
    public void JumpToPlatform()
    {     
        transform.DOJump(platform.position, jumpPower, 1, jumpDuration)
        .OnStart(() =>
        {
            //Desactivamos fisicas propias de unity y ponemos que la plataforma destino no tenga parent
            //para que el movimiento sea en world en lugar de local.
            rb.isKinematic = false;
            platform.SetParent(null);

        })
        .OnComplete(EnableAgentOnPlatform);
    }

    private void EnableAgentOnPlatform()
    {
        //Ponemos kinematic el rb para que no le afecten fuerzas externas.
        //Cambiamos los parents para que el movimiento no se mueva en world sino en local.
        //Finalmente colocamos a pyra en el origen de coords de su posicion local.
        animator.SetTrigger("Land");
        animator.SetBool("landedOnPlatform", true);
        rb.isKinematic = true;

        isJumping = false;
        isInPlatform = true;
        moveToPlatform = false;

        transform.SetParent(platform);
        transform.localPosition = Vector3.zero;

        //Reactivamos el movimiento del player;
        player.EnableMovement();
        platform.SetParent(platformParent);

    }

    private void EnableAgentOnGround()
    {
        animator.SetTrigger("Land");
        animator.SetBool("landedOnPlatform", false);
        //Hacemos que le afecten las fisicas y que vuelva a navegar por la navmesh.
        rb.isKinematic = true;
        isJumping = false;
        agent.enabled = true;

        if (!player.IsSwimming())
        {
            canChasePlayer = true;
        }
        isInPlatform = false;

        //Habilitamos el movimiento del player y volvemos a poner el padre que toca (armature) a la plataforma.
        player.EnableMovement();
        platform.SetParent(platformParent);
    }

    //Funcion para el salto desde plataforma hacia la tierra (en animator).
    public void StartJump()
    {
        //Hacemos que tanto pyra como la plataforma no tengan parent para que el movimiento se haga en world en lugar de en local.
        platform.SetParent(null);
        transform.SetParent(null);

        //Desactivamos fisicas y hacemos que el player no pueda moverse, para evitar que el spot de salto de pyra se desplace.
        rb.isKinematic = false;

        rb.DOJump(posToJump, jumpPower, 1, jumpDuration).OnComplete(EnableAgentOnGround);
    }

    public void JumpToGround(Vector3 _posToJump)
    {
        isJumping = true;
        animator.SetBool("landedOnPlatform", false);
        animator.SetTrigger("WakeUp");
        animator.SetTrigger("JumpToGround");
        player.BlockMovement();

        posToJump = _posToJump; 
    }

    public void RefreshDetectedObject()
    {
        //Elimina de la lista al que estaba siguiendo
        detectedObjects.Remove(currentInteractuable);
        if (detectedObjects.Count > 0)
            currentInteractuable = detectedObjects[0];

        //Si la lista esta vacia persigue al player
        else
        {
            canChasePlayer = true;
            isMovingToInteractuable = false;
        }
    }

    #endregion MOVEMENT_FUNCTIONS

    private void OnInteractuableCollision(Collider other)
    {
        if (other.TryGetComponent(out Interactable inter))
        {
            if (detectedObjects.Contains(inter))
            {
                return;
            }

            //Dejamos de seguir al player cuando entra un interactuable
            canChasePlayer = false;

            //Activar el modo Interactuable
            isMovingToInteractuable = true;

            //Añadimos cada interactuable con el cual colisionamos
            detectedObjects.Add(inter);

            //Y por cada interactuable ordenamos la lista
            detectedObjects.Sort(SortByPriority);

            //Como la lista esta ordenad, asignamos el indice 0 al 'CurrentInteractuable'
            currentInteractuable = detectedObjects[0];
        }
    }

    private int SortByPriority(Interactable p1, Interactable p2)
    {
        return p1.priority.CompareTo(p2.priority);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}