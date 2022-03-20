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
    private NavMeshAgent agent;

    private PlayerController player;
    private Rigidbody rb;
    private PyraProtection pyraProtection;
    private PyraHealth pyraHealth;
    private Animator animator;

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

    //Variables de objetos detectados.
    [SerializeField] private List<Interactable> detectedObjects;

    private Interactable currentInteractuable;

    //Variables de movimiento.
    public bool moveToPlatform = false, isInPlatform = false, isJumping = false;

    [Header("Jump settings")]
    [SerializeField] private float jumpPower;

    [SerializeField] float walkSpeed;
    [SerializeField] float runSpeed;
    [SerializeField] private float jumpDuration;
    [SerializeField] private float distanceToJump;
    [SerializeField] private Transform platform;
    [SerializeField] private Transform platformParent;

    private Vector3 posToJump;
    bool stayUnderBrello;
    private void Awake()
    {
        pyraProtection = GameObject.FindGameObjectWithTag("Player").GetComponent<PyraProtection>();
        pyraHealth = GetComponent<PyraHealth>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        isInteracting = false;
        //platformParent = platform;
    }

    private void Update()
    {
        AIManager();
        RotationManager();
        AnimationManager();
    }

    private void AIManager()
    {
        if (player.GetComponent<BrelloOpenManager>().GetIsOpen())
        {
            stayUnderBrello = true;
            canChasePlayer = false;
            agent.speed = walkSpeed;
        }
        else
        {
            canChasePlayer = true;
            stayUnderBrello = false;
            agent.speed = runSpeed;
        }

        if (canChasePlayer && !isInteracting && !pyraProtection.GetIsInRain() && !isInPlatform && !player.IsSwimming())
        {
            Vector3 dir = player.transform.position - transform.position;
            float rayDistance = Vector3.Distance(transform.position, player.transform.position);

            if (!Physics.Raycast(transform.position, dir, rayDistance, rainMask))
            {
                Vector3 relativeDistance = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);

                if (!(Vector3.Distance(transform.position, relativeDistance) <= agent.stoppingDistance)) {
                    agent.SetDestination(player.transform.position);
                }
                else
                {
                    agent.SetDestination(transform.position);
                }             
            }
            else
            {
                agent.SetDestination(transform.position);
            }
        }
        else if(canChasePlayer && !isInteracting && pyraProtection.GetIsInRain() && !isInPlatform)
        {
            agent.SetDestination(transform.position);
        }
        else if (moveToPlatform)
        {
            if (!isJumping)
            {
                //Este sistema de condiciones comprueba lo siguiente:
                //Si el player pulsa la E en el agua pero pyra está muy lejos de el, esta se acercará hasta que esté a 
                //la suficiente distancia como para saltar encima de el.
                if (Vector3.Distance(transform.position, player.transform.position) <= distanceToJump)
                {
                    isJumping = true;

                    //Desactivamos el agent para que podamos mover a pyra mediante rigidbody.
                    agent.enabled = false;
                    posToJump = platform.transform.position;
                    animator.SetTrigger("JumpToPlatform");
                }
                else
                {
                    agent.SetDestination(player.transform.position);
                }
            }
        }
        else if (stayUnderBrello && (!pyraProtection.GetIsInRain() || pyraHealth.GetIsProtected()) && !player.IsSwimming())
        {
            agent.SetDestination(player.transform.GetChild(0).position);
        }
        //Se mueve a los interactuables si hay algo en la lista
        else if (isMovingToInteractuable && !stayUnderBrello && !moveToPlatform && !isInPlatform)
        {
            //Mientras haya algo a la lista sigue el actual Interactuable
            agent.SetDestination(currentInteractuable.transform.position);

            //Cuando estas cerca del interactuable ve a por el siguiente
            if (Vector3.Distance(transform.position, currentInteractuable.transform.position) <= agent.stoppingDistance + 3)
            {
                currentInteractuable.Interact();
                isInteracting = true;
                RefreshDetectedObject();
            }
        }
    }

    private void RotationManager()
    {
        if (isJumping)
        {
            Vector3 rotation = new Vector3(posToJump.x, transform.position.y, posToJump.z);
            transform.LookAt(rotation);
            Debug.Log("Rota a isJumping");
        }
        else if(isInPlatform)
        {
            //transform.rotation = Quaternion.LookRotation(transform.forward);
            Debug.Log("Rota a isInPlatform");
        }
        else if(stayUnderBrello && !player.IsSwimming())
        {
            Vector3 rotation = new Vector3(player.transform.GetChild(0).position.x, transform.position.y, player.transform.GetChild(0).position.z);
            transform.LookAt(rotation);
            Debug.Log("Rota a stayUnderBrello");
        }
        else if(isMovingToInteractuable)
        {
            Vector3 rotation = new Vector3(currentInteractuable.transform.position.x, transform.position.y, currentInteractuable.transform.position.z);
            transform.LookAt(rotation);
            Debug.Log("Rota a isMovingToInteractuable");
        }
        else
        {
            Vector3 rotation = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
            transform.LookAt(rotation);
            Debug.Log("Rota a Brello");
        }
    }

    private void AnimationManager()
    {
        if ((player.IsMoving() && !player.IsSwimming()) || (isMovingToInteractuable && !isInteracting) || moveToPlatform)
        {
            animator.SetBool("isWalking", true);
            animator.SetFloat("Speed", agent.speed);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }
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
            player.BlockMovement();
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
        platform.SetParent(platformParent);
        transform.SetParent(platform);
        transform.localPosition = Vector3.zero;

        //Reactivamos el movimiento del player;
        player.EnableMovement();

        isJumping = false;
        isInPlatform = true;
        moveToPlatform = false;
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
        player.BlockMovement();

        rb.DOJump(posToJump, jumpPower, 1, jumpDuration).OnComplete(EnableAgentOnGround);
    }

    public void JumpToGround(Vector3 _posToJump)
    {
        isJumping = true;
        animator.SetBool("landedOnPlatform", false);
        animator.SetTrigger("WakeUp");
        animator.SetTrigger("JumpToGround");

        posToJump = _posToJump; 
    }

    private void RefreshDetectedObject()
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

    private void OnTriggerEnter(Collider other)
    {
        OnInteractuableCollision(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Interactable inter))
        {
            detectedObjects.Remove(inter);

            if (detectedObjects.Count != 0)
            {
                detectedObjects.Sort(SortByPriority);

                //Como la lista esta ordenad, asignamos el indice 0 al 'CurrentInteractuable'
                currentInteractuable = detectedObjects[0];
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}