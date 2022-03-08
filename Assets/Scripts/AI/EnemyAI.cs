using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{
    [Header("NavMesh variables")]
    [SerializeField] float acceleration;
    [SerializeField] float deceleration;
    protected NavMeshAgent agent;

    [Header("Player detection variables")]
    [SerializeField] protected LayerMask playerMask;
    protected GameObject player;

    [Header("AI Variables")]
    [SerializeField] protected float attackRadius;
    [SerializeField] protected float detectionRadius;
    [SerializeField] protected float timeBetweenAttacks;
    protected bool playerInSightRange, playerInAttackRange, canAttack;
    protected virtual void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        agent = GetComponent<NavMeshAgent>();
        canAttack = true;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (playerInSightRange && !playerInAttackRange)
        {
            ChasePlayer();
        }

        if (playerInSightRange)
        {
            transform.LookAt(new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z));
        }

        if (playerInAttackRange)
        {
            
            AttackPlayer();
        }
    }

    protected virtual void FixedUpdate()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, detectionRadius, playerMask);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRadius, playerMask);
    }

    protected virtual void ChasePlayer()
    {
        agent.destination = player.transform.position;
    }

    protected void AttackPlayer()
    {
        if (canAttack)
        {
            AttackAction();
            canAttack = false;
            StartCoroutine(ResetAttack());
        }
    }

    protected virtual void AttackAction()
    {

    }

    private IEnumerator ResetAttack()
    {
        yield return new WaitForSeconds(timeBetweenAttacks);
        canAttack = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}
