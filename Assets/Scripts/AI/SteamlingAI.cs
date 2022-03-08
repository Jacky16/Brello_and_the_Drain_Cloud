using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteamlingAI : EnemyAI
{
    [Header("Dash variables")]
    private float dashDistance;
    [SerializeField] float dashTime;
    [SerializeField] float timeBeforeAttacking;

    private float initSpeed;
    

    protected override void Start()
    {
        base.Start();
        initSpeed = agent.speed;
    }

    protected override void Update()
    {
        base.Update();

        if (!agent.hasPath)
        {
            agent.speed = initSpeed;
        }
    }
    protected override void AttackAction()
    {
        StartCoroutine(Assault());
    }

    private IEnumerator Assault()
    {
        agent.speed = 0;
        agent.destination = transform.position;

        yield return new WaitForSeconds(timeBeforeAttacking);

        dashDistance = Vector3.Distance(transform.position, player.transform.position);
        agent.speed = dashDistance / dashTime;

        agent.destination = player.transform.position;          
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, transform.forward * 10);
    }
}
