using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PyraAI : MonoBehaviour
{
    //Variables de Pyra.
    private NavMeshAgent agent;

    [SerializeField] private Transform player;

    //Variables de detección de objetos interactuables.
    private bool canChasePlayer = true, isMovingToInteractuable;

    private float distanceOffset = 0.5f;

    [Header("Radio en el que detectará objetos interactuables.")]
    [SerializeField] private float detectionRadius;

    [Header("Layer(s) con las que interesa que interactue Pyra.")]
    [SerializeField] private LayerMask interactable;

    //Variables de objetos detectados.
    [SerializeField] private List<Interactable> detectedObjects;

    private Interactable currentInteractuable;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
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

    private void OnTriggerEnter(Collider other)
    {
        OnInteractuableCollision(other);
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
