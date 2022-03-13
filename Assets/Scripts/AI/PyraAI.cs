using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
public class PyraAI : MonoBehaviour
{
    //Variables de Pyra.
    private NavMeshAgent agent;

    private PlayerController player;
    private Rigidbody rb;

    //Variables de detecci�n de objetos interactuables.
    public bool canChasePlayer = true;

    public bool isMovingToInteractuable;

    private float distanceOffset = 0.5f;

    [Header("Radio en el que detectar� objetos interactuables.")]
    [SerializeField] private float detectionRadius;

    [Header("Layer(s) con las que interesa que interactue Pyra.")]
    [SerializeField] private LayerMask interactable;

    //Variables de objetos detectados.
    [SerializeField] private List<Interactable> detectedObjects;

    private Interactable currentInteractuable;

    //Variables de movimiento.
    public bool moveToPlatform = false, isInPlatform = false, isJumping = false;

    [Header("Jump settings")]
    [SerializeField] private float jumpPower;

    [SerializeField] private float jumpDuration;
    [SerializeField] private float distanceToJump;
    [SerializeField] private Transform platform;
    [SerializeField] private Transform platformParent;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody>();
        //platformParent = platform;
    }

    private void Update()
    {
        InteractuableManager();
    }

    private void InteractuableManager()
    {
        if (canChasePlayer)
        {
            agent.SetDestination(player.transform.position);
        }
        else if (moveToPlatform)
        {
            if (!isJumping)
            {
                if (Vector3.Distance(transform.position, player.transform.position) <= distanceToJump)
                {
                    isJumping = true;
                    agent.enabled = false;

                    transform.DOJump(platform.position, jumpPower, 1, jumpDuration)
                        .OnStart(() =>
                        {
                            platform.SetParent(null);
                            player.BlockMovement();
                        })
                        .OnComplete(EnableAgentOnPlatform);
                }
                else
                {
                    agent.SetDestination(player.transform.position);
                }
            }
        }

        //Se mueve a los interactuables si hay algo en la lista
        else if (isMovingToInteractuable)
        {
            //Mientras haya algo a la lista sigue el actual Interactuable
            agent.SetDestination(currentInteractuable.transform.position);

            //Cuando estas cerca del interactuable ve a por el siguiente
            if (Vector3.Distance(transform.position, currentInteractuable.transform.position) <= agent.stoppingDistance)
            {
                currentInteractuable.Interact();

                RefreshDetectedObject();
            }
        }
    }

    #region MOVEMENT_FUNCTIONS

    private void EnableAgentOnPlatform()
    {
        rb.isKinematic = true;
        platform.SetParent(platformParent);
        transform.SetParent(platform);
        transform.localPosition = Vector3.zero;
        player.EnableMovement();

        isJumping = false;
        isInPlatform = true;
        moveToPlatform = false;
    }

    private void EnableAgentOnGround()
    {
        isJumping = false;
        agent.enabled = true;
        canChasePlayer = true;
        isInPlatform = false;
        player.EnableMovement();
        platform.SetParent(platformParent);
    }

    public void JumpToGround(Vector3 posToJump)
    {
        isJumping = true;
        platform.SetParent(null);
        transform.SetParent(null);
        rb.isKinematic = false;
        player.BlockMovement();
        rb.DOJump(posToJump, jumpPower, 1, jumpDuration).OnComplete(EnableAgentOnGround);
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

            //A�adimos cada interactuable con el cual colisionamos
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}