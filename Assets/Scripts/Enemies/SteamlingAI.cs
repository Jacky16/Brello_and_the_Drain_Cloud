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
    [SerializeField] float dashSpeed;
    [SerializeField] float timeBeforeAttacking;

    private int currentDamage;
    [SerializeField] private int normalDamage;
    [SerializeField] private int dashDamage;
    private float initSpeed;
    float initYPos;
    Animator animator;

    NavMeshPath dashPath;

    private bool isDashing;

    Vector3 attackPos;
    protected override void Start()
    {
        base.Start();
        isDashing = false;
        agent.autoTraverseOffMeshLink = false;
        animator = GetComponentInChildren<Animator>();
        initSpeed = agent.speed;
        currentDamage = normalDamage;
        initYPos = transform.position.y;
        attackPos = transform.GetChild(2).transform.position;
    }

    protected override void Update()
    {
        base.Update();
        //transform.position = new Vector3(transform.position.x, initYPos + 0.125f * Mathf.Sin(Time.time * 3) + 0.125f, transform.position.z);

        if (isDashing && agent.remainingDistance <= agent.stoppingDistance + 0.25f)
        {
            isAttacking = false;
            isDashing = false;
            currentDamage = normalDamage;
            agent.speed = initSpeed;
            animator.SetBool("Charge", false);
        }
    }

    public override void AttackAction()
    {
        StartCoroutine(Assault());
    }

    private IEnumerator Assault()
    {

        agent.speed = 0f;
        agent.destination = transform.position;

        animator.SetTrigger("Attack");
        //AkSoundEngine.PostEvent("Preparing_Charge_Steamling", WwiseManager.instance.gameObject);

        yield return new WaitForSeconds(timeBeforeAttacking);

        animator.SetBool("Charge", true);
        //AkSoundEngine.PostEvent("Charging_Steamling", WwiseManager.instance.gameObject);

        currentDamage = dashDamage;

        dashDistance = Vector3.Distance(transform.position, player.transform.position);

        agent.speed = dashSpeed;
        agent.destination = attackPos = transform.GetChild(2).position;
        dashPath = agent.path;
        isDashing = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, transform.forward * 10);
    }

    public void SetBigRadius()
    {
        detectionRadius = 500;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.TryGetComponent(out BrelloHealth playerHealth))
        {
            //playerHealth.GetComponent<CharacterController>().Move(transform.forward * impulseForce * Time.deltaTime);

            playerHealth.DoDamage(currentDamage);
        }
    }
}