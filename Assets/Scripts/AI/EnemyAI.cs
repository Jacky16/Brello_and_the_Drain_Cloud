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
    CombatManager combatManager;

    [Header("AI Variables")]
    [SerializeField] protected float attackRadius;
    [SerializeField] protected float detectionRadius;
    [SerializeField] protected float timeBetweenAttacks;
    protected bool playerInSightRange, playerInAttackRange, canAttack;

    Rigidbody rb;
    protected virtual void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        combatManager = player.GetComponent<CombatManager>();
        agent = GetComponent<NavMeshAgent>();
        canAttack = true;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (!playerInSightRange)
        {
            if (combatManager.enemyList.Contains(gameObject))
            {
                combatManager.enemyList.Remove(gameObject);
            }

            Idle();
        }

        if (playerInSightRange)
        {
            if (!combatManager.enemyList.Contains(gameObject))
            {
                combatManager.enemyList.Add(gameObject);
            }
            transform.LookAt(new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z));
        }

        if (playerInSightRange && !playerInAttackRange)
        {
            ChasePlayer();
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
        rb.velocity = new Vector3(0, 0, 0);
        rb.angularVelocity = new Vector3(0, 0, 0);
    }

    protected virtual void Idle()
    {
        agent.SetDestination(transform.position);
    }
    protected virtual void ChasePlayer()
    {
        agent.destination = player.transform.position;
        ExtraFunctionality();
    }

    protected virtual void ExtraFunctionality() { }
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
        Debug.LogError("El problema está en el resetAttack");
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
