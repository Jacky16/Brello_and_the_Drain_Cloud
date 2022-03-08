using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    protected override void Start()
    {
        base.Start();
        initSpeed = agent.speed;
        currentDamage = normalDamage;
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void AttackAction()
    {
        StartCoroutine(Assault());
    }

    protected override void ExtraFunctionality()
    {
        currentDamage = normalDamage;
        agent.speed = initSpeed;
    }

    private IEnumerator Assault()
    {
        agent.speed = 0;
        agent.destination = transform.position;

        yield return new WaitForSeconds(timeBeforeAttacking);

        currentDamage = dashDamage;

        dashDistance = Vector3.Distance(transform.position, player.transform.position);
        agent.speed = dashDistance / dashTime;

        agent.destination = player.transform.position;          
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, transform.forward * 10);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.TryGetComponent(out BrelloHealth playerHealth))
        {
            playerHealth.DoDamage(currentDamage);
        }
    }

}
