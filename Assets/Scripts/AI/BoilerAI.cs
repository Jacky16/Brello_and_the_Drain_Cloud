using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoilerAI : MonoBehaviour
{
    [SerializeField] float attackRadius;
    [SerializeField] LayerMask playerMask;
    [SerializeField] GameObject coalBall;
    Vector3 positionToSpawn;

    [SerializeField] float timeBetweenAttacks;

    private bool playerDetected;
    private bool canAttack;

    Transform player;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        canAttack = true;
        positionToSpawn = transform.GetChild(0).transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerDetected)
        {
            Attack();
        }
    }

    private void Attack()
    {
        if (canAttack)
        {
            //Set trigger de ataque en el animator
            ShootCoal();
            canAttack = false;
            StartCoroutine(ResetAttack());
        }
    }
    public void ShootCoal()
    {
        Instantiate(coalBall, positionToSpawn, Quaternion.identity);
    }

    IEnumerator ResetAttack()
    {
        yield return new WaitForSeconds(timeBetweenAttacks);
        canAttack = true;
    }
    private void FixedUpdate()
    {
        playerDetected = Physics.CheckSphere(transform.position, attackRadius, playerMask);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}
