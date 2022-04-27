using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Juncc_AI : EnemyAI
{
    [SerializeField] GameObject mudBall;
    Animator animator;
    PyraHealth pyraHealth;
    [SerializeField] Transform mudBallCannon;
    bool isOutside;
    protected override void Start()
    {
        isAttacking = false;
        isOutside = false;
        player = GameObject.FindGameObjectWithTag("Player");
        pyraHealth = GameObject.FindGameObjectWithTag("Pyra").GetComponent<PyraHealth>();
        combatManager = player.GetComponent<CombatManager>();
        animator = GetComponent<Animator>();
        canAttack = true;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        StateChecker();
    }

    protected override void FixedUpdate()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, detectionRadius, playerMask);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRadius, playerMask);
    }
    protected override void Idle()
    {
        
    }

    private void StateChecker()
    {
        if (isOutside && !pyraHealth.GetIsProtected())
        {
            animator.SetTrigger("Disappear");
        }
        else if(!isOutside && pyraHealth.GetIsProtected() && playerInSightRange)
        {
            animator.SetTrigger("Appear");
        }
    }
    protected override void ChasePlayer() { }

    public void SetAppeared()
    {
        isOutside = true;
    }

    public void SetDisappeared()
    {
        isOutside = false;
    }
    protected override void AttackPlayer()
    {
        if(isOutside && canAttack)
        {
            animator.SetTrigger("Attack");
        }
    }
    public override void AttackAction()
    {
        isAttacking = true;
        Instantiate(mudBall, mudBallCannon.position, Quaternion.identity);
        canAttack = false;
        StartCoroutine(ResetAttack());
    }

}
