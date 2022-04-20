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
    bool canDoJumpAttack;
    bool isStuned;

    //Bools Phases
    bool isInPhase2;
    bool isInPhase4;
   

    Vector3 startPosition;

    [Header("Settings Trash Settings")]
    [SerializeField] float timeTrashAattack;
    [SerializeField] float throwTrashPower;
    [SerializeField] GameObject trashPrefab;
    [SerializeField] Transform trashSpawn;

    [Header("Settins Assault Attack")]
    [SerializeField] float speedAssault;
    [SerializeField] float accelerationAssault;
    [SerializeField] LayerMask layerMakAttackAssault;
    Vector3 posToGo;
    bool isDistanceToGo;


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
        PhasesManager();
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
    
    #region Throw Trash Attack
    void AttackTrowTrash()
    {
        transform.DOLocalRotate(Vector3.zero, 0.5f);
        anim.SetTrigger("AttackThrowTrash");
    }
    //Se ejecuta en la animacion de trow trash
    void ThrowTrash()
    {
        GameObject trash = Instantiate(trashPrefab, trashSpawn.position, Quaternion.identity);
        Vector3 playerDir_1 = (player.transform.position - trash.transform.position).normalized;
        trash.GetComponent<Rigidbody>().AddForce(playerDir_1 * throwTrashPower, ForceMode.Impulse);
    }
    #endregion
    
    void InvokeBoiler()
    {
        if (currentBoilersActive < maxBoilers)
        {
            boilers[currentBoilersActive].SetActive(true);
            currentBoilersActive++;
        }
    }

    #region Attack Punch Attack
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

    #endregion

    #region Jump attack
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
                Stune(true);
                          
            });
            sequence.AppendInterval(.3f);
            
            sequence.AppendCallback(() =>
            {
                collider.isTrigger = false;
                navMeshAgent.enabled = true;

            });

            sequence.AppendInterval(timeStuned);

            sequence.AppendCallback(() => { 
                canDoJumpAttack = false; 
                isJumpingAttack = false;
                Stune(false);
            });
        }
    }
    bool CheckIfPlayerInside()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 3,layerMaskWallDetect);
        foreach (Collider col in colliders)
        {
            if (col.TryGetComponent(out PlayerController _pc))
            {
                return true;
            }
        }
        return false;
    }
    void AddImpulseToPlayer()
    {
        anim.SetBool("IsFalling", false);
        if (CheckIfPlayerInside())
            player.GetComponent<Rigidbody>().AddForce(player.transform.right.normalized * impulseForceOnPlayer, ForceMode.Impulse);
            
        //Do Explosion force in player

    }

    #endregion

    #region Chasing Attack
    void ChasingAttack()
    {
        if (!isAssaltingPlayer && canAssaultPlayer && !isStuned)
        {
            posToGo = GetPosToAssult();
            
            anim.SetTrigger("AttackAssault");
            anim.SetBool("IsChasing", true);
            
            canAssaultPlayer = false;
            isAssaltingPlayer = true;

            //Look at player
            Vector3 lookAt = player.transform.position;
            transform.DOLookAt(lookAt, .5f);

            //Move to player
            navMeshAgent.SetDestination(posToGo);
            
            navMeshAgent.speed = speedAssault;
            navMeshAgent.acceleration = accelerationAssault;
        }

        //Comprobacion por distancia y por tag
         isDistanceToGo = Vector3.Distance(transform.position, posToGo) <= navMeshAgent.stoppingDistance;
        
        print(Vector3.Distance(transform.position, posToGo));
        if (isAssaltingPlayer && CheckCollision("Player"))
        {
            AddImpulseToPlayer();
        }
        if (isDistanceToGo && isAssaltingPlayer)
        {          
            isAssaltingPlayer = false;
            posToGo = Vector3.zero;
            navMeshAgent.velocity = Vector3.zero;
            navMeshAgent.ResetPath();
            StartCoroutine(ResumeCanAssaultPlayer());
        }
    }
    IEnumerator ResumeCanAssaultPlayer()
    {
        Stune(true);
        anim.SetTrigger("Stuned");
        
        anim.SetBool("IsChasing", false);

        yield return new WaitForSeconds(timeStuned);
        Stune(false);
        canAssaultPlayer = true;
        

        if (currentPhase == Phases.PHASE_5)
            canDoJumpAttack = true;
    }

    Vector3 GetPosToAssult()
    {
        //Raycast forward
        RaycastHit hit;
        Debug.DrawRay(transform.position, transform.forward * Mathf.Infinity, Color.red);
        
        if (Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity, layerMakAttackAssault))
        {
            return hit.point;
        }
        else
        {
            return player.transform.position;
        }
    }
    #endregion

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

    private void PhasesManager()
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
    public void ChangePhase(float _lifeBoss)
    {
        if (_lifeBoss > 66)
        {
            currentPhase = Phases.PHASE_1;
        }

        if (_lifeBoss <= 66 && _lifeBoss > 33)
        {
            currentPhase = Phases.PHASE_2;
        }     
        else if (_lifeBoss <= 33 && currentPhase != Phases.PHASE_5 )
        {
            currentPhase = Phases.PHASE_4;
        }

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
    void Stune(bool _isStuned)
    {
        isStuned = _isStuned;
        if (_isStuned)
        {
        }

        anim.SetBool("IsStuned", isStuned);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(pivotCubeAttack.position, sizeCubePunchAttack);

        Gizmos.DrawWireSphere(wallDetect.position, distanceWallDetect);
   
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 10);
    }
}