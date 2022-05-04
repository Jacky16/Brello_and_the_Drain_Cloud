using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Juncc_AI : EnemyAI
{
    [SerializeField] GameObject mudBall;
    Animator animator;
    PyraHealth pyraHealth;
    [SerializeField] Transform mudBallCannon;
    bool isOutside;
    bool isChanging;
    protected override void Start()
    {
        isAttacking = false;
        isOutside = false;
        isChanging = false;
        player = GameObject.FindGameObjectWithTag("Player");
        pyraHealth = GameObject.FindGameObjectWithTag("Pyra").GetComponent<PyraHealth>();
        combatManager = player.GetComponent<CombatManager>();
        animator = GetComponent<Animator>();
        canAttack = true;
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (!playerInSightRange || !pyraHealth.GetIsProtected())
        {
            if (combatManager.enemyList.Contains(gameObject))
            {
                combatManager.enemyList.Remove(gameObject);
            }
        }

        Debug.LogError("canAttack: " + canAttack);
        StateChecker();
    }

    protected override void FixedUpdate()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, detectionRadius, playerMask);
    }

    private void StateChecker()
    {
        if ((isOutside && !pyraHealth.GetIsProtected() && !isChanging) || (isOutside && !playerInSightRange))
        {
            isChanging = true;
            isOutside = false;
            animator.SetTrigger("Disappear");
        }
        else if(!isOutside && pyraHealth.GetIsProtected() && playerInSightRange && !isChanging)
        {
            if (!combatManager.enemyList.Contains(gameObject))
            {
                combatManager.enemyList.Add(gameObject);
            }

            isChanging = true;
            isOutside = true;
            transform.DOLookAt(new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z), 0.5f);
            animator.SetTrigger("Appear");
        }
        else if(isOutside && pyraHealth.GetIsProtected() && playerInSightRange)
        {
            transform.DOLookAt(new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z), 0.5f);
            AttackPlayer();
        }
    }
    protected override void ChasePlayer() { }

    public void SetChanging()
    {
        isChanging = false;
    }

    protected override void AttackPlayer()
    {
        if  (canAttack)
        {
            Debug.Log("AttackPlayer");
            canAttack = false;
            animator.SetTrigger("Attack");
        }
    }

    public void SetCanAttack()
    {
        canAttack = true;
    }

    public override void AttackAction()
    {
        Debug.Log("AttackAction");
        isAttacking = true;
        Instantiate(mudBall, mudBallCannon.position, Quaternion.identity);
        StartCoroutine(ResetAttack());
    }

}
