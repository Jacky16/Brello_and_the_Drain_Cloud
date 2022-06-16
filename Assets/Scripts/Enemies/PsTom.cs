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
    PsTomEffects tomEffects;

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

    [Header("Settings Assault Attack")]
    [SerializeField] float speedAssault;
    [SerializeField] float accelerationAssault;
    [SerializeField] float distanceCheckWall;
    [SerializeField] LayerMask layerMakAttackAssault;
    [SerializeField] Vector3 checkPlayerDamageBox;
    [SerializeField] Vector2 impulseAttackAssaultToPlayer;
    [SerializeField] ParticleSystem dashParticles;
    

    Vector3 posToGo;
    bool isDistanceToGo;


    [Header("Stun Settings")]
    [SerializeField] float timeStuned;
    [SerializeField] GameObject shieldParticle;

    [Header("Return Jump Settings")]
    [SerializeField] float jumpReturnPower;
    [SerializeField] float jumpReturnDuration;
    
    [Header("Attack Jump Settings")]
    [SerializeField] float jumpAttackPower;
    [SerializeField] float jumpAttackDuration;
    [SerializeField] Vector2 impulseAttackJumpToPlayer;
    [SerializeField] Transform targeterTransform;
    [SerializeField] GameObject jumpParticles;

    [Header("Settings Punch Attack")]
    [SerializeField] float punchAttackTime = 1.5f;
    [SerializeField] Vector3 sizeCubePunchAttack;
    [SerializeField] Transform pivotCubeAttack;
    [SerializeField] Vector2 impulseAttackPunchToPlayer;
    [SerializeField] GameObject punchParticle;
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
        tomEffects = GetComponent<PsTomEffects>();
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

        //Advice:Lanzar el objeto
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

            //Advice:Empieza el ataque del puño
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
                player.GetComponent<Rigidbody>().AddForce(-transform.localPosition.normalized * impulseAttackJumpToPlayer.x + Vector3.up * impulseAttackJumpToPlayer.y, ForceMode.Impulse);
                player.GetComponent<PlayerController>().ChangeTypeofMovement(PlayerController.MovementMode.ADD_FORCE, true);
            
                Debug.Log("Ataque al player por el puño");
                Instantiate(punchParticle, _bh.transform.position, Quaternion.identity);
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
            
            //Mover el targeter al sitio que va a saltar
            sequence.Join(targeterTransform.DOMove(nextPosTarget, jumpAttackDuration));

            //On complete
            sequence.AppendCallback(() =>
            {
                if (CheckIfPlayerInside())
                {
                    //Advice:Ha acabado el salto
                    AddImpulseToPlayer(impulseAttackJumpToPlayer);
                    player.GetComponent<BrelloHealth>().DoDamage(damage);
                }
                Instantiate(jumpParticles, transform.position, Quaternion.identity);                
            });
                       
            sequence.AppendCallback(() =>
            {
                collider.isTrigger = false;
                navMeshAgent.enabled = true;
                targeterTransform.gameObject.SetActive(false);

            });

            sequence.AppendInterval(timeStuned);

            sequence.AppendCallback(() => { 
                canDoJumpAttack = false; 
                isJumpingAttack = false;
                Stun(false);
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
    void AddImpulseToPlayer( Vector2 _force)
    {
        anim.SetBool("IsFalling", false);
               
        player.GetComponent<PlayerController>().ChangeTypeofMovement(PlayerController.MovementMode.ADD_FORCE, true);

        player.GetComponent<Rigidbody>().AddForce(transform.right.normalized * _force.x + Vector3.up * _force.y, ForceMode.Impulse);
    }

    #endregion

    #region Chasing Attack
    //Se ejecuta en la animacion de la carrerilla
    void ChasingAttack()
    {
        if (!isAssaltingPlayer && !isStuned)
        {
            
            RotateToPlayer(0.1f).OnComplete(() =>
            {
                //Advice: Empieza a perseguir al player
                isAssaltingPlayer = true;

                anim.SetBool("IsChasing", isAssaltingPlayer);

                //Asignar donde va a ir el Boss
                posToGo = GetPosToAssult();

                //Move to player
                navMeshAgent.SetDestination(posToGo);

                //Aplicar velocidad y aceleración
                navMeshAgent.speed = speedAssault;
                navMeshAgent.acceleration = accelerationAssault;

                //Empiezo particulas de carrerilla
                dashParticles.Play();

            });
        }       
    }
    IEnumerator ResumeCanAssaultPlayer()
    {
        Stun(true);      
        
        anim.SetBool("IsChasing", false);

        yield return new WaitForSeconds(timeStuned);
        
        Stun(false);
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
        if (!isStuned)
            isDistanceToGo =  CheckCollisionWall();

        if (isAssaltingPlayer && CheckCollision("Player"))
        {
            //Advice: Ha golpeado al player
            print("Ha golpeado al player con Raycast");
            AddImpulseToPlayer(impulseAttackAssaultToPlayer);
            player.GetComponent<BrelloHealth>().DoDamage(damage);
        }
        else if (isDistanceToGo && isAssaltingPlayer)
        {
            //Advice: Acaba la ruta por que se ha chocado
            isAssaltingPlayer = false;
            
            //Parar en seco y quitar la ruta del navmesh
            navMeshAgent.velocity = Vector3.zero;
            navMeshAgent.ResetPath();

            //Sistema de Stun
            AkSoundEngine.PostEvent("Crash_PSTom", WwiseManager.instance.gameObject);
            dashParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            StartCoroutine(ResumeCanAssaultPlayer());
        }
    }

    Vector3 GetPosToAssult()
    {
        //Raycast forward
        RaycastHit hit;
        Vector3 pos;
        if (Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity, layerMakAttackAssault))
        {
         return hit.point;
        }
        else
        {
            Debug.Break();
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
            //Advive:Se ha chocado con el player
            
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
    bool CheckCollision(string _tag)
    {
        //Raycast forward from wallDetect 
        //OverlapBox 
        Collider[] colliders = Physics.OverlapBox(wallDetect.position, checkPlayerDamageBox, transform.rotation, layerMaskWallDetect);
        foreach (Collider col in colliders)
        {
            if (col.CompareTag(_tag))
            {
                return true;
            }
        }
        return false;
    }
    bool CheckCollisionWall()
    {
        //Raycast forward from wallDetect 
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, distanceCheckWall, layerMaskWallDetect))
        {
            if (hit.collider.tag == "Wall")
            {
                return true;
            }
        }
        return false;
    }
    void Stun(bool _isStuned)
    {
        isStuned = _isStuned;
        tomHealth.CanDamage(_isStuned);
        shieldParticle.SetActive(!_isStuned);

        if (_isStuned)
            anim.SetTrigger("Stuned");
        

        anim.SetBool("IsStuned", isStuned);
    }

    Tween RotateToPlayer(float _duration = 0.5f)
    {
        Vector3 lookAt = player.transform.position;
        return transform.DOLookAt(lookAt, _duration);
    }
   

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(pivotCubeAttack.position, sizeCubePunchAttack);
       
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * distanceCheckWall);
        //Draw box
        Gizmos.DrawWireCube(wallDetect.position, checkPlayerDamageBox);
    }
}