using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;


[RequireComponent(typeof(SphereCollider))]
public class SteamlingAI : EnemyAI
{
    [Header("Dash variables")]
    private float dashDistance;
    [SerializeField] float dashTime;
    [SerializeField] float timeBeforeAttacking;

    private int currentDamage;
    [SerializeField] private int normalDamage;
    [SerializeField] private int dashDamage;
    private float initSpeed;
    float initYPos;
    Animator animator;

    Vector3 attackPos;
    protected override void Start()
    {
        base.Start();
        agent.autoTraverseOffMeshLink = false;
        animator = transform.GetChild(0).GetComponent<Animator>();
        initSpeed = agent.speed;
        currentDamage = normalDamage;
        initYPos = transform.position.y;
        attackPos = Vector3.zero;
    }

    protected override void Update()
    {
        base.Update();
        transform.position = new Vector3(transform.position.x, initYPos + 0.125f * Mathf.Sin(Time.time * 3) + 0.125f,transform.position.z);

        if(Vector3.Distance(transform.position, attackPos) <= agent.stoppingDistance)
        {
            currentDamage = normalDamage;
            agent.speed = initSpeed;
            animator.SetBool("Charge", false);
        }
    }

    protected override void AttackAction()
    {
        StartCoroutine(Assault());
    }

    private IEnumerator Assault()
    {
        agent.speed = 0f;
        agent.destination = transform.position;

        animator.SetTrigger("Attack");

        yield return new WaitForSeconds(timeBeforeAttacking);

        animator.SetBool("Charge", true);

        currentDamage = dashDamage;

        dashDistance = Vector3.Distance(transform.position, player.transform.position);

        agent.speed = dashDistance / dashTime;
        agent.destination = attackPos = player.transform.position;
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, transform.forward * 10);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.gameObject.TryGetComponent(out BrelloHealth playerHealth))
        {
            //playerHealth.GetComponent<CharacterController>().Move(transform.forward * impulseForce * Time.deltaTime);

            playerHealth.DoDamage(currentDamage);
        }
    }

}
