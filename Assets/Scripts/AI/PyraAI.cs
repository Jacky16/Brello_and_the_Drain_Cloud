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
    private Transform player;
    private Rigidbody rb;

    //Variables de detección de objetos interactuables.
    public bool canChasePlayer = true, isMovingToInteractuable;
    private float distanceOffset = 0.5f;

    [Header("Radio en el que detectará objetos interactuables.")]
    [SerializeField] private float detectionRadius;

    [Header("Layer(s) con las que interesa que interactue Pyra.")]
    [SerializeField] private LayerMask interactable;

    //Variables de objetos detectados.
    [SerializeField] private List<Interactable> detectedObjects;
    private Interactable currentInteractuable;

    //Variables de movimiento.
    public bool moveToPlatform = false, isInPlatform = false, isJumping = false;
    private Vector3 platformPos;
    [SerializeField] float jumpPower;
    [SerializeField] float jumpDuration;
    [SerializeField] float distanceToJump;
    [SerializeField] Transform platform;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody>();
        platformPos = player.GetChild(0).position;
    }

    private void Update()
    {
        InteractuableManager();
    }

    private void InteractuableManager()
    {
        if (canChasePlayer)
        {
            agent.SetDestination(player.position);
        }
        else if (moveToPlatform)
        {
            if(Vector3.Distance(transform.position, player.position) <= distanceToJump)
            {
                isJumping = true;
                agent.enabled = false;
                rb.DOJump(platformPos, jumpPower, 1, jumpDuration).OnComplete(EnableAgentOnPlatform);
            }
            else
            {
                agent.SetDestination(player.position);
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
        isJumping = false;
        isInPlatform = true;
        moveToPlatform = false;
        transform.SetParent(platform);
    }
    private void EnableAgentOnGround()
    {
        isJumping = false;
        agent.enabled = true;
        canChasePlayer = true;
        isInPlatform = false;
    }
    public void JumpToGround(Vector3 posToJump)
    {
        isJumping = true;
        transform.SetParent(null);
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
