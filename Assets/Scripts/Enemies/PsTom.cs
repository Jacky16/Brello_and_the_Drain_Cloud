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
    GameObject player;
    Collider collider;

    //Bools variables
    bool canAssaultPlayer = true;
    bool isAssaltingPlayer;
    bool isJumpingAttack;

    //Bools Phases
    bool isInPhase2;
    bool isInPhase4;
    bool isInPhase5;
    bool canDoJumpAttack;

    Vector3 startPosition;

    [Header("Settings Trash Settings")]
    [SerializeField] float timeTrashAattack;
    [SerializeField] float throwTrashPower;
    [SerializeField] GameObject trashPrefab;
    [SerializeField] Transform trashSpawn;



    [Header("Settins Assault Attack")]
    [SerializeField] float speedAssault;
    [SerializeField] float accelerationAssault;
    float counterAssault = 0;
    float timeToAssault = 2;


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

    [Header("Wall Detect Settings")]
    [SerializeField] Transform wallDetect;
    [SerializeField] LayerMask layerMaskWallDetect;
    [SerializeField] float distanceWallDetect = 5;
    
    [SerializeField]enum Phases {PHASE_1,PHASE_2,PHASE_3,PHASE_4,PHASE_5}
    [SerializeField] Phases currentPhase = Phases.PHASE_1;    
    
    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
        collider = GetComponent<Collider>();
        anim = GetComponent<Animator>();
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
            Phase4();
        }
        else if (currentPhase == Phases.PHASE_5)
        {
            Phase5();
        }
       
    }
    
    #region Phases
    private void Phase1()
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
        anim.SetTrigger("Triggered");
        yield return new WaitForSeconds(2);
        JumpReturn();
        yield return new WaitForSeconds(jumpReturnDuration + 0.5f);
        AttackPunch();
        for (int i = 0; i < 2; i++)
        {
            AttackTrowTrash();
            yield return new WaitForSeconds(timeTrashAattack);
        }
      
        //Pasar a la fase 3
        yield return new WaitForSeconds(timeTrashAattack);
        anim.SetTrigger("Triggered");
        yield return new WaitForSeconds(2);
        currentPhase = Phases.PHASE_3;

    }
    private void Phase3()
    {
        ChasingAttack();
    }

    private void Phase4()
    {
        if (!isInPhase4)
        {
            isInPhase4 = true;
            StartCoroutine(Phase4Coroutine());
        }
    }
    IEnumerator Phase4Coroutine()
    {
        anim.SetTrigger("Triggered");
        yield return new WaitForSeconds(2);
        JumpReturn();
        
        for (int i = 0; i < 4; i++)
        {
            AttackTrowTrash();
            yield return new WaitForSeconds(timeTrashAattack);
        }
        
        AttackPunch();
        yield return new WaitForSeconds(punchAttackTime);

        InvokeBoiler();

        //Pasar a la fase 5
        yield return new WaitForSeconds(timeTrashAattack);
        anim.SetTrigger("Triggered");
        yield return new WaitForSeconds(2);
        currentPhase = Phases.PHASE_5;

    }
    
    private void Phase5()
    {
        if (canDoJumpAttack)
        {
            JumpAttack();
        }
        else
        {
            ChasingAttack();
        }

    }

    #endregion

    #region Main Attacks
    void AttackTrowTrash()
    {
        anim.SetTrigger("AttackThrowTrash");
    }
    //Se ejecuta en la animacion de trow trash
    void ThrowTrash()
    {
        GameObject trash = Instantiate(trashPrefab, trashSpawn.position, Quaternion.identity);
        Vector3 playerDir_1 = (player.transform.position - trash.transform.position).normalized;
        trash.GetComponent<Rigidbody>().AddForce(playerDir_1 * throwTrashPower, ForceMode.Impulse);
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
            sequence.AppendCallback(() => anim.SetTrigger("AttackJump"));
            sequence.Append(transform.DOJump(currentPosPlayer , jumpAttackPower, 1, jumpAttackDuration));
                
            //On complete
            sequence.AppendCallback(() =>
            {
                AddImpulseToPlayer();
                collider.isTrigger = false;
                navMeshAgent.enabled = true;               
            });

            sequence.AppendInterval(timeStuned);

            sequence.AppendCallback(() => { 
                canDoJumpAttack = false; 
                isJumpingAttack = false; 
            });
        }
    }
    void ChasingAttack()
    {
        if (!isAssaltingPlayer && canAssaultPlayer)
        {
            anim.SetTrigger("AttackAssault");
            anim.SetBool("IsChasing", true);
            
            canAssaultPlayer = false;
            isAssaltingPlayer = true;

            navMeshAgent.SetDestination(player.transform.position);

            navMeshAgent.speed = speedAssault;
            navMeshAgent.acceleration = accelerationAssault;
        }

        //Comprobacion por distancia y por tag
        bool isDistance = Vector3.Distance(transform.position, navMeshAgent.destination) <= navMeshAgent.stoppingDistance;
        
        if (isAssaltingPlayer && (CheckCollision("Player") || isDistance))
        {
            anim.SetBool("IsChasing", false);
            
            navMeshAgent.ResetPath();
            navMeshAgent.velocity = Vector3.zero;
            
            isAssaltingPlayer = false;
            
            StartCoroutine(ResumeCanAssaultPlayer());
        }
    }

    IEnumerator ResumeCanAssaultPlayer()
    {
        yield return new WaitForSeconds(timeStuned);
        canAssaultPlayer = true;
        
        if (currentPhase == Phases.PHASE_5)
        {
            canDoJumpAttack = true;
        }
    }
    Sequence JumpReturn()
    {
        navMeshAgent.enabled = false;

        Sequence sequence = DOTween.Sequence();
        //sequence.AppendInterval(timeStuned);
        sequence.AppendCallback(() => anim.SetTrigger("JumpReturn"));

        sequence.Append(transform.DOJump(startPosition, jumpReturnPower, 1, jumpReturnDuration));

        sequence.OnStart(() =>
        { 
            navMeshAgent.enabled = false;
            
            anim.SetBool("IsFalling", true);
        });
        sequence.OnComplete(() =>
        {         
            anim.SetBool("IsFalling", false);
            navMeshAgent.enabled = true;
            //isAssaltingPlayer = false;
            //canAssaultPlayer = true;
        });
        return sequence;
    }
    
    #endregion

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

    Collider CheckCollision(string _tag)
    {
        //Raycast forward from wallDetect 
        //OverlapSphere on wallDetect

        Collider[] colliders = Physics.OverlapSphere(wallDetect.position, navMeshAgent.stoppingDistance, layerMaskWallDetect);
        foreach (Collider col in colliders)
        {
            if (col.CompareTag(_tag))
            {
                return col;
            }
            Debug.Log(col.tag);
        }
        return null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(pivotCubeAttack.position, sizeCubePunchAttack);

        Gizmos.DrawWireSphere(wallDetect.position, distanceWallDetect);
    }
}
