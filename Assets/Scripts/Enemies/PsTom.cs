using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class PsTom : MonoBehaviour
{
    //References/Components
    Animator anim;
    NavMeshAgent navMeshAgent;
    PsTomHealth psTomHealth;
    GameObject player;
    Collider collider;

    //Bools variables
    bool canAssaultPlayer = true;
    bool isAssaltingPlayer;
    bool isJumpingAttack;

    //Bools Phases
    bool isInPhase1;
    bool isInPhase2;
    bool isInPhase3;
    bool isInPhase4;
    bool isInPhase5;
    bool isInPhase6;

    Vector3 startPosition;

    [Header("Settings Trash Settings")]
    [SerializeField] float timeTrashAattack;
    [SerializeField] float throwTrashPower;
    [SerializeField] float timeToThrowTrash;
    [SerializeField] GameObject trashPrefab;
    [SerializeField] Transform trashSpawn;
    float counterTrash = 0;
    int trahsThrowed = 0;
    int maxTrashThrowed = 2;


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
    [SerializeField] float impulseForceOnPlayer = 150;

    [Header("Settings Punch Attack")]
    [SerializeField] float punchAttackTime = 1.5f;
    [SerializeField] Vector3 sizeCubePunchAttack;
    [SerializeField] Transform pivotCubeAttack;

    [Header("Boiler Settings")]
    [SerializeField] GameObject[] boilers;
    int maxBoilers;
    int currentBoilersActive = 0;


    enum Phases {PHASE_1,PHASE_2,PHASE_3,PHASE_4,PHASE_5,PHASE_6 }
    Phases currentPhase = Phases.PHASE_1;

    PsTomHealth bossHealth;
    
    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        psTomHealth = GetComponent<PsTomHealth>();
        player = GameObject.FindGameObjectWithTag("Player");
        collider = GetComponent<Collider>();
        anim = GetComponent<Animator>();
        bossHealth = GetComponent<PsTomHealth>();
    }
    private void Start()
    {
        startPosition = transform.position;
        maxBoilers = boilers.Length;
    }
    private void Update()
    {
        if (currentPhase == Phases.PHASE_1)
        {
            Phase1();
        }
        else if (currentPhase == Phases.PHASE_2)
        {
            Phase2();
        }
        else if (currentPhase == Phases.PHASE_3)
        {
            Phase3();
        }
        else if (currentPhase == Phases.PHASE_4)
        {

        }
        else if (currentPhase == Phases.PHASE_5)
        {
        }
        else if (currentPhase == Phases.PHASE_6)
        {
        }
    }

    private void Phase3()
    {
        ChasingAttack();
    }

    private void Phase2()
    {
        if (!isInPhase2)
        {
            isInPhase2 = true;
            StartCoroutine(Phase2Coroutine());
        }
    }
    IEnumerator Phase2Coroutine()
    {     
        JumpReturn();
        yield return new WaitForSeconds(jumpReturnDuration);
        AttackPunch();
        yield return new WaitForSeconds(timeTrashAattack);
        ThrowTrash();
    }

    private void Phase1()
    {    
        
        ChasingAttack();
        
    }

    #region Main Attacks
    void ThrowTrash()
    {
        counterTrash += Time.deltaTime;
        if (counterTrash >= timeToThrowTrash && trahsThrowed < maxTrashThrowed)
        {
            counterTrash = 0;
            trahsThrowed++;

            GameObject trash = Instantiate(trashPrefab, trashSpawn.position, Quaternion.identity);
         
            Vector3 playerDir_1 = (player.transform.position - trash.transform.position).normalized;

            trash.GetComponent<Rigidbody>().AddForce(playerDir_1 * throwTrashPower, ForceMode.Impulse);
        }
        
        //Si sobrepasa el numero de basuras, comprueba la fase y la cambia
        if (trahsThrowed >= maxTrashThrowed)
        {
            currentPhase = Phases.PHASE_3;
        }

    }
    void InvokeBoiler()
    {
        if (currentBoilersActive < maxBoilers)
        {
            boilers[currentBoilersActive].SetActive(true);
            currentBoilersActive++;
        }
    }
    void AttackPunch()
    {
        anim.SetTrigger("AttackPunch");
    }
    //Se ejecuta en la animacion de punch 
    void AttackPunchCheck()
    {
        Collider[] colliders = Physics.OverlapBox(pivotCubeAttack.position, sizeCubePunchAttack, Quaternion.identity);
        foreach (Collider col in colliders)
        {
            if (col.TryGetComponent(out BrelloHealth _bh))
            {
                _bh.DoDamage(1);
                Debug.Log("Ataque al player por el puño");
            }
        }
       
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

            //Start
            sequence.AppendCallback(() => collider.isTrigger = true);

            //Jump Attack
            sequence.AppendCallback(() => anim.SetTrigger("JumpAttack"));
            sequence.Append(transform.DOJump(currentPosPlayer + new Vector3(0.5f,.5f,0.5f), jumpAttackPower, 1, jumpAttackDuration));
                
            //On complete
            sequence.AppendCallback(() => AddImpulseToPlayer());

            sequence.AppendInterval(.5f);

            sequence.AppendCallback(() => collider.isTrigger = false);

            sequence.AppendInterval(timeStuned);

            //Jump return
            sequence.Append(transform.DOJump(startPosition, jumpReturnPower, 1, jumpReturnDuration));
            sequence.OnComplete(() => isJumpingAttack = false);
        }
    }
    void ChasingAttack()
    {
        Vector3 currentPosPlayer = player.transform.position;
        
        if (!isAssaltingPlayer && canAssaultPlayer)
        {
            anim.SetBool("IsChasing", true);
            anim.SetTrigger("AttackChase");
            canAssaultPlayer = false;
            isAssaltingPlayer = true;

            navMeshAgent.SetDestination(currentPosPlayer);

            navMeshAgent.speed = speedAssault;
            navMeshAgent.acceleration = accelerationAssault;
        }

        float distance = Vector3.Distance(transform.position, navMeshAgent.destination);
        if(distance <= navMeshAgent.stoppingDistance && isAssaltingPlayer)
        {
            //TODO: Ejecutar animacion de Stop
            anim.SetBool("IsChasing", false);
            
            isAssaltingPlayer = false;
            navMeshAgent.velocity = Vector3.zero;

            Invoke("ResumeCanAssaultPlayer", 2f);
        }
    }

    void ResumeCanAssaultPlayer()
    {
        canAssaultPlayer = true;
    }
    
    #endregion


    Sequence JumpReturn()
    {
        navMeshAgent.enabled = false;
        Sequence sequence = DOTween.Sequence();
        sequence.AppendInterval(timeStuned);
        sequence.AppendCallback(() => anim.SetTrigger("JumpReturn"));

        sequence.Append(transform.DOJump(startPosition, jumpReturnPower, 1, jumpReturnDuration));

        sequence.OnStart(() =>
        { 
            navMeshAgent.enabled = false;
            
            anim.SetBool("IsFalling", true);
        });
        sequence.OnComplete(() =>
        {
            navMeshAgent.enabled = true;
            isAssaltingPlayer = false;
            canAssaultPlayer = true;
        
            anim.SetBool("IsFalling", false);
        });
        return sequence;
    }
    public void ChangePhase(float _lifeBoss)
    {
        if (_lifeBoss > 66)
        {
            currentPhase = Phases.PHASE_1;
        }
        if (_lifeBoss <= 66)
        {
            currentPhase = Phases.PHASE_2;
        }
        else if (_lifeBoss <= 33)
        {
            currentPhase = Phases.PHASE_4;
        }

    }
    void AddImpulseToPlayer()
    {
        anim.SetBool("IsFalling", false);
        if (CheckIfPlayerInside())
        {
            Vector3 dir = player.transform.position - transform.position;
            player.GetComponent<AddForceCharacterController>().AddImpact(dir, 150);
                
            //player.GetComponent<PlayerController>().HandleAddForce(dir, 20);
        }
    }
    bool CheckIfPlayerInside()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 2);
        foreach (Collider col in colliders)
        {
            if (col.TryGetComponent(out PlayerController _pc)){
                return true;
            }
        }
        return false;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isAssaltingPlayer = false;
            Invoke("ResumeCanAssaultPlayer", 1.5f);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player") && isAssaltingPlayer)
        {
            isAssaltingPlayer = false;
            Invoke("ResumeCanAssaultPlayer", 1.5f);
        }
        if (collision.collider.CompareTag("Wall") && isAssaltingPlayer)
        {
            anim.SetTrigger("Collision");
            isAssaltingPlayer = false;
            Invoke("ResumeCanAssaultPlayer", 1.5f);

            //JumpReturn();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(pivotCubeAttack.position, sizeCubePunchAttack);
    }
}
