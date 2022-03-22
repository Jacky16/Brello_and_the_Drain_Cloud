using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class PsTom : MonoBehaviour
{
    //References/Components
    NavMeshAgent navMeshAgent;
    PsTomHealth psTomHealth;
    GameObject player;

    //Bools variables
    bool canAssaultPlayer = true;
    bool isAssaltingPlayer;
    bool isJumpingAttack;

    Vector3 startPosition;

    [Header("Settins Assault Attack")]
    [SerializeField] float speedAssault;
    [SerializeField] float accelerationAssault;

    [Header("Stune Settings")]
    [SerializeField] float timeStuned;

    [Header("Return Jump Settings")]
    [SerializeField] float jumpReturnPower;
    [SerializeField] float jumpReturnDuration;
    

    [Header("Attack Jump Settings")]
    [SerializeField] float jumpAttackPower;
    [SerializeField] float jumpAttackDuration;



    enum Phases {PHASE_1,PHASE_2,PHASE_3,PHASE_4,PHASE_5,PHASE_6 }
    Phases currentPhase = Phases.PHASE_1;
    
    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        psTomHealth = GetComponent<PsTomHealth>();
        player = GameObject.FindGameObjectWithTag("Player");
    }
    private void Start()
    {
        startPosition = transform.position;

    }
    private void Update()
    {
        JumpAttack();
    }

    void BossManager()
    {
        switch (currentPhase)
        {
            case Phases.PHASE_1:
                break;
            case Phases.PHASE_2:
                break;
            case Phases.PHASE_3:
                break;
            case Phases.PHASE_4:
                break;
            case Phases.PHASE_5:
                break;
            case Phases.PHASE_6:
                break;
        }
    }

    #region Main Attacks
    void ShootPunch()
    {

    }
    void InvokeTrush()
    {

    }
    void InvokeBoiler()
    {

    }
    void PunchNormal()
    {

    }
    void JumpAttack()
    {
        Vector3 currentPosPlayer = player.transform.position;
        if (!isJumpingAttack)
        {
            isJumpingAttack = true;

            navMeshAgent.enabled = false;

            Sequence sequence = DOTween.Sequence();
            sequence.AppendInterval(2);
            sequence.Append(transform.DOJump(currentPosPlayer, jumpAttackPower, 1, jumpAttackDuration).SetEase(Ease.InOutExpo));
            sequence.AppendInterval(timeStuned);
            sequence.Append(transform.DOJump(startPosition, jumpReturnPower, 1, jumpReturnDuration));
            sequence.OnComplete(() => isJumpingAttack = false);
        }
    }
    void Assault()
    {
        Vector3 currentPosPlayer = player.transform.position;
        if (!isAssaltingPlayer && canAssaultPlayer)
        {
            canAssaultPlayer = false;
            isAssaltingPlayer = true;

            navMeshAgent.SetDestination(currentPosPlayer);

            navMeshAgent.speed = speedAssault;
            navMeshAgent.acceleration = accelerationAssault;
        }

        float distance = Vector3.Distance(transform.position, navMeshAgent.destination);
        if(distance <= navMeshAgent.stoppingDistance && isAssaltingPlayer)
        {
            isAssaltingPlayer = false;
            JumpReturn();
        }
    }
    #endregion

  
    Sequence JumpReturn()
    {
        navMeshAgent.enabled = false;
        Sequence sequence = DOTween.Sequence();
        sequence.AppendInterval(timeStuned);

        sequence.Append(transform.DOJump(startPosition, jumpReturnPower, 1, jumpReturnDuration));

        sequence.OnStart(() =>
        { 
            navMeshAgent.enabled = false;
        });
        sequence.OnComplete(() =>
        {
            navMeshAgent.enabled = true;

            isAssaltingPlayer = false;
            canAssaultPlayer = true;
        
        });
        return sequence;
    }
    void ChangePhase(Phases _phase)
    {
        currentPhase = _phase;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Wall") && isAssaltingPlayer)
        {
            isAssaltingPlayer = false;
            JumpReturn();
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player") && isAssaltingPlayer)
        {
            isAssaltingPlayer = false;
            JumpReturn();
        }
        if (collision.collider.CompareTag("Wall") && isAssaltingPlayer)
        {
            isAssaltingPlayer = false;
            JumpReturn();
        }
    }


}
