using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


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
    [SerializeField] private int impulseForce;
    private float initSpeed;
    float initYPos;

    Vector3 attackPos;
    protected override void Start()
    {
        base.Start();
        initSpeed = agent.speed;
        currentDamage = normalDamage;
        initYPos = transform.position.y;
        attackPos = new Vector3(0, 0, 0);
    }

    protected override void Update()
    {
        base.Update();
        transform.position = new Vector3(transform.position.x, initYPos + 0.125f * Mathf.Sin(Time.time * 3) + 0.125f,transform.position.z);

        if(Vector3.Distance(transform.position, attackPos) <= agent.stoppingDistance)
        {
            isAttacking = false;
            currentDamage = normalDamage;
            agent.speed = initSpeed;
        }
    }

    protected override void AttackAction()
    {
        StartCoroutine(Assault());
    }

    private IEnumerator Assault()
    {
        isAttacking = true;

        agent.speed = 0f;
        agent.destination = transform.position;  

        yield return new WaitForSeconds(timeBeforeAttacking);

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
            //playerHealth.gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * impulseForce, ForceMode.Impulse);

            playerHealth.DoDamage(currentDamage);
        }
    }

}
