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
    PsTomHealth tomHealth;

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
    [Range(1,6)]
    [SerializeField] int damage;
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
    [SerializeField] Vector2 impulseForceOnPlayer;
    [SerializeField] Transform targeterTransform;

    [Header("Settings Punch Attack")]
    [SerializeField] float punchAttackTime = 1.5f;
    [SerializeField] Vector3 sizeCubePunchAttack;
    [SerializeField] Transform pivotCubeAttack;
    bool isAttackingPunchAttack;

    [Header("Boiler Settings")]
    [SerializeField] GameObject[] boilers;
   
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
        tomHealth = GetComponent<PsTomHealth>();
    }
    private void Start()
    {
        startPosition = transform.position;
        targeterTransform.gameObject.SetActive(false);
    }
    private void Update()
    {
        if (!tomHealth.IsAlive()) return;
        PhasesManager();

        if (isAttackingPunchAttack)
        {
            AttackPunchCheck();
        }
    }

    #region Phases
    private void Phase1()
    {
        HandleChasingAttack();     
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
        yield return new WaitForSeconds(3);    
        for (int i = 0; i < 2; i++)
        {
            AttackTrowTrash();
            yield return new WaitForSeconds(timeTrashAattack);
        }
      
        //Pasar a la fase 3
        yield return new WaitForSeconds(timeTrashAattack);
        currentPhase = Phases.PHASE_3;

    }
    private void Phase3()
    {
        HandleChasingAttack();
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

        InvokeBoilers();

        //Pasar a la fase 5
        yield return new WaitForSeconds(timeTrashAattack);
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
            HandleChasingAttack();
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
    
    void InvokeBoilers()
    {
        for (int i = 0; i < boilers.Length; i++)
        {
            boilers[i].SetActive(true);
        }
    }

    #region Punch Attack
    void AttackPunch()
    {
        if (!isAttackingPunchAttack)
        {
            RotateToPlayer();
            anim.SetTrigger("AttackPunch");          
        }
    }
    void AttackPunchBoolean(int value)
    {
        if (value == 1)
        {
            isAttackingPunchAttack = true;
        }
        else if (value == 0)
        {
            isAttackingPunchAttack = false;
        }
    }
    
   
    //Se ejecuta en la animacion de punch 
    void AttackPunchCheck()
    {
        Collider[] colliders = Physics.OverlapBox(pivotCubeAttack.position, sizeCubePunchAttack, Quaternion.identity);
        foreach (Collider col in colliders)
        {
            if (col.TryGetComponent(out BrelloHealth _bh))
            {
                _bh.DoDamage(damage);
                isAttackingPunchAttack = false;
                player.GetComponent<Rigidbody>().AddForce(transform.right.normalized * impulseForceOnPlayer.x + Vector3.up * impulseForceOnPlayer.y, ForceMode.Force);
                Debug.Log("Ataque al player por el puño");
            }
        }       
    }

    #endregion

    #region Jump attack
    void JumpAttack()
    {
        if (!isJumpingAttack)
        {
            RotateToPlayer();       
            
            isJumpingAttack = true;

            navMeshAgent.enabled = false;

            Sequence sequence = DOTween.Sequence();
            //Delay del ataque
            sequence.AppendInterval(.3f);
            
            Vector3 currentPosPlayer = player.transform.position;
                        
            //Start
            sequence.AppendCallback(() =>
            {
                collider.isTrigger = true;
                targeterTransform.gameObject.SetActive(true);
                targeterTransform.position = new Vector3(transform.position.x,0,transform.position.z);
            });
            
            //Jump Attack
            sequence.AppendCallback(() => anim.SetTrigger("AttackJump"));
            sequence.Append(transform.DOJump(currentPosPlayer, jumpAttackPower, 1, jumpAttackDuration));
            
            //Asignar la posicion del target al player
            Vector3 nextPosTarget = currentPosPlayer;
            nextPosTarget.y = 0;          
            sequence.Join(targeterTransform.DOMove(nextPosTarget, jumpAttackDuration));

            //On complete
            sequence.AppendCallback(() =>
            {
                AddImpulseToPlayer();
                Stune(true);
                targeterTransform.gameObject.SetActive(false);

            });
                       
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
                player.GetComponent<BrelloHealth>().DoDamage(damage);
                return true;
            }
        }
        return false;
    }
    void AddImpulseToPlayer()
    {
        anim.SetBool("IsFalling", false);
        
        if (CheckIfPlayerInside())
        {
            player.GetComponent<BrelloHealth>().DoDamage(damage);
            
            player.GetComponent<PlayerController>().ChangeTypeofMovement(PlayerController.MovementMode.ADD_FORCE,true);
            
            player.GetComponent<Rigidbody>().AddForce(transform.right.normalized * impulseForceOnPlayer.x + Vector3.up * impulseForceOnPlayer.y, ForceMode.Force);
        }
    }

    #endregion

    #region Chasing Attack
    //Se ejecuta en la animacion de la carrerilla
    void ChasingAttack()
    {
        if (!isAssaltingPlayer && !isStuned)
        {
            isAssaltingPlayer = true;
            anim.SetBool("IsChasing", isAssaltingPlayer);

            //Asignar donde va a ir el Boss
            posToGo = GetPosToAssult();
           
            //Move to player
            navMeshAgent.SetDestination(posToGo);
            
            //Aplicar velocidad y aceleración
            navMeshAgent.speed = speedAssault;
            navMeshAgent.acceleration = accelerationAssault;
        }       
    }
    IEnumerator ResumeCanAssaultPlayer()
    {
        Stune(true);      
        
        anim.SetBool("IsChasing", false);

        yield return new WaitForSeconds(timeStuned);
        
        Stune(false);
        canAssaultPlayer = true;
        
        if (currentPhase == Phases.PHASE_5)
        {
            canDoJumpAttack = true;
        }
    }
    
    void HandleChasingAttack()
    {
        if (canAssaultPlayer)
        {
            canAssaultPlayer = false;
            anim.SetTrigger("AttackAssault");

            //Look at player
            RotateToPlayer();
        }
        
        //Comprobacion por distancia y por tag
        if(!isStuned)
            isDistanceToGo = Vector3.Distance(transform.position, posToGo) <= navMeshAgent.stoppingDistance || CheckCollision("Wall");        
        
        if (isAssaltingPlayer && CheckCollision("Player"))
        {
            AddImpulseToPlayer();
        }
        else if (isDistanceToGo && isAssaltingPlayer)
        {
            isAssaltingPlayer = false;
            
            //Parar en seco y quitar la ruta del navmesh
            navMeshAgent.velocity = Vector3.zero;
            navMeshAgent.ResetPath();
            
            //Sistema de Stune
            StartCoroutine(ResumeCanAssaultPlayer());
        }
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

        if (_lifeBoss <= 66 && _lifeBoss > 33 && !isInPhase2)
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
        }
        return null;
    }
    void Stune(bool _isStuned)
    {
        isStuned = _isStuned;
        tomHealth.CanDamage(_isStuned);
        if (_isStuned)
            anim.SetTrigger("Stuned");
        

        anim.SetBool("IsStuned", isStuned);
    }

    void RotateToPlayer(float _duration = 0.5f)
    {
        Vector3 lookAt = player.transform.position;
        transform.DOLookAt(lookAt, _duration);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(pivotCubeAttack.position, sizeCubePunchAttack);

        Gizmos.DrawWireSphere(wallDetect.position, distanceWallDetect);
   
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 10);
    }
}