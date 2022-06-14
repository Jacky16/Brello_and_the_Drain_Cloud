using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoilerAI : MonoBehaviour
{
    [SerializeField] float attackRadius;
    [SerializeField] LayerMask playerMask;
    [SerializeField] GameObject coalBall;
    [SerializeField] GameObject shootParticles;
    Animator animator;
    Vector3 positionToSpawn;

    [SerializeField, Tooltip("Tiempo mínimo entre disparos del boiler.")] float minTimeBetweenAttacks;
    [SerializeField, Tooltip("Tiempo máximo entre disparos del boiler.")] float maxTimeBetweenAttacks;

    float timeBetweenAttacks;

    private bool playerDetected;
    private bool canAttack;

    Transform player;
    void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        canAttack = false;
        positionToSpawn = transform.GetChild(0).transform.position;
        timeBetweenAttacks = Random.Range(minTimeBetweenAttacks, maxTimeBetweenAttacks);
        StartCoroutine(ResetAttack());
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
            canAttack = false;
            animator.SetTrigger("Attack");
        }
    }
    public void ShootCoal()
    {
        AkSoundEngine.PostEvent("Shoot_Boiler", WwiseManager.instance.gameObject);
        Instantiate(coalBall, positionToSpawn, Quaternion.identity);
        Instantiate(shootParticles, positionToSpawn, Quaternion.identity);
        StartCoroutine(ResetAttack());
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
